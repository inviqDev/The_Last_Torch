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