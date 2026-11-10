using TMPro;
using UnityEngine;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    public sealed class BondDisplay : MonoBehaviour
    {
        public Material[] atomMaterials;
        public Material bondMaterial, electronMaterial;
        public TMP_FontAsset font;
        public Transform model;
        BondRecipe current;
        bool electrons;
        public void Show(BondRecipe recipe, bool showElectrons)
        {
            current = recipe; electrons = showElectrons;
            for (int i = model.childCount - 1; i >= 0; i--) Destroy(model.GetChild(i).gameObject);
            if (recipe == null) return;
            if (recipe.ionic) DrawLattice(); else DrawMolecule(recipe);
        }
        void DrawMolecule(BondRecipe recipe)
        {
            for (int i = 0; i < recipe.atoms.Length; i++) Atom(recipe.atoms[i], recipe.positions[i] * .82f, .23f);
            for (int i = 0; i < recipe.bondOrders.Length; i++)
            {
                var a = recipe.positions[recipe.bondPairs[i * 2]] * .82f;
                var b = recipe.positions[recipe.bondPairs[i * 2 + 1]] * .82f;
                int order = recipe.bondOrders[i];
                for (int line = 0; line < order; line++)
                {
                    var offset = Vector3.forward * ((line - (order - 1) * .5f) * .12f);
                    Link(a + offset, b + offset, .035f, bondMaterial);
                    if (electrons) Dot((a + b) * .5f + offset + Vector3.up * .06f);
                }
            }
            if (electrons)
            {
                for (int i = 0; i < recipe.atoms.Length; i++)
                {
                    int free = recipe.atoms[i] == ElementKind.O ? 2 : recipe.atoms[i] == ElementKind.Cl ? 3 : 0;
                    for (int j = 0; j < free; j++) Dot(recipe.positions[i] * .82f + new Vector3((j - 1) * .13f, .30f, 0));
                }
            }
        }
        void DrawLattice()
        {
            for (int x = -1; x <= 1; x++) for (int y = -1; y <= 1; y++) for (int z = -1; z <= 1; z++)
                Atom(((x + y + z) & 1) == 0 ? ElementKind.Na : ElementKind.Cl,
                    new Vector3(x, y, z) * .37f, .17f, true);
            if (electrons)
            {
                // One highlighted chloride octet keeps the 27-ion lattice readable.
                var chloride = new Vector3(.37f, 0, 0);
                for (int i = 0; i < 8; i++)
                {
                    float angle = i * Mathf.PI / 4;
                    Dot(chloride + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * .25f);
                }
            }
        }
        void Atom(ElementKind element, Vector3 p, float radius, bool ion = false)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere); go.name = BondCatalog.Symbols[(int)element];
            go.transform.SetParent(model, false); go.transform.localPosition = p; go.transform.localScale = Vector3.one * radius * 2;
            Destroy(go.GetComponent<Collider>()); go.GetComponent<Renderer>().sharedMaterial = atomMaterials[(int)element];
            var label = new GameObject("Symbol").AddComponent<TextMeshPro>(); label.transform.SetParent(model, false);
            label.transform.localPosition = p + Vector3.back * (radius + .02f); label.transform.localScale = Vector3.one * .11f;
            label.font = font; label.text = BondCatalog.Symbols[(int)element] + (ion ? element == ElementKind.Na ? "⁺" : "⁻" : "");
            label.fontSize = 4; label.alignment = TextAlignmentOptions.Center;
            label.color = element == ElementKind.H ? Color.black : Color.white;
        }
        void Dot(Vector3 p)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Sphere); go.name = "Par de elétrons";
            go.transform.SetParent(model, false); go.transform.localPosition = p; go.transform.localScale = Vector3.one * .065f;
            Destroy(go.GetComponent<Collider>()); go.GetComponent<Renderer>().sharedMaterial = electronMaterial;
        }
        void Link(Vector3 a, Vector3 b, float diameter, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder); go.name = "Ligação covalente";
            go.transform.SetParent(model, false); go.transform.localPosition = (a + b) * .5f;
            go.transform.localRotation = Quaternion.FromToRotation(Vector3.up, b - a);
            go.transform.localScale = new Vector3(diameter, Vector3.Distance(a, b) * .5f, diameter);
            Destroy(go.GetComponent<Collider>()); go.GetComponent<Renderer>().sharedMaterial = material;
        }
    }
}
