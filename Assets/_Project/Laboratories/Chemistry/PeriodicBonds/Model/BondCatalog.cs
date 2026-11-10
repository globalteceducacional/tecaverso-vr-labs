using System;
using System.Collections.Generic;
using UnityEngine;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    public enum ElementKind { H, C, O, Na, Cl }
    public enum BondKind { H2, O2, H2O, CO2, NaCl }

    public sealed class BondRecipe
    {
        public readonly BondKind kind;
        public readonly string formula, name, classification, explanation;
        public readonly ElementKind[] atoms;
        public readonly Vector3[] positions;
        public readonly int[] bondPairs;
        public readonly int[] bondOrders;
        public readonly bool ionic;

        public BondRecipe(BondKind kind, string formula, string name, string classification, string explanation,
            ElementKind[] atoms, Vector3[] positions, int[] bondPairs, int[] bondOrders, bool ionic=false)
        {
            this.kind=kind;this.formula=formula;this.name=name;this.classification=classification;
            this.explanation=explanation;this.atoms=atoms;this.positions=positions;
            this.bondPairs=bondPairs;this.bondOrders=bondOrders;this.ionic=ionic;
        }
    }

    public static class BondCatalog
    {
        public static readonly ElementKind[] Active={ElementKind.H,ElementKind.C,ElementKind.O,ElementKind.Na,ElementKind.Cl};
        public static readonly string[] Symbols={"H","C","O","Na","Cl"};
        public static readonly string[] Names={"Hidrogênio","Carbono","Oxigênio","Sódio","Cloro"};
        public static readonly int[] AtomicNumbers={1,6,8,11,17};
        public static readonly int[] ValenceElectrons={1,4,6,1,7};
        public static readonly Color[] Colors={
            new Color(.92f,.94f,1f),new Color(.18f,.25f,.35f),new Color(.92f,.25f,.28f),
            new Color(.65f,.43f,.9f),new Color(.26f,.76f,.42f)};

        public static readonly BondRecipe[] Recipes={
            new BondRecipe(BondKind.H2,"H₂","Hidrogênio molecular","Molécula covalente",
                "Dois átomos compartilham um par de elétrons. Modelo de ligação simples.",
                new[]{ElementKind.H,ElementKind.H},new[]{new Vector3(-.45f,0,0),new Vector3(.45f,0,0)},new[]{0,1},new[]{1}),
            new BondRecipe(BondKind.O2,"O₂","Oxigênio molecular","Molécula covalente",
                "Os oxigênios compartilham dois pares de elétrons. Modelo de ligação dupla.",
                new[]{ElementKind.O,ElementKind.O},new[]{new Vector3(-.5f,0,0),new Vector3(.5f,0,0)},new[]{0,1},new[]{2}),
            new BondRecipe(BondKind.H2O,"H₂O","Água","Molécula covalente angular",
                "O oxigênio compartilha um par com cada hidrogênio. A forma é angular.",
                new[]{ElementKind.O,ElementKind.H,ElementKind.H},
                new[]{Vector3.zero,new Vector3(-.62f,-.50f,0),new Vector3(.62f,-.50f,0)},new[]{0,1,0,2},new[]{1,1}),
            new BondRecipe(BondKind.CO2,"CO₂","Dióxido de carbono","Molécula covalente linear",
                "O carbono faz uma ligação dupla com cada oxigênio. O modelo é linear.",
                new[]{ElementKind.O,ElementKind.C,ElementKind.O},
                new[]{new Vector3(-.95f,0,0),Vector3.zero,new Vector3(.95f,0,0)},new[]{0,1,1,2},new[]{2,2}),
            new BondRecipe(BondKind.NaCl,"NaCl","Cloreto de sódio","Sólido iônico · rede cristalina",
                "Há transferência de elétron e atração entre Na⁺ e Cl⁻. O sólido forma uma rede, não moléculas isoladas.",
                new[]{ElementKind.Na,ElementKind.Cl},new[]{new Vector3(-.5f,0,0),new Vector3(.5f,0,0)},
                Array.Empty<int>(),Array.Empty<int>(),true)
        };

        public static BondRecipe Match(IReadOnlyList<ElementKind?> slots)
        {
            foreach(var recipe in Recipes)if(Exact(slots,recipe))return recipe;
            return null;
        }

        public static bool HasCompletion(IReadOnlyList<ElementKind?> slots)
        {
            foreach(var recipe in Recipes)if(Subset(slots,recipe))return true;
            return false;
        }

        static bool Exact(IReadOnlyList<ElementKind?> slots,BondRecipe recipe)
        {
            int placed=0;for(int i=0;i<slots.Count;i++)if(slots[i].HasValue)placed++;
            return placed==recipe.atoms.Length&&Subset(slots,recipe);
        }

        static bool Subset(IReadOnlyList<ElementKind?> slots,BondRecipe recipe)
        {
            var used=new int[Active.Length];for(int i=0;i<slots.Count;i++)if(slots[i].HasValue)used[(int)slots[i].Value]++;
            var available=new int[Active.Length];foreach(var atom in recipe.atoms)available[(int)atom]++;
            for(int i=0;i<used.Length;i++)if(used[i]>available[i])return false;
            return true;
        }

        public static string Composition(IReadOnlyList<ElementKind?> slots)
        {
            var counts=new int[Active.Length];for(int i=0;i<slots.Count;i++)if(slots[i].HasValue)counts[(int)slots[i].Value]++;
            var parts=new List<string>();for(int i=0;i<counts.Length;i++)if(counts[i]>0)parts.Add(Symbols[i]+(counts[i]>1?" × "+counts[i]:""));
            return parts.Count==0?"Nenhum átomo encaixado":string.Join("  ·  ",parts);
        }
    }

    public sealed class BondAssembly
    {
        public const int SlotCount=4;
        readonly ElementKind?[] slots=new ElementKind?[SlotCount];
        public IReadOnlyList<ElementKind?> Slots=>slots;
        public BondRecipe Matched=>BondCatalog.Match(slots);
        public int Count {get{int count=0;foreach(var slot in slots)if(slot.HasValue)count++;return count;}}
        public bool CanPlace(int index,ElementKind element)
        {
            if(index<0||index>=SlotCount||slots[index].HasValue)return false;
            slots[index]=element;bool allowed=BondCatalog.HasCompletion(slots);slots[index]=null;return allowed;
        }
        public bool Place(int index,ElementKind element)
        {if(!CanPlace(index,element))return false;slots[index]=element;return true;}
        public void Remove(int index){if(index>=0&&index<SlotCount)slots[index]=null;}
        public void Clear(){for(int i=0;i<SlotCount;i++)slots[i]=null;}
    }
}
