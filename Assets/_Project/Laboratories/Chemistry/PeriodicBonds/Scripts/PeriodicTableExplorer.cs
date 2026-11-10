using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace Tecaverso.Labs.Chemistry.PeriodicBonds
{
    public sealed class PeriodicTableExplorer : MonoBehaviour
    {
        [Serializable] public class Dataset { public Table Table; }
        [Serializable] public class Table { public Row[] Row; }
        [Serializable] public class Row { public string[] Cell; }
        public TextAsset data;
        public PeriodicElementCube[] cubes;
        public TMP_Text heading, body, pageLabel;
        public Button[] pages;
        public Button clear, back;
        public int SelectedNumber { get; private set; }
        public int Page { get; private set; }
        Dataset catalog;
        public ElementReferenceView reference;
        void Awake() { catalog=JsonUtility.FromJson<Dataset>(data.text); }
        void Start()
        {
            for(int i=0;i<pages.Length;i++){int p=i;pages[i].onClick.AddListener(()=>SetPage(p));}
            clear.onClick.AddListener(()=>Select(0));
            back.onClick.AddListener(()=>SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity"));
            Select(0);
        }
        public void SetPage(int page) { Page=Mathf.Clamp(page,0,3); Refresh(); }
        public void Select(int number)
        {
            SelectedNumber=Mathf.Clamp(number,0,118);
            foreach(var cube in cubes)cube.SetState(number==cube.number,number!=0&&number!=cube.number);
            Refresh();
        }
        string Value(int field,string unit="")
        {
            string v=catalog.Table.Row[SelectedNumber-1].Cell[field];
            return string.IsNullOrWhiteSpace(v)?"Não disponível":v+unit;
        }
        static string Translate(string v)
        {
            switch(v) {
                case "Solid":return "Sólido";case "Gas":return "Gasoso";case "Liquid":return "Líquido";
                case "Nonmetal":return "Não metal";case "Noble gas":return "Gás nobre";
                case "Alkali metal":return "Metal alcalino";case "Alkaline earth metal":return "Metal alcalinoterroso";
                case "Transition metal":return "Metal de transição";case "Post-transition metal":return "Metal pós-transição";
                case "Metalloid":return "Semimetal";case "Halogen":return "Halogênio";
                case "Lanthanide":return "Lantanídeo";case "Actinide":return "Actinídeo"; default:return v;
            }
        }
        void Refresh()
        {
            if(reference)reference.Prepare(body,SelectedNumber,Page);
            for(int i=0;i<pages.Length;i++)pages[i].GetComponent<Image>().color=i==Page?new Color(.46f,.28f,.91f):new Color(.16f,.31f,.63f);
            pageLabel.text=new[]{"VISÃO GERAL","PROPRIEDADES","NA NATUREZA","FONTES"}[Page];
            if(SelectedNumber==0){heading.text="TABELA PERIÓDICA";body.text="118 elementos para explorar\n\nAponte para um cubo e pressione o gatilho.\n\nO elemento escolhido fica destacado. Os demais continuam selecionáveis.\n\nConsulte as páginas desta ficha e use Limpar seleção para retornar à visão geral.";if(reference)reference.Complete();return;}
            var e=PeriodicElementCatalog.All[SelectedNumber-1];
            heading.text=e.symbol+" · "+e.name+"   |   Z = "+e.atomicNumber;
            switch(Page)
            {
                case 0:body.text="Categoria: "+Translate(Value(15))+"\nPeríodo: "+e.period+"   ·   "+(e.group>0?"Grupo: "+e.group:e.IsLanthanide?"Série dos lantanídeos":"Série dos actinídeos")+"\n\nMassa atômica: "+Value(3," u")+"\nEstado padrão: "+Translate(Value(11))+"\nDescoberta: "+Value(16)+"\n\nConfiguração eletrônica:\n"+Value(5)+"\n\nValores entre colchetes indicam número de massa de um isótopo, não massa atômica média.";break;
                case 1:body.text="Eletronegatividade (Pauling): "+Value(6)+"\nRaio atômico: "+Value(7," pm")+"\nEnergia de ionização: "+Value(8," eV")+"\nAfinidade eletrônica: "+Value(9," eV")+"\nEstados de oxidação: "+Value(10)+"\n\nFusão: "+Value(12," K")+"\nEbulição: "+Value(13," K")+"\nDensidade: "+Value(14," g/cm³")+"\n\nDados de referência PubChem. Temperaturas em kelvin. As condições e incertezas variam conforme a propriedade; não são uma simulação de aquecimento.";break;
                case 2:
                    var details=reference.Get(SelectedNumber);
                    body.text="<b>OCORRÊNCIA</b>\n"+details.occurrence+"\n\n<b>APLICAÇÕES</b>\n"+details.applications;
                    break;
                default:
                    var entry=reference.Get(SelectedNumber);
                    body.text="<b>DADOS E TEXTOS · BASE LOCAL</b>\nRevisão: 09/11/2026. Síntese educativa em português; usos do elemento, compostos e isótopos são diferenciados.\n"+entry.sources+"\n\n<b>FOTOGRAFIA</b>\n"+(string.IsNullOrEmpty(entry.resource)?entry.photoStatus:entry.caption+"\nAutor: "+entry.author+"\n"+entry.license+"\n"+entry.licenseUrl+"\n"+entry.photoSource+"\nImagem redimensionada para uso offline.")+"\n\nNenhuma consulta à internet é feita nesta sala. Campos ausentes não significam zero.";
                    break;
            }
            if(reference)reference.Complete();
        }
    }
}

