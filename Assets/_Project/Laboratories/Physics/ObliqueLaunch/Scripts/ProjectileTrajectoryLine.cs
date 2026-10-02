using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class ProjectileTrajectoryLine : MonoBehaviour
    {
        sealed class LaunchRecord
        {
            public LineRenderer Line;
            public Material LineMaterial;
            public GameObject PeakMarker;
            public Material PeakMaterial;
            public int FadeSteps;
        }

        const int MaximumRecords = 5;
        const float FadePerLaunch = .15f;

        [SerializeField] LineRenderer lineTemplate;
        [SerializeField] Material projectileMaterial;
        [SerializeField, Min(.01f)] float minimumPointDistance = .05f;

        readonly List<LaunchRecord> records = new();
        LaunchRecord current;
        Vector3 currentOrigin, lastPoint;
        LaunchParameters currentParameters;
        float peakTime;
        bool hasPoint, peakCreated;
        int launchSequence;

        public int RecordCount => records.Count;

        public void Bind(LineRenderer template, Material projectileAppearance)
        {
            lineTemplate = template;
            projectileMaterial = projectileAppearance;
        }

        void Awake()
        {
            lineTemplate.positionCount = 0;
            lineTemplate.gameObject.SetActive(false);
        }

        public void Begin(Vector3 initialPosition, in LaunchParameters parameters)
        {
            FadePreviousRecords();
            if (records.Count >= MaximumRecords) RemoveRecord(records[0]);

            currentOrigin = initialPosition;
            currentParameters = parameters;
            peakTime = Mathf.Max(0f, parameters.InitialVelocity.y / parameters.Gravity);
            peakCreated = false;
            hasPoint = false;

            var instance = Instantiate(lineTemplate, transform);
            instance.gameObject.name = $"Launch {++launchSequence:00} Trajectory";
            instance.gameObject.SetActive(true);
            instance.positionCount = 0;
            instance.startColor = instance.endColor = Opaque(instance.startColor);
            instance.material = CreateTransparentMaterial(lineTemplate.sharedMaterial);

            current = new LaunchRecord { Line = instance, LineMaterial = instance.material };
            records.Add(current);
            AddPoint(initialPosition);
        }

        public void Plot(in FlightSample sample)
        {
            if (current == null || hasPoint && sample.Position.x < lastPoint.x - .001f) return;
            if (!hasPoint || Vector3.Distance(lastPoint, sample.Position) >= minimumPointDistance || sample.Position.y <= 0f)
                AddPoint(sample.Position);
            if (!peakCreated && sample.Time + .001f >= peakTime) CreatePeakMarker();
        }

        public void Clear()
        {
            while (records.Count > 0) RemoveRecord(records[0]);
            current = null;
            hasPoint = false;
            peakCreated = false;
            launchSequence = 0;
            if (lineTemplate != null) lineTemplate.positionCount = 0;
        }

        void FadePreviousRecords()
        {
            foreach (var record in records)
            {
                record.FadeSteps++;
                float opacity = Mathf.Clamp01(1f - record.FadeSteps * FadePerLaunch);
                var lineColor = record.Line.startColor;
                lineColor.a = opacity;
                record.Line.startColor = record.Line.endColor = lineColor;
                SetMaterialOpacity(record.LineMaterial, opacity);
                SetMaterialOpacity(record.PeakMaterial, opacity);
            }
        }

        void CreatePeakMarker()
        {
            var v0 = currentParameters.InitialVelocity;
            var peakPosition = currentOrigin + new Vector3(v0.x * peakTime, v0.y * peakTime - .5f * currentParameters.Gravity * peakTime * peakTime, 0f);
            var marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = $"Launch {launchSequence:00} Peak Projectile";
            marker.transform.SetParent(transform);
            marker.transform.position = peakPosition;
            marker.transform.localScale = Vector3.one * .35f;
            var markerCollider = marker.GetComponent<Collider>();
            if (markerCollider != null) Destroy(markerCollider);
            var renderer = marker.GetComponent<Renderer>();
            renderer.material = CreateTransparentMaterial(projectileMaterial);
            current.PeakMarker = marker;
            current.PeakMaterial = renderer.material;
            peakCreated = true;
        }

        void AddPoint(Vector3 position)
        {
            current.Line.positionCount++;
            current.Line.SetPosition(current.Line.positionCount - 1, position);
            lastPoint = position;
            hasPoint = true;
        }

        void RemoveRecord(LaunchRecord record)
        {
            records.Remove(record);
            if (record.LineMaterial != null) Destroy(record.LineMaterial);
            if (record.PeakMaterial != null) Destroy(record.PeakMaterial);
            if (record.Line != null) Destroy(record.Line.gameObject);
            if (record.PeakMarker != null) Destroy(record.PeakMarker);
        }

        static Material CreateTransparentMaterial(Material source)
        {
            var material = new Material(source);
            material.SetFloat("_Surface", 1f);
            material.SetFloat("_SrcBlend", (float)BlendMode.SrcAlpha);
            material.SetFloat("_DstBlend", (float)BlendMode.OneMinusSrcAlpha);
            material.SetFloat("_ZWrite", 0f);
            material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            material.renderQueue = (int)RenderQueue.Transparent;
            SetMaterialOpacity(material, 1f);
            return material;
        }

        static void SetMaterialOpacity(Material material, float opacity)
        {
            if (material == null) return;
            var color = material.color;
            color.a = opacity;
            material.color = color;
        }

        static Color Opaque(Color color) { color.a = 1f; return color; }
    }
}
