using System;
using System.Linq;

namespace Tecaverso.Labs.Chemistry.Reactions
{
    public sealed class ReagentDefinition
    {
        public readonly string Id,Formula,Name,Group;
        public readonly bool Solid;
        public ReagentDefinition(string id,string formula,string name,string group,bool solid=false)
        {Id=id;Formula=formula;Name=name;Group=group;Solid=solid;}
    }

    /// <summary>Authoritative identity gate. Runtime transfers use this API, never a selected UI index.</summary>
    public sealed class ReactionWorkbench
    {
        public static readonly ReagentDefinition[] Reagents={
            new ReagentDefinition("acetic","CH₃COOH","Ácido acético","ÁCIDOS"),
            new ReagentDefinition("hcl","HCl","Ácido clorídrico","ÁCIDOS"),
            new ReagentDefinition("sulfuric","H₂SO₄","Ácido sulfúrico diluído","ÁCIDOS"),
            new ReagentDefinition("naoh","NaOH","Hidróxido de sódio","BASES"),
            new ReagentDefinition("koh","KOH","Hidróxido de potássio","BASES"),
            new ReagentDefinition("mgoh","Mg(OH)₂","Hidróxido de magnésio","BASES",true),
            new ReagentDefinition("bicarb","NaHCO₃","Bicarbonato de sódio","SAIS",true),
            new ReagentDefinition("cacl","CaCl₂","Cloreto de cálcio","SAIS"),
            new ReagentDefinition("carb","Na₂CO₃","Carbonato de sódio","SAIS"),
            new ReagentDefinition("mgo","MgO","Óxido de magnésio","ÓXIDOS",true),
            new ReagentDefinition("cuo","CuO","Óxido de cobre(II)","ÓXIDOS",true),
            new ReagentDefinition("zno","ZnO","Óxido de zinco","ÓXIDOS",true)
        };
        public string A {get;private set;}
        public string B {get;private set;}
        public ReactionSession Session {get;private set;}
        public bool Ready=>Session!=null;
        public bool Used=>Session!=null&&Session.Used;
        public static ReagentDefinition Find(string id)=>Array.Find(Reagents,r=>r.Id==id);
        public static ReactionDefinition Recipe(string a,string b)
        {
            var x=Find(a);var y=Find(b);if(x==null||y==null||a==b)return null;
            return Array.Find(ReactionDefinition.All,d=>(d.Species[0].Formula==x.Formula&&d.Species[1].Formula==y.Formula)||(d.Species[1].Formula==x.Formula&&d.Species[0].Formula==y.Formula));
        }
        public bool SelectA(string id){if(Ready||Find(id)==null)return false;A=id;B=null;return true;}
        public void RemoveA(){if(!Ready){A=B=null;}}
        public bool SelectB(string id)
        {
            if(Ready)return false;var recipe=Recipe(A,id);if(recipe==null)return false;
            B=id;Session=new ReactionSession(recipe);return true;
        }
        public void Reset(){A=B=null;Session=null;}
        public int Source(string id)
        {
            if(!Ready||id!=A&&id!=B)return -1;
            return Session.Definition.Species[0].Formula==Find(id).Formula?0:1;
        }
        public string BlockReason(string id)
        {
            if(!Ready)return "Escolha os dois reagentes nos suportes A e B antes de dosar.";
            if(Source(id)<0)return "Este reagente não faz parte do par. Use Nova mistura para trocar.";
            return null;
        }
        public ReactionSession.Transfer PourLiquid(string id,double ml,bool hit)
        {
            int source=Source(id);if(source<0||Find(id).Solid)return default;
            return Session.PourLiquid(source,ml,hit);
        }
        public double LoadSpoon(string id)=>Source(id)==1&&Find(id).Solid?Session.LoadSpoon():0;
        public ReactionSession.Transfer PourPowder(string id,double grams,bool hit)
            =>Source(id)==1&&Find(id).Solid?Session.PourPowder(grams,hit):default;
        public bool AddIndicator()=>Ready&&Session.AddIndicator();
        public double Stock(string id)
        {
            var reagent=Find(id);if(reagent==null)return 0;int source=Source(id);
            return reagent.Solid?(source<0?ReactionSession.PowderGrams:Session.PowderStock):(source<0?ReactionSession.StockMl:Session.SourceMl[source]);
        }
        public double Spoon(string id)=>Source(id)==1&&Find(id).Solid?Session.SpoonGrams:0;
    }
}
