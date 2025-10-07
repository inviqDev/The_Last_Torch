using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Runtime
{
    /// <summary>
    /// Overlay (topmost) → unload old scene (always) → GC → real progress for LOAD → enforce min time → activate
    /// → payload/hooks → hide overlay.
    /// </summary>
    [DisallowMultipleComponent]
    public class TransitionService : Singleton<TransitionService>
    {
        // ---------- Inspector: Overlay / Startup ----------
        
        [Header("UI")]
        [SerializeField] private LoadingOverlay loadingOverlayPrefab;
        [SerializeField] private float overlayFade = 0.35f;

        [Header("Startup")]
        [SerializeField] private TransitionAsset initialTransition;

        // ---------- Inspector: Progress layout ----------

        [Header("Progress Layout")]
        [Tooltip("Portion of the progress bar allocated to the UNLOAD phase (0..1). The remainder is used for LOAD.")]
        [SerializeField, Range(0.05f, 0.5f)] private float unloadProgressPortion = 0.14f;

        // ---------- Runtime state ----------

        private LoadingOverlay _loadingOverlay;
        private Scene? _currentContentScene;        // Last active content scene (not Bootstrap)
        private Scene _loadedSceneBuffer;           // Scene loaded during this transition
        private float _minOverlayTimer;             // Per-transition timer from TransitionAsset
        private AsyncOperation _pendingLoadOp;      // Load op tracked across steps
        private bool _isTransitionRunning;          // Guard against concurrent transitions

        // ---------- Unity lifecycle ----------

        protected override void Awake()
        {
            base.Awake();
            CreateOverlayOrDie(); // Create and configure the overlay once
            overlayFade = 0f;
        }

        private void Start()
        {
            if (initialTransition)
            {
                StartCoroutine(LoadRoutine(initialTransition));
            }
        }

        // ---------- Public API ----------

        public void Load(TransitionAsset asset)
        {
            if (!asset)
            {
                UnityEngine.Assertions.Assert.IsNotNull(asset, "TransitionAsset is null");
                return;
            }
            
            if (!asset.TargetScene.IsAssigned)
            {
                UnityEngine.Assertions.Assert.IsTrue(asset.TargetScene.IsAssigned, "TargetScene is not assigned");
                return;
            }

            if (_isTransitionRunning)
            {
                // Prevent starting another transition while one is already running
                UnityEngine.Assertions.Assert.IsFalse(_isTransitionRunning, "Transition already in progress");
                return;
            }

            StartCoroutine(LoadRoutine(asset));
        }

        // ---------- Core routine (high-level narrative) ----------

        private IEnumerator LoadRoutine(TransitionAsset asset)
        {
            // Sanity check: overlay must exist after Awake
            UnityEngine.Assertions.Assert.IsNotNull(_loadingOverlay, "Loading overlay failed to initialize.");
            if (!_loadingOverlay)
            {
                yield break;
            }

            _isTransitionRunning = true;
            try
            {
                BeginTransition(asset);

                // 1) Overlay + pre-hooks
                yield return ShowOverlayAndRunPreHooks(asset);

                // 2) Always unload the previous scene (if any)
                yield return UnloadPreviousSceneAlways(asset);

                // 3) Load the new scene until it's ready for activation (progress ~0.9)
                yield return LoadNewSceneUntilReady(asset);

                // 4) Enforce minimal overlay time right before activation
                yield return EnforceMinimalOverlayTime();

                // 5) Activate and bind the loaded scene handle
                yield return ActivateAndBindLoadedScene(asset);

                // 6) Payload post-hooks + scene hooks (still under overlay)
                RunPostLoadHooks(asset);

                // 7) Hide overlay — player now sees the new scene
                overlayFade = 0.5f;
                yield return HideOverlayAndFinish();
            }
            finally
            {
                _isTransitionRunning = false;
                _pendingLoadOp = null;
            }
        }

        // ---------- Step 0: per-transition init ----------

        private void BeginTransition(TransitionAsset asset)
        {
            _loadedSceneBuffer = default;
            _pendingLoadOp = null;
            _minOverlayTimer = asset.MinOverlayShowTime;
        }

        // ---------- Step 1: overlay + payload pre-hooks ----------

        private IEnumerator ShowOverlayAndRunPreHooks(TransitionAsset asset)
        {
            if (asset.ShowProgress)
            {
                _loadingOverlay.SetProgress(0f);
            }

            yield return _loadingOverlay.ShowLoadingProgress(overlayFade);
            asset.Payload?.OnWillLoad();
        }

        // ---------- Step 2: always UNLOAD previous scene (if present) ----------

        private IEnumerator UnloadPreviousSceneAlways(TransitionAsset asset)
        {
            if (!(_currentContentScene.HasValue && _currentContentScene.Value.IsValid()))
            {
                yield break; // Nothing to unload (e.g., first boot)
            }

            var old = _currentContentScene.Value;
            var unloadOp = SceneManager.UnloadSceneAsync(old);
            UnityEngine.Assertions.Assert.IsNotNull(unloadOp, "Unload AsyncOperation did not start");

            // Visualize UNLOAD as [0 .. unloadProgressPortion]
            if (asset.ShowProgress)
            {
                var start = 0f;
                var end = unloadProgressPortion;

                while (!unloadOp.isDone)
                {
                    // On some platforms the progress can stay 0 until completion — this is fine; still tick the timer.
                    var p = Mathf.Clamp01(unloadOp.progress);
                    var vis = Mathf.Lerp(start, end, p);
                    _loadingOverlay.SetProgress(vis);

                    TickMinOverlayTimer();
                    yield return null;
                }

                _loadingOverlay.SetProgress(end);
            }
            else
            {
                while (!unloadOp.isDone)
                {
                    TickMinOverlayTimer();
                    yield return null;
                }
            }

            // Memory cleanup after unload
            yield return Resources.UnloadUnusedAssets();
            GC.Collect();
            _currentContentScene = null;
        }

        // ---------- Step 3: LOAD new scene until ready (progress to 0.9) ----------

        private IEnumerator LoadNewSceneUntilReady(TransitionAsset asset)
        {
            var sceneId = BuildSceneId(asset);
            _pendingLoadOp = SceneManager.LoadSceneAsync(sceneId, LoadSceneMode.Additive);
            UnityEngine.Assertions.Assert.IsNotNull(_pendingLoadOp, "Load AsyncOperation did not start");
            if (_pendingLoadOp == null)
            {
                yield break;
            }

            _pendingLoadOp.allowSceneActivation = false;

            if (asset.ShowProgress)
            {
                var start = unloadProgressPortion; // Continue after UNLOAD
                var range = 1f - start;

                while (_pendingLoadOp.progress < 0.9f)
                {
                    // AsyncOperation.progress grows from 0 to ~0.9 prior to activation
                    var real01 = Mathf.Clamp01(_pendingLoadOp.progress / 0.9f);
                    var vis = start + range * real01;
                    _loadingOverlay.SetProgress(vis);

                    TickMinOverlayTimer();
                    yield return null;
                }

                // Reach 1.0 visually before activation
                _loadingOverlay.SetProgress(1f);
            }
            else
            {
                while (_pendingLoadOp.progress < 0.9f)
                {
                    TickMinOverlayTimer();
                    yield return null;
                }
            }
        }

        // ---------- Step 4: ensure minimal overlay time (single place) ----------

        private IEnumerator EnforceMinimalOverlayTime()
        {
            if (_minOverlayTimer > 0f)
            {
                yield return new WaitForSecondsRealtime(_minOverlayTimer);
                _minOverlayTimer = 0f;
            }
        }

        // ---------- Step 5: activate and bind loaded scene ----------

        private IEnumerator ActivateAndBindLoadedScene(TransitionAsset asset)
        {
            UnityEngine.Assertions.Assert.IsNotNull(_pendingLoadOp, "Pending load op is null before activation");
            if (_pendingLoadOp == null)
            {
                yield break;
            }

            _pendingLoadOp.allowSceneActivation = true;
            while (!_pendingLoadOp.isDone)
            {
                yield return null;
            }

            _loadedSceneBuffer = GetLoadedScene(asset);
            UnityEngine.Assertions.Assert.IsTrue(_loadedSceneBuffer.IsValid() && _loadedSceneBuffer.isLoaded, "Target scene failed to load");
            if (!_loadedSceneBuffer.IsValid() || !_loadedSceneBuffer.isLoaded)
            {
                yield break;
            }

            if (asset.SetActiveAfterLoad)
            {
                SceneManager.SetActiveScene(_loadedSceneBuffer);
            }

            _currentContentScene = _loadedSceneBuffer;
        }

        // ---------- Step 6: payload post-hooks + scene hooks ----------

        private void RunPostLoadHooks(TransitionAsset asset)
        {
            if (!(_loadedSceneBuffer.IsValid() && _loadedSceneBuffer.isLoaded))
            {
                return;
            }
            
            asset.Payload?.OnDidLoad(_loadedSceneBuffer);
            // TryFindAndCallOnSceneLoadMethods(asset);
        }

        private void TryFindAndCallOnSceneLoadMethods(TransitionAsset asset)
        {
            var roots = _loadedSceneBuffer.GetRootGameObjects();
            foreach (var hook in roots.SelectMany(r => r.GetComponentsInChildren<ISceneLoadHook>(true)))
            {
                try
                {
                    hook.OnSceneLoaded(asset.Payload);
                }
                catch (Exception e)
                {
                    UnityEngine.Assertions.Assert.IsTrue(false, $"OnSceneLoaded exception:\n{e}");
                }
            }
        }

        // ---------- Step 7: hide overlay ----------

        private IEnumerator HideOverlayAndFinish()
        {
            yield return _loadingOverlay.Hide(overlayFade);
        }

        // ---------- Helpers ----------

        private void TickMinOverlayTimer()
        {
            if (_minOverlayTimer > 0f)
            {
                _minOverlayTimer -= Time.unscaledDeltaTime;
            }
        }

        /// <summary>
        /// Create the overlay once in Awake; enforce topmost ordering and input blocking.
        /// </summary>
        private void CreateOverlayOrDie()
        {
            UnityEngine.Assertions.Assert.IsNotNull(loadingOverlayPrefab, "Loading overlay Prefab is not assigned.");

            _loadingOverlay = Instantiate(loadingOverlayPrefab, transform);
            // _loadingOverlay.gameObject.transform.SetParent(null);
            // DontDestroyOnLoad(_loadingOverlay.gameObject);
            
            _loadingOverlay.SetDefaultValues();
        }

        private static string BuildSceneId(TransitionAsset asset)
        {
            var path = asset.TargetScene.Path;
            return !string.IsNullOrEmpty(path) ? path : asset.TargetScene.Name;
        }

        private static Scene GetLoadedScene(TransitionAsset asset)
        {
            var path = asset.TargetScene.Path;
            return !string.IsNullOrEmpty(path)
                ? SceneManager.GetSceneByPath(path)
                : SceneManager.GetSceneByName(asset.TargetScene.Name);
        }
    }
}
