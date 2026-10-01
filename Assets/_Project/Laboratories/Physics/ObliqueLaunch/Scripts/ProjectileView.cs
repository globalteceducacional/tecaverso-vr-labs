using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class ProjectileView : MonoBehaviour
    {
        [SerializeField] VectorArrowView resultArrow, horizontalArrow, verticalArrow;
        public void Bind(VectorArrowView result, VectorArrowView horizontal, VectorArrowView vertical)
        { resultArrow = result; horizontalArrow = horizontal; verticalArrow = vertical; }
        public void Show(in FlightSample sample)
        {
            transform.position = sample.Position;
            resultArrow.SetVector(sample.Velocity);
            horizontalArrow.SetVector(new Vector3(sample.Velocity.x, 0f, 0f));
            verticalArrow.SetVector(new Vector3(0f, sample.Velocity.y, 0f));
        }
    }
}
