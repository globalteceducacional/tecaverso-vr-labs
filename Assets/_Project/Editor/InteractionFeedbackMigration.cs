using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Tecaverso.Interaction;
using Tecaverso.Labs.Biology.Cells;

public static class InteractionFeedbackMigration
{
    public const string RendererPath="Assets/_Project/Shared/Rendering/TecaversoVRRenderer.asset";
    public static string Install()
    {
        if(Application.isPlaying || Enumerable.Range(0,SceneManager.sceneCount).Any(i=>SceneManager.GetSceneAt(i).isDirty))throw new InvalidOperationException("Edit Mode e cenas sem alterações pendentes são necessários.");
        var pipeline=AssetDatabase.LoadAssetAtPath<UniversalRenderPipelineAsset>("Assets/_Project/Shared/Rendering/TecaversoVR.asset");
        var settings=new SerializedObject(pipeline);var slot=settings.FindProperty("m_RendererDataList").GetArrayElementAtIndex(0);
        if(!AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath))
        {
            string source=AssetDatabase.GetAssetPath(slot.objectReferenceValue);
            if(!AssetDatabase.CopyAsset(source,RendererPath))throw new InvalidOperationException("Não foi possível derivar o renderer atual.");
        }
        var renderer=AssetDatabase.LoadAssetAtPath<UniversalRendererData>(RendererPath);
        var feature=renderer.rendererFeatures.OfType<InteractionSilhouetteFeature>().SingleOrDefault();
        if(!feature){feature=ScriptableObject.CreateInstance<InteractionSilhouetteFeature>();feature.name="Tecaverso interaction silhouette";AssetDatabase.AddObjectToAsset(feature,renderer);renderer.rendererFeatures.Add(feature);}
        feature.silhouetteShader=AssetDatabase.LoadAssetAtPath<Shader>("Assets/_Project/Shared/Interaction/InteractionSilhouette.shader");
        if(!feature.silhouetteShader)throw new InvalidOperationException("Shader não importado.");
        feature.SetActive(true);feature.Create();EditorUtility.SetDirty(feature);renderer.SetDirty();EditorUtility.SetDirty(renderer);
        slot.objectReferenceValue=renderer;settings.ApplyModifiedPropertiesWithoutUndo();EditorUtility.SetDirty(pipeline);
        // Existing scenes inherit the implementation through their serialized component GUID and runtime bootstrap.
        var current=SceneManager.GetActiveScene();bool changed=false;
        foreach(var p in UnityEngine.Object.FindObjectsByType<CellStructure>(FindObjectsInactive.Include,FindObjectsSortMode.None))
            if(p.selectionOutline && p.selectionOutline.enabled){p.selectionOutline.enabled=false;changed=true;}
        if(changed){EditorSceneManager.MarkSceneDirty(current);EditorSceneManager.SaveScene(current);}
        AssetDatabase.SaveAssets();
        return "Renderer compartilhado instalado sem alterar MSAA, materiais, colliders ou transforms; cenas existentes e criação runtime cobertas pelo bootstrap.";
    }
}
