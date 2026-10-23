using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class CircuitPart : MonoBehaviour
    {
        public ElectricalRole role;
        public CircuitTerminal a,b;
        public CircuitTerminal[] additionalTerminals;
        public float resistance=10;
        public bool switchClosed;
        public TMP_Text display;
        public Renderer lampRenderer;
        public Light lampLight;
        public CircuitLab lab;
        public void ToggleSwitch(){if(role!=ElectricalRole.Switch)return;switchClosed=!switchClosed;lab.Recalculate();}
        public void CloseSwitch(){if(role!=ElectricalRole.Switch||switchClosed)return;switchClosed=true;lab.Recalculate();}
        public void OpenSwitch(){if(role!=ElectricalRole.Switch||!switchClosed)return;switchClosed=false;lab.Recalculate();}
        public void Show(double volts,double amps)
        {
            if(display!=null)
            {
                switch(role)
                {
                    case ElectricalRole.Ammeter:display.text=$"{amps:0.00} A";break;
                    case ElectricalRole.Voltmeter:display.text=$"{volts:+0.00;-0.00;0.00} V";break;
                    case ElectricalRole.Resistor:display.text=$"{resistance:0} Ω\n{volts:0.0} V · {amps:0.00} A";break;
                    case ElectricalRole.Switch:display.text=switchClosed?"FECHADO":"ABERTO";break;
                }
            }
            if(role==ElectricalRole.Lamp)
            {
                float brightness=Mathf.Clamp01((float)(volts*amps)/8);
                if(lampLight!=null){lampLight.enabled=brightness>.01f;lampLight.intensity=brightness*3;}
                if(lampRenderer!=null)
                {
                    var block=new MaterialPropertyBlock();lampRenderer.GetPropertyBlock(block);
                    block.SetColor("_EmissionColor",new Color(1,.65f,.22f)*brightness*2);
                    lampRenderer.SetPropertyBlock(block);
                }
            }
        }
    }
}
