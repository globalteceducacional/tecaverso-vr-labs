using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tecaverso.Labs.PlanarGeometry
{
    public sealed class PlanarGallery : MonoBehaviour
    {
        public PlanarExhibit[] exhibits;
        public Toggle verticesToggle, contoursToggle, dimensionsToggle;
        public Button backButton;
        bool leaving;
        void OnEnable()
        {
            verticesToggle.onValueChanged.AddListener(ShowVertices);
            contoursToggle.onValueChanged.AddListener(ShowContours);
            dimensionsToggle.onValueChanged.AddListener(ShowDimensions);
            backButton.onClick.AddListener(Back);
            ShowVertices(verticesToggle.isOn); ShowContours(contoursToggle.isOn); ShowDimensions(dimensionsToggle.isOn);
        }
        void OnDisable()
        {
            verticesToggle.onValueChanged.RemoveListener(ShowVertices);
            contoursToggle.onValueChanged.RemoveListener(ShowContours);
            dimensionsToggle.onValueChanged.RemoveListener(ShowDimensions);
            backButton.onClick.RemoveListener(Back);
        }
        void ShowVertices(bool value) { foreach (var exhibit in exhibits) exhibit.ShowVertices(value); }
        void ShowContours(bool value) { foreach (var exhibit in exhibits) exhibit.ShowContours(value); }
        void ShowDimensions(bool value) { foreach (var exhibit in exhibits) exhibit.ShowDimensions(value); }
        public void Back()
        {
            if (leaving) return;
            leaving = true; backButton.interactable = false;
            SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");
        }
    }
}
