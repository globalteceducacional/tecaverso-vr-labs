using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tecaverso.Labs.Shapes
{
    public sealed class SolidGallery : MonoBehaviour
    {
        public SolidExhibit[] exhibits;
        public Toggle verticesToggle, edgesToggle, rotationToggle;
        public Button backButton;
        bool leaving;

        void OnEnable()
        {
            verticesToggle.onValueChanged.AddListener(ShowVertices);
            edgesToggle.onValueChanged.AddListener(ShowEdges);
            rotationToggle.onValueChanged.AddListener(Rotate);
            backButton.onClick.AddListener(Back);
            ShowVertices(verticesToggle.isOn);
            ShowEdges(edgesToggle.isOn);
            Rotate(rotationToggle.isOn);
        }
        void OnDisable()
        {
            verticesToggle.onValueChanged.RemoveListener(ShowVertices);
            edgesToggle.onValueChanged.RemoveListener(ShowEdges);
            rotationToggle.onValueChanged.RemoveListener(Rotate);
            backButton.onClick.RemoveListener(Back);
        }
        void ShowVertices(bool value) { foreach (var exhibit in exhibits) exhibit.ShowVertices(value); }
        void ShowEdges(bool value) { foreach (var exhibit in exhibits) exhibit.ShowEdges(value); }
        void Rotate(bool value) { foreach (var exhibit in exhibits) exhibit.rotating = value; }
        public void Back()
        {
            if (leaving) return;
            leaving = true;
            backButton.interactable = false;
            SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");
        }
    }
}
