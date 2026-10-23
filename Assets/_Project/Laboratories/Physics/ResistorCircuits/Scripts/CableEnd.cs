using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

namespace Tecaverso.Labs.ResistorCircuits
{
    public sealed class CableEnd : MonoBehaviour
    {
        public CircuitTerminal terminal;
        Vector3 homePosition;
        Quaternion homeRotation;
        void Awake(){homePosition=transform.localPosition;homeRotation=transform.localRotation;}
        public void ReturnHome()
        {
            transform.localPosition=homePosition;transform.localRotation=homeRotation;
            var body=GetComponent<Rigidbody>();if(body!=null){body.linearVelocity=Vector3.zero;body.angularVelocity=Vector3.zero;}
        }
    }
}
