using System;
using UnityEngine;
using Unity.Mathematics;
using Unity.XR.CoreUtils;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Utilities;

public class AffordanceToggle : MonoBehaviour
{
    [Tooltip("Nome exato do filho a ser ligado/desligado.")]
    [SerializeField] private string affordanceChildName = "Affordance Callout";

    private XRGrabInteractable grab;
    private GameObject affordanceGO;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        affordanceGO = FindChildByName(transform, affordanceChildName);

        if (affordanceGO == null)
        {
            Debug.LogWarning($"[GrabAffordanceToggle] Filho '{affordanceChildName}' não encontrado em '{name}'.");
        }
    }

    private void OnEnable()
    {
        // Assina os eventos específicos do XRIT
        grab.firstSelectEntered.AddListener(OnFirstSelectEntered);
        grab.lastSelectExited.AddListener(OnLastSelectExited);
    }

    private void OnDisable()
    {
        // Remove as assinaturas para evitar leaks
        grab.firstSelectEntered.RemoveListener(OnFirstSelectEntered);
        grab.lastSelectExited.RemoveListener(OnLastSelectExited);
    }

    private void OnFirstSelectEntered(SelectEnterEventArgs args)
    {
        if (affordanceGO != null)
            affordanceGO.SetActive(true);
    }

    private void OnLastSelectExited(SelectExitEventArgs args)
    {
        if (affordanceGO != null)
            affordanceGO.SetActive(false);
    }

    /// <summary>
    /// Procura recursivamente um filho pelo nome (inclui inativos).
    /// </summary>
    private GameObject FindChildByName(Transform root, string childName)
    {
        foreach (var t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName)
                return t.gameObject;
        }
        return null;
    }
}
