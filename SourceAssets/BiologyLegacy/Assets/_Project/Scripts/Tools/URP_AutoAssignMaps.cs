// Assets/Editor/URP_AutoAssignMaps.cs
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public static class URP_AutoAssignMaps
{
    // Ajuste aqui seus sufixos:
    private const string SUF_DIFFUSE = "_Diffuse";
    private const string SUF_NORMAL = "_Normal";
    private const string SUF_SPECULAR = "_Specular";
    private const string SUF_GLOSSINESS = "_Glossiness";   // opcional (ver nota)
    private const string SUF_DISPLACEMENT = "_Displacement"; // ignorado no URP/Lit padrão

    // Extensões que vamos procurar
    private static readonly string[] exts = { ".png", ".tga", ".tif", ".tiff", ".jpg", ".jpeg", ".psd", ".exr", ".hdr" };

    [MenuItem("Tools/URP/Auto-atribuir mapas (materiais selecionados)")]
    private static void AutoAssign_SelectedMaterials()
    {
        var mats = Selection.objects
            .OfType<Material>()
            .Distinct()
            .ToArray();

        if (mats.Length == 0)
        {
            EditorUtility.DisplayDialog("URP Auto-Assign", "Selecione um ou mais materiais no Project.", "Ok");
            return;
        }

        Undo.RecordObjects(mats, "URP Auto-assign maps");
        int count = 0;

        foreach (var mat in mats)
        {
            if (TryAssignForMaterial(mat))
                count++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[URP Auto-Assign] Materiais processados: {count}/{mats.Length}");
    }

    [MenuItem("Tools/URP/Auto-atribuir mapas em pasta…")]
    private static void AutoAssign_InFolder()
    {
        var folder = EditorUtility.OpenFolderPanel("Selecione a pasta com materiais", "Assets", "");
        if (string.IsNullOrEmpty(folder)) return;

        // Converter caminho absoluto para relativo (Assets/…)
        var projectPath = Application.dataPath.Replace("/Assets", "");
        if (!folder.StartsWith(projectPath))
        {
            EditorUtility.DisplayDialog("URP Auto-Assign", "Escolha uma pasta dentro de Assets.", "Ok");
            return;
        }
        string rel = "Assets" + folder.Substring(projectPath.Length);

        var guids = AssetDatabase.FindAssets("t:material", new[] { rel });
        var mats = guids
            .Select(g => AssetDatabase.GUIDToAssetPath(g))
            .Select(p => AssetDatabase.LoadAssetAtPath<Material>(p))
            .Where(m => m != null)
            .ToArray();

        if (mats.Length == 0)
        {
            EditorUtility.DisplayDialog("URP Auto-Assign", "Nenhum material encontrado na pasta.", "Ok");
            return;
        }

        Undo.RecordObjects(mats, "URP Auto-assign maps (folder)");
        int count = 0;
        foreach (var mat in mats)
        {
            if (TryAssignForMaterial(mat))
                count++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"[URP Auto-Assign] Materiais processados na pasta: {count}/{mats.Length}");
    }

    private static bool TryAssignForMaterial(Material mat)
    {
        if (mat == null) return false;

        // Garantir URP/Lit e Workflow Specular
        if (mat.shader.name != "Universal Render Pipeline/Lit")
        {
            var urpLit = Shader.Find("Universal Render Pipeline/Lit");
            if (urpLit == null)
            {
                Debug.LogWarning($"[URP Auto-Assign] Shader URP/Lit não encontrado para {mat.name}");
                return false;
            }
            mat.shader = urpLit;
        }

        // _WorkflowMode: 0 = Metallic, 1 = Specular
        if (mat.HasProperty("_WorkflowMode"))
            mat.SetFloat("_WorkflowMode", 1f);

        string baseName = mat.name; // ex.: "Adrenal_Gland_Left"

        // Procura texturas pelo padrão "<MaterialName><SUFIXO>.<ext>"
        Texture2D albedo = FindTextureExact(baseName + SUF_DIFFUSE);
        Texture2D normal = FindTextureExact(baseName + SUF_NORMAL);
        Texture2D specular = FindTextureExact(baseName + SUF_SPECULAR);
        Texture2D glossiness = FindTextureExact(baseName + SUF_GLOSSINESS);
        // Texture2D height  = FindTextureExact(baseName + SUF_DISPLACEMENT); // não usado aqui

        bool changed = false;

        // BaseMap (Albedo)
        if (albedo && mat.HasProperty("_BaseMap"))
        {
            mat.SetTexture("_BaseMap", albedo);
            changed = true;
        }

        // Normal Map
        if (normal && mat.HasProperty("_BumpMap"))
        {
            mat.SetTexture("_BumpMap", normal);
            // força keyword de normal
            mat.EnableKeyword("_NORMALMAP");
            changed = true;
        }

        // Specular (URP espera _SpecGlossMap, com Smoothness no canal A)
        if (specular && mat.HasProperty("_SpecGlossMap"))
        {
            if (glossiness != null && NeedsPackedSpecGloss(specular))
            {
                // Caso você tenha Glossiness separado, sugerimos empacotar fora.
                // Aqui apenas avisamos sem empacotar para evitar gerar arquivos.
                Debug.LogWarning($"[URP Auto-Assign] {baseName}: Encontrado Glossiness separado. O URP/Lit (Specular) espera Smoothness no alpha do Specular. Considere empacotar Glossiness como alpha do Specular.");
            }

            mat.SetTexture("_SpecGlossMap", specular);
            mat.EnableKeyword("_SPECGLOSSMAP");
            changed = true;
        }

        // Opcional: ajuste de smoothness global (ganho)
        if (mat.HasProperty("_Smoothness") && glossiness == null)
        {
            // valor padrão razoável para tecidos úmidos
            if (mat.GetFloat("_Smoothness") < 0.4f)
                mat.SetFloat("_Smoothness", 0.55f);
        }

        if (changed)
            EditorUtility.SetDirty(mat);

        return changed;
    }

    /// <summary>
    /// Procura textura por nome EXATO (sem sufixos de import type) em todo o projeto.
    /// Aceita várias extensões comuns.
    /// </summary>
    private static Texture2D FindTextureExact(string fileNameNoExt)
    {
        // Busca por nome exato via GUIDs (FindAssets não busca por extensão específica com exatidão),
        // então filtramos por caminho e por terminações.
        var guids = AssetDatabase.FindAssets("t:texture2D " + Path.GetFileNameWithoutExtension(fileNameNoExt));
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            string nameNoExt = Path.GetFileNameWithoutExtension(path);

            if (string.Equals(nameNoExt, fileNameNoExt, System.StringComparison.OrdinalIgnoreCase) &&
                exts.Any(e => path.EndsWith(e, System.StringComparison.OrdinalIgnoreCase)))
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                if (tex) return tex;
            }
        }
        return null;
    }

    // Heurística: se precisar empacotar glossiness em alpha do spec
    private static bool NeedsPackedSpecGloss(Texture2D spec)
    {
        // Não temos como inspecionar os canais aqui com segurança.
        // Retorna true para lembrar o usuário quando existe um Glossiness separado.
        return true;
    }
}
