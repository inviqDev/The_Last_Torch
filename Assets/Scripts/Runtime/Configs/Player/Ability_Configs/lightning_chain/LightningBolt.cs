using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Runtime
{
    [RequireComponent(typeof(LineRenderer))]
    public class LightningBolt : MonoBehaviour, IPoolable
    {
        [Header("Unique Key in pool dictionary")]
        [SerializeField] protected string uniquePoolKey;
        public string UniquePoolKey => uniquePoolKey;
        
        [Header("Shape")]
        [SerializeField] private float pointsPerUnit = 3f;
        [SerializeField] private int   minPoints = 8;
        [SerializeField] private int   maxPoints = 64;
        [SerializeField] private float noiseAmplitude = 0.2f;
        [SerializeField] private float noiseScaleWithDistance = 0.15f;
        [SerializeField] private float thickness = 0.07f;

        [Header("Lifetime")]
        [SerializeField] private float duration = 0.12f;  // короткая жизнь сегмента
        [SerializeField] private float fadeOut  = 0.06f;
        [SerializeField] private bool  followWhileAlive;

        [Header("UV Scroll")]
        [SerializeField] private float uvScrollSpeed = 6f;

        private LineRenderer lr;
        private Material     mat;
        private Color        baseColor;

        private Transform fromT, toT;
        private Vector3 fromPos, toPos;

        private void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.useWorldSpace   = true;
            lr.textureMode     = LineTextureMode.Tile;
            lr.alignment       = LineAlignment.View;
            lr.widthMultiplier = thickness;
            lr.widthCurve      = AnimationCurve.Constant(0f, 1f, 1f);

            mat = new Material(lr.sharedMaterial);
            lr.material = mat;
            baseColor = mat.HasProperty("_TintColor")
                ? mat.GetColor("_TintColor")
                : (mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white);
        }

        public void Launch(Transform from, Transform to, float? overrideDuration = null, bool? follow = null)
        {
            fromT = from; toT = to;
            if (overrideDuration.HasValue) duration = overrideDuration.Value;
            if (follow.HasValue) followWhileAlive = follow.Value;
            
            SetAlpha(1f);
            StopAllCoroutines();
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            float t = 0f;
            while (t < duration)
            {
                if (followWhileAlive)
                {
                    if (fromT) fromPos = fromT.position;
                    if (toT)   toPos   = toT.position;
                }
                else if (t == 0f)
                {
                    fromPos = fromT ? fromT.position : fromPos;
                    toPos   = toT   ? toT.position   : toPos;
                }

                RebuildOnce();

                if (mat && mat.HasProperty("_MainTex"))
                {
                    var off = mat.mainTextureOffset; off.x -= uvScrollSpeed * Time.deltaTime;
                    mat.mainTextureOffset = off;
                }

                t += Time.deltaTime;
                yield return null;
            }

            // fade
            float f = 0f;
            while (f < fadeOut)
            {
                SetAlpha(1f - Mathf.Clamp01(f / fadeOut));
                f += Time.deltaTime;
                yield return null;
            }
            SetAlpha(0f);

            Pool.Instance?.ReturnToPool(this);
        }

        private void RebuildOnce()
        {
            var dir = toPos - fromPos;
            var dist = dir.magnitude;
            if (dist < 0.0001f) { lr.positionCount = 0; return; }

            dir /= dist;
            var up = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(up, dir)) > 0.95f) up = Vector3.right;
            var right = Vector3.Cross(dir, up).normalized;
            var up2   = Vector3.Cross(right, dir).normalized;

            int count = Mathf.Clamp(Mathf.RoundToInt(dist * pointsPerUnit), minPoints, maxPoints);
            if (count < 2) count = 2;
            lr.positionCount = count;

            float amp = noiseAmplitude + dist * noiseScaleWithDistance;
            for (int i = 0; i < count; i++)
            {
                float t = i / (count - 1f);
                var p = Vector3.Lerp(fromPos, toPos, t);

                var env = 1f - Mathf.Abs(t * 2f - 1f);
                env = Mathf.Pow(env, 0.6f);

                var r1 = (Random.value - 0.5f);
                var r2 = (Random.value - 0.5f);
                var offset = (right * r1 + up2 * r2) * (amp * env);
                lr.SetPosition(i, p + offset);
            }
        }

        private void SetAlpha(float a)
        {
            if (!mat) return;
            if (mat.HasProperty("_TintColor"))
            {
                var c = baseColor; c.a *= a; mat.SetColor("_TintColor", c);
            }
            else if (mat.HasProperty("_Color"))
            {
                var c = baseColor; c.a *= a; mat.SetColor("_Color", c);
            }
        }
        
        public void OnGetFromPool()
        {
            transform.SetParent(null);
            gameObject.SetActive(true);
        }
        
        public void OnReturnToPool()
        {
            StopAllCoroutines();
            gameObject.SetActive(false);
            transform.SetParent(Pool.Instance?.transform);
        }
    }
}




// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using Random = UnityEngine.Random;
//
// namespace Runtime
// {
//     [RequireComponent(typeof(LineRenderer))]
//     public class LightningBolt : AbilityVFX
//     {
//         [Header("Chain")]
//         [SerializeField] private int   bounceAmount   = 3;       // доп. прыжков ПОСЛЕ первого
//         [SerializeField] private float maxBounceDistance  = 5f;  // если хотим переопределять контекстный — оставь для дефолта
//
//         [Header("Shape")]
//         [SerializeField] private float pointsPerUnit = 3f;
//         [SerializeField] private int   minPoints = 8;
//         [SerializeField] private int   maxPoints = 64;
//         [SerializeField] private float noiseAmplitude = 0.2f;
//         [SerializeField] private float noiseScaleWithDistance = 0.15f;
//         [SerializeField] private float thickness = 0.07f;
//
//         [Header("Lifetime")]
//         [SerializeField] private float linkDuration = 0.5f;
//         [SerializeField] private float fadeOut      = 0.06f;
//         [SerializeField] private bool  chainIsFollowingTarget;
//
//         [Header("UV Scroll (optional)")]
//         [SerializeField] private float uvScrollSpeed = 6f;
//         
//         private LineRenderer lineRenderer;
//         private Material     runtimeMat;
//         private Color        baseColor;
//
//         private Vector3 startPos, endPos;
//         private readonly List<EnemyModel> affected = new();
//         private List<EnemyModel> availableEnemies = new();
//
//         private void Awake()
//         {
//             lineRenderer = GetComponent<LineRenderer>();
//             
//             lineRenderer.widthMultiplier = thickness;
//             lineRenderer.widthCurve = AnimationCurve.Constant(0f, 1f, 1f);
//             
//             lineRenderer.useWorldSpace   = true;
//             lineRenderer.textureMode     = LineTextureMode.Tile;
//             lineRenderer.alignment       = LineAlignment.View;
//             lineRenderer.widthMultiplier = thickness;
//
//             runtimeMat = new Material(lineRenderer.sharedMaterial);
//             lineRenderer.material = runtimeMat;
//             baseColor = runtimeMat.HasProperty("_TintColor")
//                 ? runtimeMat.GetColor("_TintColor")
//                 : (runtimeMat.HasProperty("_Color") ? runtimeMat.GetColor("_Color") : Color.white);
//         }
//
//         protected override void OnPlay(AbilityContext ctx)
//         {
//             SetAlpha(1f);
//             StopAllCoroutines();
//             StartCoroutine(ChainRoutine(ctx));
//         }
//
//         private IEnumerator ChainRoutine(AbilityContext ctx)
//         {
//             var player   = ctx.Player;
//             var ability  = ctx.Ability;
//             var currentEnemy  = ctx.InitialTarget;
//
//             var bouncesLeft = bounceAmount;
//             affected.Clear();
//
//             while (currentEnemy != null)
//             {
//                 if (!currentEnemy.gameObject.activeInHierarchy || currentEnemy.CurrentHealth <= 0)
//                     break;
//
//                 // урон за линк
//                 currentEnemy.TakeDamage(ability.Damage);
//                 print(currentEnemy.gameObject.name + "TAKES" + ability.Damage);
//                 affected.Add(currentEnemy);
//
//                 // стартовая точка
//                 startPos = (affected.Count == 1) ? player.transform.position : affected[^2].transform.position;
//                 endPos   = currentEnemy.transform.position;
//
//                 var t = 0f;
//                 while (t < linkDuration)
//                 {
//                     if (chainIsFollowingTarget)
//                     {
//                         startPos = (affected.Count == 1) ? player.transform.position : affected[^2].transform.position;
//                         endPos   = currentEnemy.transform.position;
//                     }
//
//                     RebuildOnce();
//                     ScrollUV();
//
//                     t += Time.deltaTime;
//                     yield return null;
//                 }
//
//                 if (bouncesLeft <= 0) break;
//
//                 
//                 availableEnemies = ctx.EnemiesCollector.AttackableEnemies;
//                 var nextEnemy = FindNextEnemy(currentEnemy, availableEnemies);
//                 availableEnemies.Clear();
//                 
//                 if (!nextEnemy)
//                 {
//                     break;
//                 }
//                 
//                 currentEnemy = nextEnemy;
//                 bouncesLeft--;
//             }
//
//             // общий fade и завершение всей цепи
//             yield return FadeOut();
//             RaiseFinished(ctx.Ability);
//
//             chainIsFollowingTarget = false;
//             gameObject.SetActive(false);
//         }
//
//         private EnemyModel FindNextEnemy(EnemyModel lookingFrom, List<EnemyModel> collectedEnemies)
//         {
//             if (collectedEnemies == null || collectedEnemies.Count == 0)
//             {
//                 UnityEngine.Assertions.Assert.IsNotNull(collectedEnemies, "AttackableEnemies is null");
//                 return null;
//             }
//             
//             EnemyModel closestEnemy = null;
//             var bestSqrMag = maxBounceDistance * maxBounceDistance;
//             foreach (var e in collectedEnemies)
//             {
//                 if (affected.Contains(e)) continue;
//                 if (!e.gameObject.activeInHierarchy || e.CurrentHealth <= 0) continue;
//
//                 var currentSqrMag = (e.transform.position - lookingFrom.transform.position).sqrMagnitude;
//                 if (currentSqrMag > bestSqrMag) continue;
//                 
//                 bestSqrMag = currentSqrMag;
//                 closestEnemy = e;
//             }
//
//             chainIsFollowingTarget = true;
//             return closestEnemy;
//         }
//
//         private IEnumerator FadeOut()
//         {
//             var fade = 0f;
//             while (fade < fadeOut)
//             {
//                 var k = 1f - Mathf.Clamp01(fade / fadeOut);
//                 SetAlpha(k);
//                 fade += Time.deltaTime;
//                 
//                 yield return null;
//             }
//             SetAlpha(0f);
//         }
//
//         private void ScrollUV()
//         {
//             if (runtimeMat && runtimeMat.HasProperty("_MainTex"))
//             {
//                 var off = runtimeMat.mainTextureOffset;
//                 off.x -= uvScrollSpeed * Time.deltaTime;
//                 runtimeMat.mainTextureOffset = off;
//             }
//         }
//
//         private void RebuildOnce()
//         {
//             Vector3 dir = endPos - startPos;
//             float dist = dir.magnitude;
//             if (dist < 0.0001f) { lineRenderer.positionCount = 0; return; }
//
//             dir /= dist;
//             var up = Vector3.up;
//             if (Mathf.Abs(Vector3.Dot(up, dir)) > 0.95f) up = Vector3.right;
//
//             var right = Vector3.Cross(dir, up).normalized;
//             var up2   = Vector3.Cross(right, dir).normalized;
//
//             int count = Mathf.Clamp(Mathf.RoundToInt(dist * pointsPerUnit), minPoints, maxPoints);
//             if (count < 2) count = 2;
//             lineRenderer.positionCount = count;
//
//             float amp = noiseAmplitude + dist * noiseScaleWithDistance;
//
//             for (int i = 0; i < count; i++)
//             {
//                 float t = i / (count - 1f);
//                 var p = Vector3.Lerp(startPos, endPos, t);
//
//                 var env = 1f - Mathf.Abs(t * 2f - 1f);
//                 env = Mathf.Pow(env, 0.6f);
//
//                 var r1 = (Random.value - 0.5f);
//                 var r2 = (Random.value - 0.5f);
//                 var offset = (right * r1 + up2 * r2) * (amp * env);
//
//                 lineRenderer.SetPosition(i, p + offset);
//             }
//
//             // lineRenderer.widthMultiplier = thickness * (0.85f + 0.15f * Mathf.Sin(Time.time * 80f));
//         }
//
//         private void SetAlpha(float a)
//         {
//             if (!runtimeMat) return;
//
//             if (runtimeMat.HasProperty("_TintColor"))
//             {
//                 var c = baseColor; c.a *= a;
//                 runtimeMat.SetColor("_TintColor", c);
//             }
//             else if (runtimeMat.HasProperty("_Color"))
//             {
//                 var c = baseColor; c.a *= a;
//                 runtimeMat.SetColor("_Color", c);
//             }
//         }
//     }
// }