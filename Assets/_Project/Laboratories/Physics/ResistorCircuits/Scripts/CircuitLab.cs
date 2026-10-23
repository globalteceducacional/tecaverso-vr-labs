using System;
using System.Collections.Generic;
using Tecaverso.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class CircuitLab : MonoBehaviour
    {
        public CircuitTerminal[] terminals;
        public CircuitPart[] parts;
        public FlexibleCable[] cables;
        public TMP_Text summary;
        public Button resetButton,backButton,confirmButton,cancelButton;
        public GameObject leaveDialog,pages;
        bool loading;
        void Awake()
        {
            resetButton.onClick.AddListener(ResetAssembly);backButton.onClick.AddListener(OpenLeave);
            confirmButton.onClick.AddListener(Leave);cancelButton.onClick.AddListener(CancelLeave);
            UIVisibility.Set(leaveDialog,false);Recalculate();
        }
        public void Recalculate()
        {
            if(terminals==null||parts==null||cables==null)return;
            var wires=new List<(int,int)>();var branches=new List<CircuitGraph.Branch>();
            int positive=-1,negative=-1;var branchOwners=new List<CircuitPart>();
            foreach(var part in parts)
            {
                if(part.a==null||part.b==null)continue;
                if(part.role==ElectricalRole.Source){positive=part.a.id;negative=part.b.id;continue;}
                if(part.role==ElectricalRole.Voltmeter)continue;
                if(part.role==ElectricalRole.Junction||part.role==ElectricalRole.Switch&&part.switchClosed)
                {
                    wires.Add((part.a.id,part.b.id));
                    if(part.role==ElectricalRole.Junction&&part.additionalTerminals!=null)
                        foreach(var terminal in part.additionalTerminals)wires.Add((part.a.id,terminal.id));
                    continue;
                }
                if(part.role==ElectricalRole.Switch)continue;
                branches.Add(new CircuitGraph.Branch(part.a.id,part.b.id,part.role==ElectricalRole.Ammeter?.01:part.resistance));branchOwners.Add(part);
            }
            var cableBranches=new int[cables.Length];
            for(int i=0;i<cables.Length;i++)
            {
                cableBranches[i]=-1;var cable=cables[i];
                if(cable.first.terminal==null||cable.second.terminal==null)continue;
                cableBranches[i]=branches.Count;
                branches.Add(new CircuitGraph.Branch(cable.first.terminal.id,cable.second.terminal.id,.01));branchOwners.Add(null);
            }
            if(positive<0||negative<0)return;
            var result=CircuitGraph.Solve(terminals.Length,wires,branches,positive,negative,9);
            double total=0;
            for(int i=0;i<branches.Count;i++)
            {
                var part=branchOwners[i];if(part==null)continue;
                double v=result.Voltage[part.a.id]-result.Voltage[part.b.id];
                part.Show(v,result.Current[i]);
                if(part.role==ElectricalRole.Ammeter)total=Math.Max(total,Math.Abs(result.Current[i]));
            }
            foreach(var part in parts)
            {
                if(part.role==ElectricalRole.Voltmeter)part.Show(result.Voltage[part.a.id]-result.Voltage[part.b.id],0);
                if(part.role==ElectricalRole.Switch)part.Show(0,0);
            }
            for(int i=0;i<cables.Length;i++)cables[i].current=cableBranches[i]<0||result.ShortCircuit?0:(float)result.Current[cableBranches[i]];
            summary.text=result.ShortCircuit?"CURTO-CIRCUITO · alimentação protegida":result.Powered?
                "CIRCUITO FECHADO · observe a lâmpada e os instrumentos":
                "CIRCUITO ABERTO · conecte os terminais e feche o interruptor";
        }
        void ResetAssembly()
        {
            foreach(var cable in cables)
            {
                foreach(var end in new[]{cable.first,cable.second})
                {
                    if(end.terminal==null)continue;
                    var socket=end.terminal.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
                    var grab=end.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                    if(socket!=null&&grab!=null&&socket.interactionManager!=null)socket.interactionManager.SelectExit((UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor)socket,(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable)grab);
                    end.terminal=null;
                    end.ReturnHome();
                }
            }
            foreach(var part in parts)if(part.role==ElectricalRole.Switch)
            {
                var lever=part.GetComponentInChildren<UnityEngine.XR.Content.Interaction.XRLever>();
                if(lever!=null)lever.value=false;
                part.switchClosed=false;
            }
            Recalculate();
        }
        void OpenLeave(){UIVisibility.Set(leaveDialog,true);UIVisibility.SetInteraction(pages,false);}
        void CancelLeave(){UIVisibility.Set(leaveDialog,false);UIVisibility.SetInteraction(pages,true);}
        void Leave()
        {
            const string hub="Assets/_Project/Hub/Scenes/Hub.unity";
            if(loading)return;
            if(!Application.CanStreamedLevelBeLoaded(hub)){summary.text="Hub indisponível.";CancelLeave();return;}
            loading=true;SceneManager.LoadSceneAsync(hub);
        }
    }
}
