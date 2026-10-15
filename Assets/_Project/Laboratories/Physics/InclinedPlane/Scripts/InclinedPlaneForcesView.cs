using System.Globalization;
using Tecaverso.Labs.ObliqueLaunch;
using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.InclinedPlane
{
    public sealed class InclinedPlaneForcesView : MonoBehaviour
    {
        [SerializeField] VectorArrowView[] arrows;
        [SerializeField] TMP_Text[] labels;
        [SerializeField] float unitsPerNewton = .035f;
        InclineParameters previous;
        bool initialized, previousVisible, previousResultant, previousComponents;
        static readonly CultureInfo Portuguese = CultureInfo.GetCultureInfo("pt-BR");
        static readonly string[] Names = { "P", "N", "Fat", "ΣF", "Ppar", "Pperp" };
        static readonly Vector3[] LabelOffsets = {
            new Vector3(-.65f,-.9f,0), new Vector3(.65f,.9f,0), new Vector3(-1.25f,.45f,0),
            new Vector3(1.25f,-.35f,0), new Vector3(1.25f,-.7f,0), new Vector3(-.65f,-.9f,0)
        };
        public void Bind(VectorArrowView[] vectorViews, TMP_Text[] valueLabels) { arrows=vectorViews; labels=valueLabels; }
        public float Scale(InclineParameters p) => Mathf.Min(unitsPerNewton, 1.2f/(p.Mass*p.Gravity));
        public void Show(Vector3 blockPosition, InclineParameters p, InclineForces forces, bool visible, bool resultant, bool components)
        {
            // Keep the diagram in front of the ramp's front rail, including the cone radius.
            transform.position = blockPosition + Vector3.back * .86f;
            // Forces are constant during each descent. Only the anchor moves every frame.
            if(initialized && previous.Angle==p.Angle && previous.Mass==p.Mass && previous.Gravity==p.Gravity &&
                previous.StaticFriction==p.StaticFriction && previous.KineticFriction==p.KineticFriction &&
                previousVisible==visible && previousResultant==resultant && previousComponents==components) return;
            initialized=true;previous=p;previousVisible=visible;previousResultant=resultant;previousComponents=components;
            var vectors = new[] { forces.Weight, forces.Normal, forces.Friction, forces.Resultant, forces.ParallelWeight, forces.PerpendicularWeight };
            for(int i=0;i<arrows.Length;i++)
            {
                bool show = visible && (i==0 ? !components : i==3 ? resultant : i>=4 ? components : true);
                arrows[i].gameObject.SetActive(show);
                labels[i].gameObject.SetActive(show);
                if(!show) continue;
                arrows[i].SetScaledVector(vectors[i], Scale(p));
                labels[i].text = Names[i]+" = "+vectors[i].magnitude.ToString("0.0", Portuguese)+" N";
                labels[i].transform.localPosition = LabelOffsets[i];
            }
        }
    }
}
