using System.Collections;
using UnityEngine;

namespace Runtime
{
    [RequireComponent(typeof(LineRenderer))]
    public class LightningBolt : AbilityVFX
    {
        [Header("Shape")] 
        [SerializeField] private float pointsPerUnit = 3.0f; // чем больше, тем детальнее на дальних дистанциях

        [SerializeField] private int minPoints = 8;
        [SerializeField] private int maxPoints = 64;
        [SerializeField] private float noiseAmplitude = 0.2f; // базовая «рваность» в метрах
        [SerializeField] private float noiseScaleWithDistance = 0.15f; // как шум растёт с длиной
        [SerializeField] private float thickness = 0.07f;

        [Header("Lifetime")] 
        [SerializeField] private float duration = 0.06f; // сколько «живёт» болт
        [SerializeField] private float fadeOut = 0.06f; // мягкое затухание после
        [SerializeField] private bool followTargetsWhileAlive = false;
 
        [Header("UV Scroll (optional)")] [SerializeField]
        private float uvScrollSpeed = 6f; // прокрутка текстуры по линии

        private LineRenderer lr;
        private Transform startT;
        private Transform endT;
        private Vector3 startPos;
        private Vector3 endPos;
        private Material runtimeMat; // инстанс материала для безопасного изменения цвета/офсета
        private Color baseColor;

        void Awake()
        {
            lr = GetComponent<LineRenderer>();
            lr.useWorldSpace = true;
            lr.textureMode = LineTextureMode.Tile; // лучше тайлить молниевую текстуру
            lr.alignment = LineAlignment.View;
            lr.widthMultiplier = thickness;

            // Делаем экземпляр материала, чтобы не править shared
            runtimeMat = new Material(lr.sharedMaterial);
            lr.material = runtimeMat;
            baseColor = runtimeMat.HasProperty("_TintColor")
                ? runtimeMat.GetColor("_TintColor")
                : (runtimeMat.HasProperty("_Color") ? runtimeMat.GetColor("_Color") : Color.white);
        }

        public override void UseAbility(Transform playerTransform, Transform enemyTransform)
        {
            print("override");
            startT = playerTransform;
            endT = enemyTransform;
            
            StopAllCoroutines();
            StartCoroutine(Run());
        }
        
        /// <summary>Запуск болта между двумя трансформами.</summary>
        public void Fire(Transform start, Transform end, float? overrideDuration = null)
        {
            startT = start;
            endT = end;
            duration = overrideDuration ?? duration;
            StopAllCoroutines();
            StartCoroutine(Run());
        }

        /// <summary>Запуск болта между двумя точками (без слежения)</summary>
        public void Fire(Vector3 start, Vector3 end, float? overrideDuration = null)
        {
            startT = null;
            endT = null;
            startPos = start;
            endPos = end;
            duration = overrideDuration ?? duration;
            StopAllCoroutines();
            StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            float t = 0f;

            while (t < duration)
            {
                // Обновляем старт/энд
                if (followTargetsWhileAlive && startT != null) startPos = startT.position;
                else if (startT != null && t == 0f) startPos = startT.position;

                if (followTargetsWhileAlive && endT != null) endPos = endT.position;
                else if (endT != null && t == 0f) endPos = endT.position;

                RebuildOnce();
                // Прокрутка UV
                if (runtimeMat.HasProperty("_MainTex"))
                {
                    var off = runtimeMat.mainTextureOffset;
                    off.x -= uvScrollSpeed * Time.deltaTime;
                    runtimeMat.mainTextureOffset = off;
                }

                t += Time.deltaTime;
                yield return null;
            }

            // Плавный фейд
            float fade = 0f;
            while (fade < fadeOut)
            {
                float k = 1f - Mathf.Clamp01(fade / fadeOut);
                SetAlpha(k);
                fade += Time.deltaTime;
                yield return null;
            }

            // Возврат в пул или выключение
            SetAlpha(0f);
            gameObject.SetActive(false);
        }

        private void RebuildOnce()
        {
            Vector3 dir = endPos - startPos;
            float dist = dir.magnitude;
            if (dist < 0.0001f)
            {
                lr.positionCount = 0;
                return;
            }

            dir /= dist;
            // Перпендикуляр для шума (лучше два для 3D; возьмём ортобазис)
            var up = Vector3.up;
            if (Mathf.Abs(Vector3.Dot(up, dir)) > 0.95f) up = Vector3.right;
            var right = Vector3.Cross(dir, up).normalized;
            var up2 = Vector3.Cross(right, dir).normalized;

            var count = Mathf.Clamp(Mathf.RoundToInt(dist * pointsPerUnit), minPoints, maxPoints);
            if (count < 2) count = 2;
            lr.positionCount = count;

            var amp = noiseAmplitude + dist * noiseScaleWithDistance;

            for (int i = 0; i < count; i++)
            {
                var t = i / (count - 1f);
                var p = Vector3.Lerp(startPos, endPos, t);

                // «Бубнеобразный» профиль шума (меньше у концов)
                var envelope = 1f - Mathf.Abs(t * 2f - 1f); // максимум посередине
                envelope = Mathf.Pow(envelope, 0.6f);

                // 2D смещение в плоскости, перпендикулярной лучу
                var r1 = (Random.value - 0.5f);
                var r2 = (Random.value - 0.5f);
                var offset = (right * r1 + up2 * r2) * (amp * envelope);

                lr.SetPosition(i, p + offset);
            }

            // ширина тоньше к концам
            lr.widthMultiplier = thickness * (0.85f + 0.15f * Mathf.Sin(Time.time * 80f));
        }

        private void SetAlpha(float a)
        {
            if (runtimeMat.HasProperty("_TintColor"))
            {
                var c = baseColor;
                c.a *= a;
                runtimeMat.SetColor("_TintColor", c);
            }
            else if (runtimeMat.HasProperty("_Color"))
            {
                var c = baseColor;
                c.a *= a;
                runtimeMat.SetColor("_Color", c);
            }
        }
    }
}