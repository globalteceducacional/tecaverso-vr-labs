using Tecaverso.Interaction;
using UnityEngine;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class FlexibleCable : MonoBehaviour
    {
        public CableEnd first, second;
        public LineRenderer sheath, flow;
        public float current;
        readonly Vector3[] curve = new Vector3[20];
        MaterialPropertyBlock properties;
        float phase;
        static readonly int Phase = Shader.PropertyToID("_Phase");
        static readonly int Direction = Shader.PropertyToID("_Direction");

        void Awake()
        {
            properties = new MaterialPropertyBlock();
            sheath.positionCount = flow.positionCount = curve.Length;
            flow.textureMode = LineTextureMode.Tile;
            flow.widthMultiplier = sheath.widthMultiplier * .83f;
        }

        void LateUpdate()
        {
            if (!first || !second) return;
            var a = first.transform.position;
            var b = second.transform.position;
            float slack = Mathf.Max(.12f, Vector3.Distance(a, b) * .16f);
            for (int i = 0; i < curve.Length; i++)
            {
                float t = i / (float)(curve.Length - 1);
                curve[i] = Vector3.Lerp(a, b, t) + Vector3.down * (Mathf.Sin(t * Mathf.PI) * slack);
            }
            sheath.SetPositions(curve);
            flow.enabled = Mathf.Abs(current) > 1e-4f;
            if (!flow.enabled) return;
            flow.SetPositions(curve);
            if (!MotionPreferences.Reduced)
                phase = Mathf.Repeat(phase + Time.deltaTime * Mathf.Clamp(Mathf.Abs(current), .25f, 1.5f), 1f);
            properties.SetFloat(Phase, phase);
            properties.SetFloat(Direction, Mathf.Sign(current));
            flow.SetPropertyBlock(properties);
        }
    }
}
