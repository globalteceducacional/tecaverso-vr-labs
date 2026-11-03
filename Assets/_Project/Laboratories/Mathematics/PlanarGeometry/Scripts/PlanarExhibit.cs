using UnityEngine;
using Tecaverso.UI;

namespace Tecaverso.Labs.PlanarGeometry
{
    public sealed class PlanarExhibit : MonoBehaviour
    {
        public GameObject vertexMarkers, contourGeometry, dimensionGeometry;
        public CanvasGroup perimeterLabel, dimensionLabels;
        public bool circle;
        public int vertexCount;
        public void ShowVertices(bool value) => vertexMarkers.SetActive(value && !circle);
        public void ShowContours(bool value)
        { contourGeometry.SetActive(value); UIVisibility.Set(perimeterLabel.gameObject, value); }
        public void ShowDimensions(bool value)
        { dimensionGeometry.SetActive(value); UIVisibility.Set(dimensionLabels.gameObject, value); }
    }
}
