using Tecaverso.UI.Spectator;
using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Tecaverso.EditorTools
{
    /// <summary>Creates new spectator assets only; never rebuilds the authored Hub or lab.</summary>
    public static class SpectatorWaitingAuthoring
    {
        const string Folder = "Assets/_Project/UI/Spectator";
        const string Prefab = Folder + "/SpectatorWaiting.prefab";
        public const string Preview = Folder + "/SpectatorWaitingPreview.unity";
        static readonly Color Brand = new Color32(40, 78, 160, 255);
        static TMP_FontAsset font;

        [MenuItem("Tecaverso/Spectator/Create waiting assets (once)")]
        public static void Create()
        {
            if (EditorApplication.isPlaying) throw new System.InvalidOperationException("Exit Play Mode first.");
            if (AssetDatabase.LoadAssetAtPath<GameObject>(Prefab) != null)
                throw new System.InvalidOperationException("Assets already exist. Edit the prefab instead of regenerating it.");
            font = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>("Assets/_Project/UI/Fonts/Montserrat/Montserrat-Regular SDF.asset");
            var previous = SceneManager.GetActiveScene();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            try
            {
                SceneManager.SetActiveScene(scene);
                var root = Rect("Spectator Waiting", null);
                var canvas = root.gameObject.AddComponent<Canvas>(); canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                var scaler = root.gameObject.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080); scaler.matchWidthOrHeight = 1;
                root.gameObject.AddComponent<GraphicRaycaster>();
                var page = Rect("Waiting page", root); Stretch(page);
                page.gameObject.AddComponent<CanvasGroup>();
                page.gameObject.AddComponent<Image>().color = new Color32(2, 23, 50, 255);
                var center = Rect("Placeholder", page); Stretch(center);
                var title = Text("Tecaverso Labs", center, 64, Color.white); Stretch(title.rectTransform);
                title.alignment = TextAlignmentOptions.Center;
                var surface = Rect("Recorded preview", page).gameObject.AddComponent<RawImage>();
                surface.raycastTarget = false; surface.enabled = false;
                var aspect = surface.gameObject.AddComponent<AspectRatioFitter>();
                aspect.aspectMode = AspectRatioFitter.AspectMode.FitInParent; aspect.aspectRatio = 16f / 9f;
                var video = root.gameObject.AddComponent<VideoPlayer>();
                video.playOnAwake = false; video.isLooping = true; video.audioOutputMode = VideoAudioOutputMode.None;
                video.renderMode = VideoRenderMode.APIOnly;

                var label = Text("ESPAÇO PARA PRÉVIA GRAVADA · Vídeo ainda não configurado", page, 24, Color.white);
                label.rectTransform.anchorMin = new Vector2(.04f, .89f); label.rectTransform.anchorMax = new Vector2(.96f, .96f);
                label.rectTransform.offsetMin = label.rectTransform.offsetMax = Vector2.zero;
                var card = Rect("Session notice", page);
                card.anchorMin = new Vector2(.52f, .04f); card.anchorMax = new Vector2(.96f, .40f);
                card.offsetMin = card.offsetMax = Vector2.zero;
                card.gameObject.AddComponent<Image>().color = new Color32(232, 230, 254, 255);
                var layout = card.gameObject.AddComponent<VerticalLayoutGroup>();
                layout.padding = new RectOffset(32, 32, 24, 24); layout.spacing = 16;
                layout.childControlWidth = layout.childControlHeight = true;
                layout.childForceExpandWidth = true; layout.childForceExpandHeight = false;
                var heading = Text("Nenhuma sala conectada", card, 30, Brand); Height(heading.gameObject, 48);
                var body = Text("Conheça o Tecaverso enquanto aguarda. A busca automática de salas ainda não está disponível.", card, 24, new Color32(22,22,22,255));
                Height(body.gameObject, 112);
                var actions = Rect("Actions", card); Height(actions.gameObject, 64);
                var row = actions.gameObject.AddComponent<HorizontalLayoutGroup>(); row.spacing = 16;
                row.childControlHeight = row.childControlWidth = true; row.childForceExpandWidth = true;
                var connect = Button("Conectar", actions); var watch = Button("Assistir", actions); var explore = Button("Explorar sozinho", actions);
                // No live client exists yet: capabilities are supplied by its future adapter.
                foreach (var b in new[] { connect, watch, explore }) Tecaverso.UI.UIVisibility.Set(b.gameObject, false, true);
                var view = root.gameObject.AddComponent<SpectatorWaitingView>();
                view.Bind(page.gameObject, heading, body, label, connect, watch, explore, video, surface, aspect);
                PrefabUtility.SaveAsPrefabAsset(root.gameObject, Prefab);
                Object.DestroyImmediate(root.gameObject);
                PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Prefab), scene);
                var camera = new GameObject("Preview Camera", typeof(Camera));
                camera.GetComponent<Camera>().clearFlags = CameraClearFlags.SolidColor;
                camera.GetComponent<Camera>().backgroundColor = new Color32(2,23,50,255);
                new GameObject("Directional Light", typeof(Light)).GetComponent<Light>().type = LightType.Directional;
                new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
                EditorSceneManager.SaveScene(scene, Preview);
            }
            finally
            {
                EditorSceneManager.CloseScene(scene, true);
                SceneManager.SetActiveScene(previous);
            }
            Debug.Log("Spectator waiting prefab and isolated preview created. Build scenes and VR scenes unchanged.");
        }
        static RectTransform Rect(string name, Transform parent)
        {
            var r = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            if (parent != null) r.SetParent(parent, false);
            return r;
        }
        static void Stretch(RectTransform r) { r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one; r.offsetMin = r.offsetMax = Vector2.zero; }
        static void Height(GameObject go, float height) { var e = go.AddComponent<LayoutElement>(); e.minHeight = e.preferredHeight = height; }
        static TMP_Text Text(string value, Transform parent, float size, Color color)
        {
            var t = Rect(value, parent).gameObject.AddComponent<TextMeshProUGUI>();
            t.font = font; t.text = value; t.fontSize = size; t.color = color; t.raycastTarget = false;
            t.textWrappingMode = TextWrappingModes.Normal; t.overflowMode = TextOverflowModes.Ellipsis;
            return t;
        }
        static Button Button(string value, Transform parent)
        {
            var r = Rect(value, parent); var image = r.gameObject.AddComponent<Image>(); image.color = Brand;
            var button = r.gameObject.AddComponent<Button>(); button.targetGraphic = image;
            var le = r.gameObject.AddComponent<LayoutElement>(); le.flexibleWidth = 1; le.minWidth = 150;
            var text = Text(value, r, 22, Color.white); Stretch(text.rectTransform); text.alignment = TextAlignmentOptions.Center;
            r.gameObject.AddComponent<Tecaverso.Labs.ObliqueLaunch.ButtonTweenFeedback>();
            return button;
        }
    }
}
