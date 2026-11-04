using System;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Tecaverso.UI;

namespace Tecaverso.Labs.Functions
{
    public sealed class FunctionsLab : MonoBehaviour
    {
        [Serializable] public sealed class CoefficientControl
        { public FunctionKnob knob; public Button minus,plus; public TMP_Text value; public GameObject station; }
        public FunctionPlot plot;
        public CoefficientControl[] coefficients;
        public Button affine,quadratic,markerMinus,markerPlus,saveReference,removeReference,restore,undo,back,confirm,cancel;
        public Toggle gridToggle,featuresToggle;
        public TMP_Text expression,referenceText,analysis,coordinate,familyText,modalText;
        public GameObject content,modal;
        public FunctionSession Session { get; private set; }=new FunctionSession();
        public bool Modal { get; private set; }
        bool syncing,leaving;
        Action confirmed;
        readonly System.Collections.Generic.List<Action> unsubscribe=new();
        static readonly CultureInfo Pt=CultureInfo.GetCultureInfo("pt-BR");
        public static string Number(double value)=>Math.Abs(value)<.00005?"0":value.ToString("0.###",Pt);
        public static string Expression(Polynomial p)
        {
            string result="";
            void Term(double n,string suffix)
            {
                if(n==0)return;result+=(result.Length==0?(n<0?"−":""):(n<0?" − ":" + "));
                if(Math.Abs(n)!=1||suffix.Length==0)result+=Number(Math.Abs(n)); result+=suffix;
            }
            Term(p.A,"x²");Term(p.B,"x");Term(p.C,"");return "f(x) = "+(result.Length==0?"0":result);
        }
        void OnEnable()
        {
            Bind(affine,()=>SelectFamily(0));Bind(quadratic,()=>SelectFamily(1));
            for(int i=0;i<coefficients.Length;i++)
            {
                int index=i;var row=coefficients[i];
                Bind(row.minus,()=>SetCoefficient(index,Session.Current.Coefficient(index)-.1));
                Bind(row.plus,()=>SetCoefficient(index,Session.Current.Coefficient(index)+.1));
                UnityAction<float> changed=v=>{if(!syncing)SetCoefficient(index,Mathf.Lerp(index==0?-3:-5,index==0?3:5,v));};
                row.knob.onValueChange.AddListener(changed);unsubscribe.Add(()=>row.knob.onValueChange.RemoveListener(changed));
            }
            Bind(markerMinus,()=>StepMarker(-.1));Bind(markerPlus,()=>StepMarker(.1));
            Bind(saveReference,RequestReference);Bind(removeReference,()=>{if(Modal)return;Session.RemoveReference();Refresh();});
            Bind(restore,()=>{if(Modal)return;Session.Restore();Refresh();});Bind(undo,()=>{if(Modal)return;Session.UndoRestore();Refresh();});
            Bind(back,RequestBack);Bind(cancel,CloseModal);Bind(confirm,Confirm);
            UnityAction<bool> grid=v=>{if(!Modal)Refresh();};UnityAction<bool> features=v=>{if(!Modal)Refresh();};
            gridToggle.onValueChanged.AddListener(grid);featuresToggle.onValueChanged.AddListener(features);
            unsubscribe.Add(()=>gridToggle.onValueChanged.RemoveListener(grid));unsubscribe.Add(()=>featuresToggle.onValueChanged.RemoveListener(features));
            CloseModal();Refresh();
        }
        void OnDisable(){foreach(var action in unsubscribe)action();unsubscribe.Clear();}
        void Bind(Button button,UnityAction action){button.onClick.AddListener(action);unsubscribe.Add(()=>button.onClick.RemoveListener(action));}
        public void SelectFamily(int family){if(Modal)return;Session.Select((FunctionFamily)family);Refresh();}
        public void SetCoefficient(int index,double value)
        {if(syncing||Modal||index==2&&Session.Family==FunctionFamily.Affine)return;Session.Set(index,value);Refresh();}
        public void StepMarker(double step){if(Modal)return;Session.StepMarker(step);RefreshMarker();}
        public void DragMarker(double x){if(Modal)return;Session.DragMarker(x);RefreshMarker();}
        public void Refresh()
        {
            syncing=true;
            for(int i=0;i<coefficients.Length;i++)
            {
                var row=coefficients[i];bool visible=i<2||Session.Family==FunctionFamily.Quadratic;
                row.station.SetActive(visible);if(!visible)continue;
                float value=(float)Session.Current.Coefficient(i),limit=i==0?3:5;
                row.knob.value=Mathf.InverseLerp(-limit,limit,value);row.value.text=((char)('a'+i))+" = "+Number(value);
                row.minus.interactable=value>-limit;row.plus.interactable=value<limit;
            }
            syncing=false;
            affine.GetComponentInChildren<TMP_Text>().text=Session.Family==FunctionFamily.Affine?"Afim · ativa":"Afim";
            quadratic.GetComponentInChildren<TMP_Text>().text=Session.Family==FunctionFamily.Quadratic?"Quadrática · ativa":"Quadrática";
            expression.text="CURVA ATIVA · "+Expression(Session.Current);
            referenceText.text=Session.Reference.HasValue?"REFERÊNCIA TRACEJADA · "+Expression(Session.Reference.Value):"Referência: nenhuma curva guardada";
            familyText.text=Session.Family==FunctionFamily.Affine?"Afim: a altera a inclinação; b é o intercepto vertical.":Session.Current.A==0?"a = 0: a expressão deixou de ser quadrática.":"Quadrática: explore concavidade, raízes e vértice.";
            saveReference.GetComponentInChildren<TMP_Text>().text=Session.Reference.HasValue?"Substituir referência":"Guardar referência";
            UIVisibility.Set(removeReference.gameObject,Session.Reference.HasValue,true);
            UIVisibility.Set(undo.gameObject,Session.CanUndo,true);
            analysis.text=featuresToggle.isOn?Analysis(Session.Current):"Ative raízes e vértice para consultar os resultados.";
            plot.Draw(Session,gridToggle.isOn,featuresToggle.isOn);RefreshMarker();
        }
        public static string Analysis(Polynomial p)
        {
            string text;
            if(FunctionMath.InfiniteRoots(p))text="Raízes: todos os números reais.";
            else
            {
                var roots=FunctionMath.Roots(p);
                text=roots.Length==0?"Sem raízes reais.":"Raízes: "+string.Join("; ",roots.Select(x=>Number(x)+(x< -6||x>6?" (fora da janela)":"")))+".";
            }
            if(FunctionMath.Vertex(p,out var vertex))
                text+="\nVértice: ("+Number(vertex.x)+"; "+Number(vertex.y)+")"+(FunctionMath.Visible(vertex.x,vertex.y)?"":" · fora da janela")+".\nΔ = "+Number(p.B*p.B-4*p.A*p.C);
            else text+="\nEsta função não possui vértice.";
            return text;
        }
        void RefreshMarker()
        {
            double x=Session.MarkerX,y=Session.Current.Evaluate(x);
            coordinate.text="P ("+Number(x)+"; "+Number(y)+")"+(FunctionMath.Visible(x,y)?"":"\nPonto fora da janela");
            markerMinus.interactable=x> -6;markerPlus.interactable=x<6;plot.DrawMarker(Session);
        }
        public void RequestReference()
        {if(Modal)return;if(Session.Reference.HasValue)OpenModal("Substituir a curva de referência guardada?",()=>{Session.SaveReference();Refresh();});else{Session.SaveReference();Refresh();}}
        public void RequestBack()
        {if(Modal||leaving)return;if(Session.HasWork)OpenModal("Voltar ao Hub? Os ajustes e a referência desta visita não serão salvos.",Leave);else Leave();}
        void Leave(){leaving=true;back.interactable=false;SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity");}
        void OpenModal(string message,Action action)
        {Modal=true;confirmed=action;modalText.text=message;UIVisibility.SetInteraction(content,false);LockInstruments(true);UIVisibility.Set(modal,true);}
        void LockInstruments(bool locked)
        {
            foreach(var row in coefficients)
            {
                row.knob.enabled=!locked;
                UIVisibility.SetInteraction(row.minus.transform.parent.parent.gameObject,!locked);
            }
            plot.marker.enabled=!locked;
        }
        public void CloseModal(){Modal=false;confirmed=null;UIVisibility.Set(modal,false);UIVisibility.SetInteraction(content,true);LockInstruments(false);}
        public void Confirm(){if(!Modal)return;var action=confirmed;CloseModal();action?.Invoke();}
    }
}
