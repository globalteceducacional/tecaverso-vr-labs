using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Tecaverso.UI;
namespace Tecaverso.Labs.Fractions
{
    public sealed class FractionsLab:MonoBehaviour
    {
        public FractionPiece[] templates;
        public Transform[] dispensers,origins;
        public FractionSocket[] sockets;
        public Transform returnArea,preview;
        public LineRenderer equivalenceLine;
        public TMP_Text[] totals;
        public TMP_Text comparison,recipe,status,message;
        public Toggle divisions,decimals,equivalence;
        public Button undo,recover,clearRows,clearRecipe,back,confirm,cancel;
        public Button[] multipliers;
        public GameObject[] controls;
        public GameObject modal;
        public FractionSession Session {get;private set;}=new();
        public bool Modal {get;private set;}
        public readonly List<FractionPiece> Pieces=new();
        readonly List<FractionPiece>[] mounted={new(),new(),new(),new()};
        readonly List<Action> cleanup=new();
        Action pending;
        public int LooseCount=>Pieces.Count(p=>p.gameObject.activeSelf&&!p.supply&&p.dock<0);
        public bool AnyHeld=>Pieces.Any(p=>p.gameObject.activeSelf&&p.Held);
        void OnEnable()
        {
            Bind(undo,Undo);Bind(recover,Recover);Bind(clearRows,()=>Ask("Limpar as montagens A e B? A receita será preservada.",()=>{Session.ClearRows();RestoreRows();},Session.Total(0)+Session.Total(1)>0));
            Bind(clearRecipe,()=>Ask("Devolver todas as fichas da receita?",()=>{foreach(var p in mounted[2].Concat(mounted[3]).ToArray())Return(p);Session.ClearRecipe();Refresh();},Session.Recipe.Sum()>0));
            Bind(back,()=>Ask("Voltar ao Hub? As montagens desta visita não serão salvas.",()=>SceneManager.LoadSceneAsync("Assets/_Project/Hub/Scenes/Hub.unity"),Session.HasWork));
            Bind(confirm,()=>{var action=pending;Close();action?.Invoke();});Bind(cancel,Close);
            for(int i=0;i<3;i++){int m=i+1;Bind(multipliers[i],()=>{Session.Multiplier=m;Refresh();});}
            foreach(var toggle in new[]{divisions,decimals,equivalence}){UnityAction<bool> action=v=>Refresh();toggle.onValueChanged.AddListener(action);cleanup.Add(()=>toggle.onValueChanged.RemoveListener(action));}
            Close();
        }
        void Start(){Restock();Refresh();}
        void OnDisable(){foreach(var action in cleanup)action();cleanup.Clear();}
        void Bind(Button b,UnityAction action){b.onClick.AddListener(action);cleanup.Add(()=>b.onClick.RemoveListener(action));}
        public void Ask(string text,Action action,bool needed=true)
        {
            if(Modal)return;if(AnyHeld){status.text="Solte a peça antes de continuar.";return;}if(!needed){action();return;}
            pending=action;message.text=text;Modal=true;UIVisibility.Set(modal,true);foreach(var control in controls)UIVisibility.SetInteraction(control,false);foreach(var p in Pieces)p.enabled=false;preview.gameObject.SetActive(false);
        }
        void Close(){pending=null;Modal=false;UIVisibility.Set(modal,false);foreach(var control in controls)UIVisibility.SetInteraction(control,true);foreach(var p in Pieces)p.enabled=true;}
        FractionPiece Allocate(int kind)
        {
            var p=Pieces.FirstOrDefault(p=>p.kind==kind&&!p.gameObject.activeSelf);
            if(p==null){p=Instantiate(templates[kind],transform);p.name="Piece "+kind;p.lab=this;Pieces.Add(p);}
            p.dock=-1;p.supply=false;p.gameObject.SetActive(true);p.enabled=!Modal;if(p.divisions)p.divisions.SetActive(divisions.isOn);return p;
        }
        void Restock()
        {
            if(Pieces.Count(p=>p.gameObject.activeSelf&&!p.supply&&p.dock<0)>=6)return;
            for(int k=0;k<templates.Length;k++)if(!Pieces.Any(p=>p.gameObject.activeSelf&&p.supply&&p.kind==k))
            {var p=Allocate(k);p.supply=true;p.destination=dispensers[k].position;p.orientation=dispensers[k].rotation;p.transform.SetPositionAndRotation(p.destination,p.orientation);}
        }
        public void Take(FractionPiece p)
        {
            if(Modal)return;
            if(p.dock>=0){int dock=p.dock,index=mounted[dock].IndexOf(p);if(dock<2)Session.Remove(dock,index);else Session.RemoveToken(dock-2);mounted[dock].Remove(p);p.dock=-1;Arrange(dock);}
            p.supply=false;Restock();Refresh();
        }
        public Vector3 End(int dock)=>dock<2?origins[dock].TransformPoint(Vector3.right*(Session.Total(dock)/120f*1.2f)):origins[dock].position;
        Vector3 Placement(int dock,FractionPiece p)
        {
            if(dock<2)return End(dock)+origins[dock].right*(.6f/FractionSession.Denominators[p.kind]);
            int i=mounted[dock].Count;return origins[dock].TransformPoint(new Vector3((i%5-2)*.25f,(i/5)*.3f,0));
        }
        bool Fits(int dock,FractionPiece p)=>dock<2?p.kind<9&&Session.Fits(dock,FractionSession.Denominators[p.kind]):p.kind==dock+7&&Session.Recipe[dock-2]<10;
        int Nearest(FractionPiece p)
        {
            int result=-1;float closest=.65f;
            for(int i=0;i<4;i++){if((i<2)!=(p.kind<9))continue;float distance=Vector3.Distance(p.transform.position,End(i));if(distance<closest){closest=distance;result=i;}}return result;
        }
        public bool Dock(FractionPiece p,int dock)
        {
            if(Modal||p.Held||p.supply||p.dock>=0||!Fits(dock,p))return false;
            var socket=sockets[dock];var position=Placement(dock,p);socket.attachTransform.SetPositionAndRotation(position,origins[dock].rotation);socket.candidate=p;
            try{socket.interactionManager.SelectEnter((IXRSelectInteractor)socket,(IXRSelectInteractable)p);socket.interactionManager.SelectExit((IXRSelectInteractor)socket,(IXRSelectInteractable)p);}
            finally{socket.candidate=null;}
            if(dock<2)Session.Add(dock,FractionSession.Denominators[p.kind]);else Session.AddToken(dock-2);
            p.dock=dock;mounted[dock].Add(p);p.destination=position;p.orientation=origins[dock].rotation;p.transform.SetParent(transform,true);p.transform.SetPositionAndRotation(position,p.orientation);Arrange(dock);status.text="Peça encaixada. Pegue novamente para retirar.";Restock();Refresh();return true;
        }
        public void Release(FractionPiece p)
        {
            if(Modal)return;
            if(Vector3.Distance(p.transform.position,returnArea.position)<.8f){Return(p);return;}
            int dock=Nearest(p);if(dock>=0){if(!Dock(p,dock))status.text=Fits(dock,p)?"Solte para encaixar.":p.kind>=9&&p.kind!=dock+7?"Use o recipiente da mesma categoria.":"Não cabe neste espaço.";}
            else status.text="Solte perto da ponta livre ou devolva na área marcada. Peças soltas retornam em 8 s.";
            Refresh();
        }
        public void Return(FractionPiece p)
        {
            if(p.Held)return;
            if(p.dock>=0){int d=p.dock;int index=mounted[d].IndexOf(p);if(d<2)Session.Remove(d,index);else Session.RemoveToken(d-2);mounted[d].Remove(p);Arrange(d);}
            p.dock=-1;p.supply=false;p.gameObject.SetActive(false);Restock();Refresh();
        }
        void Arrange(int dock)
        {
            float x=0;for(int i=0;i<mounted[dock].Count;i++){var p=mounted[dock][i];if(p.Held)continue;
                if(dock<2){float width=1.2f/FractionSession.Denominators[p.kind];p.destination=origins[dock].TransformPoint(new Vector3(x+width/2,0,0));x+=width;}
                else p.destination=origins[dock].TransformPoint(new Vector3((i%5-2)*.25f,(i/5)*.3f,0));p.orientation=origins[dock].rotation;}
        }
        public void Recover(){foreach(var p in Pieces.Where(p=>p.gameObject.activeSelf&&!p.supply&&p.dock<0&&!p.Held).ToArray())Return(p);status.text=AnyHeld?"Peças soltas recolhidas. A peça na mão foi preservada.":"Peças soltas recolhidas.";}
        public void Undo(){if(Modal||AnyHeld)return;if(Session.Undo())RestoreRows();}
        void RestoreRows()
        {
            for(int d=0;d<2;d++){foreach(var p in mounted[d]){p.dock=-1;p.gameObject.SetActive(false);}mounted[d].Clear();foreach(int denominator in Session.Row(d)){var p=Allocate(Array.IndexOf(FractionSession.Denominators,denominator));p.dock=d;mounted[d].Add(p);}Arrange(d);foreach(var p in mounted[d])p.transform.SetPositionAndRotation(p.destination,p.orientation);}
            Restock();Refresh();
        }
        public void Refresh()
        {
            for(int r=0;r<2;r++){var expression=string.Join(" + ",Session.Row(r).Select(d=>d==1?"1":"1/"+d));totals[r].text=(r==0?"A":"B")+": "+(expression==""?"Vazia":expression)+"\nTotal: "+Session.TotalText(r,decimals.isOn);}
            int a=Session.Total(0),b=Session.Total(1);comparison.text=equivalence.isOn?(a==b?"A = B · Mesmo comprimento":a>b?"A > B":"A < B"):"1 inteiro = 1,2 m · Cada linha comporta 2 inteiros";
            equivalenceLine.enabled=equivalence.isOn;equivalenceLine.SetPosition(0,End(0));equivalenceLine.SetPosition(1,End(1));
            recipe.text=Session.RecipeText()+"\nParticipação de A = A / (A + B)";
            for(int i=0;i<3;i++)multipliers[i].GetComponentInChildren<TMP_Text>().text=(i+1)+"×"+(Session.Multiplier==i+1?" · ativo":"");
            undo.interactable=Session.CanUndo&&!AnyHeld;foreach(var p in Pieces)if(p.divisions)p.divisions.SetActive(divisions.isOn);
        }
        void Update()
        {
            if(Modal)return;var held=Pieces.FirstOrDefault(p=>p.gameObject.activeSelf&&p.Held);undo.interactable=Session.CanUndo&&held==null;
            if(!held){preview.gameObject.SetActive(false);return;}
            int dock=Nearest(held);bool valid=dock>=0&&Fits(dock,held);preview.gameObject.SetActive(valid);
            if(valid){preview.SetPositionAndRotation(Placement(dock,held),origins[dock].rotation);preview.localScale=new Vector3(held.kind<9?1.2f/FractionSession.Denominators[held.kind]:.2f,.26f,.04f);status.text="Solte para encaixar aqui.";}
            else if(dock>=0)status.text="Não cabe neste espaço ou categoria incompatível.";
        }
    }
}
