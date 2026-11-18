using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tecaverso.Editor
{
    public static class BiologyRoomAuthoring
    {
        const string Template = "Assets/_Project/Laboratories/Biology/Cells/Scenes/Cells.unity";

        public static Scene Begin(string scenePath)
        {
            if (Application.isPlaying || File.Exists(scenePath))
                throw new InvalidOperationException("Crie apenas uma sala nova em Edit Mode.");
            CellsAssets.Ensure(Path.GetDirectoryName(scenePath).Replace('\\', '/'));
            var source = SceneManager.GetSceneByPath(Template);
            bool opened = !source.isLoaded;
            if (opened) source = EditorSceneManager.OpenScene(Template, OpenSceneMode.Additive);
            // Copy the whole scene first so serialized links between XR roots survive the copy.
            try
            {
                if (source.isDirty) throw new InvalidOperationException("A cena de origem tem alterações pendentes.");
                if (!EditorSceneManager.SaveScene(source, scenePath, true))
                    throw new IOException("Falha ao copiar a sala de referência.");
            }
            finally { if (opened) EditorSceneManager.CloseScene(source, true); }
            var room = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
            SceneManager.SetActiveScene(room);
            string[] keep = { "XR Origin (XR Rig)", "XR Interaction Manager", "EventSystem",
                "Directional Light", "TecaversoStandardRoom", "Shared learning rendering" };
            foreach (var root in room.GetRootGameObjects())
            {
                if (!keep.Contains(root.name)) UnityEngine.Object.DestroyImmediate(root);
                else if (root.name == keep[0])
                    root.transform.SetPositionAndRotation(new Vector3(0, 0, -3), Quaternion.identity);
            }
            return room;
        }
    }
}
