using System;
using System.Collections.Generic;
using System.Linq;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    public enum ReactionKind { Gas, Precipitation, Neutralization, Dissolution, BlueSolution }

    public sealed class ChemicalSpecies
    {
        public readonly string Formula, Name;
        public readonly IReadOnlyDictionary<string,int> Atoms;
        public ChemicalSpecies(string formula,string name,params (string element,int count)[] atoms)
        {
            Formula=formula;Name=name;
            Atoms=atoms.ToDictionary(p=>p.element,p=>p.count);
        }
    }

    public sealed class ReactionDefinition
    {
        public readonly ReactionKind Kind;
        public readonly string Title, Observation;
        public readonly ChemicalSpecies[] Species;
        public readonly int[] Stoichiometry;
        public readonly string[] Elements;
        public const double Concentration=0.10; // mol/L, explicitly labelled educational stock solutions.
        public double SolidMolarMass {get; private set;}
        public bool PowderSecond=>SolidMolarMass>0;
        public bool SupportsPH=>ReferenceEquals(this,All[2]);
        // H+(aq) + OH-(aq) -> H2O(l), OpenStax Chemistry 2e Appendix G.
        // Formation enthalpies: 0, -230.0, -285.83 kJ/mol. Unknown is never zero.
        public double ReactionEnthalpyKJPerMol=>ReferenceEquals(this,All[2])||ReferenceEquals(this,All[7])?-55.83:double.NaN;
        public ReactionDefinition(ReactionKind kind,string title,string observation,ChemicalSpecies[] species,int[] coefficients)
        {
            Kind=kind;Title=title;Observation=observation;Species=species;Stoichiometry=coefficients;
            Elements=species.SelectMany(s=>s.Atoms.Keys).Distinct().ToArray();
        }
        public int AtomCount(int species,string element)=>Species[species].Atoms.TryGetValue(element,out int count)?count:0;
        public bool IsBalanced(int[] coefficients)
        {
            if(coefficients==null||coefficients.Length!=Species.Length||coefficients.Any(c=>c<1))return false;
            foreach(var element in Elements)
                if(Enumerable.Range(0,Species.Length).Sum(i=>(i<2?1:-1)*coefficients[i]*AtomCount(i,element))!=0)return false;
            return true;
        }
        public static readonly ReactionDefinition[] All={
            new ReactionDefinition(ReactionKind.Gas,"Formação de gás","Bolhas de CO₂; o gás liberado também entra na conservação.",new[]{
                new ChemicalSpecies("CH₃COOH","Ácido acético diluído",("C",2),("H",4),("O",2)),
                new ChemicalSpecies("NaHCO₃","Bicarbonato de sódio",("Na",1),("H",1),("C",1),("O",3)),
                new ChemicalSpecies("CH₃COONa","Acetato de sódio",("C",2),("H",3),("O",2),("Na",1)),
                new ChemicalSpecies("H₂O","Água formada",("H",2),("O",1)),
                new ChemicalSpecies("CO₂","Gás liberado",("C",1),("O",2))},new[]{1,1,1,1,1}),
            new ReactionDefinition(ReactionKind.Precipitation,"Formação de precipitado","Surge CaCO₃ sólido; Na⁺ e Cl⁻ permanecem na solução.",new[]{
                new ChemicalSpecies("CaCl₂","Cloreto de cálcio",("Ca",1),("Cl",2)),
                new ChemicalSpecies("Na₂CO₃","Carbonato de sódio",("Na",2),("C",1),("O",3)),
                new ChemicalSpecies("CaCO₃(s)","Carbonato de cálcio",("Ca",1),("C",1),("O",3)),
                new ChemicalSpecies("NaCl","Cloreto de sódio",("Na",1),("Cl",1))},new[]{1,1,1,2}),
            new ReactionDefinition(ReactionKind.Neutralization,"Neutralização","Fenolftaleína: rosa em meio básico; incolor em meio neutro ou ácido.",new[]{
                new ChemicalSpecies("HCl","Ácido clorídrico",("H",1),("Cl",1)),
                new ChemicalSpecies("NaOH","Hidróxido de sódio",("Na",1),("O",1),("H",1)),
                new ChemicalSpecies("NaCl","Cloreto de sódio",("Na",1),("Cl",1)),
                new ChemicalSpecies("H₂O","Água formada",("H",2),("O",1))},new[]{1,1,1,1})
        };
        static ReactionDefinition()
        {
            All[0].SolidMolarMass=84.0066;
            var list=All.ToList();
            ChemicalSpecies S(string formula,string name,params (string,int)[] atoms)=>new ChemicalSpecies(formula,name,atoms);
            var hcl=All[2].Species[0];var naoh=All[2].Species[1];var water=All[2].Species[3];var salt=All[2].Species[2];
            var acetic=All[0].Species[0];var bicarb=All[0].Species[1];var co2=All[0].Species[4];
            var sulfuric=S("H₂SO₄","Ácido sulfúrico diluído",("H",2),("S",1),("O",4));
            var magnesiumChloride=S("MgCl₂","Cloreto de magnésio",("Mg",1),("Cl",2));
            void Add(ReactionKind kind,string title,string observation,double solidMass,ChemicalSpecies[] species,int[] coefficients)
            {var d=new ReactionDefinition(kind,title,observation,species,coefficients);d.SolidMolarMass=solidMass;list.Add(d);}
            Add(ReactionKind.Gas,"Ácido e bicarbonato","Liberação de CO₂; o gás permanece no balanço.",84.0066,new[]{hcl,bicarb,salt,water,co2},new[]{1,1,1,1,1});
            Add(ReactionKind.Gas,"Ácido e carbonato","Liberação de CO₂; dois mols de HCl por mol de carbonato.",0,new[]{hcl,All[1].Species[1],salt,water,co2},new[]{2,1,2,1,1});
            Add(ReactionKind.Neutralization,"Ácido acético e base","Pode haver reação sem bolhas nem mudança de cor.",0,new[]{acetic,naoh,All[0].Species[2],water},new[]{1,1,1,1});
            Add(ReactionKind.Dissolution,"Óxido de magnésio e ácido","Consumo do sólido branco, sem gás. Duração visual didática.",40.3044,new[]{hcl,S("MgO","Óxido de magnésio",("Mg",1),("O",1)),magnesiumChloride,water},new[]{2,1,1,1});
            Add(ReactionKind.Neutralization,"Hidróxido de potássio e ácido","Neutralização sem mudança visual obrigatória.",0,new[]{hcl,S("KOH","Hidróxido de potássio",("K",1),("O",1),("H",1)),S("KCl","Cloreto de potássio",("K",1),("Cl",1)),water},new[]{1,1,1,1});
            Add(ReactionKind.Dissolution,"Base sólida e ácido","Consumo de Mg(OH)₂ branco; sem bolhas. Dissolução didática.",58.3197,new[]{hcl,S("Mg(OH)₂","Hidróxido de magnésio",("Mg",1),("O",2),("H",2)),magnesiumChloride,water},new[]{2,1,1,2});
            Add(ReactionKind.Neutralization,"Neutralização 1:2","Balanço global de neutralização completa; não representa equilíbrio nem pH.",0,new[]{sulfuric,naoh,S("Na₂SO₄","Sulfato de sódio",("Na",2),("S",1),("O",4)),water},new[]{1,2,1,2});
            Add(ReactionKind.BlueSolution,"Óxido de cobre(II) e ácido","Sólido preto consumido; solução azul. Não há precipitado azul nem gás.",79.545,new[]{sulfuric,S("CuO","Óxido de cobre(II)",("Cu",1),("O",1)),S("CuSO₄","Sulfato de cobre(II)",("Cu",1),("S",1),("O",4)),water},new[]{1,1,1,1});
            Add(ReactionKind.Dissolution,"Óxido de zinco e ácido","Consumo do sólido branco, sem gás. Duração visual didática.",81.379,new[]{hcl,S("ZnO","Óxido de zinco",("Zn",1),("O",1)),S("ZnCl₂","Cloreto de zinco",("Zn",1),("Cl",2)),water},new[]{2,1,1,1});
            All=list.ToArray();
        }
    }
}
