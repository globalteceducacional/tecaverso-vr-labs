using TMPro;
using UnityEngine;

public class ObjectTransformDisplay : MonoBehaviour
{
    [Header("Referências de Texto (TextMeshProUGUI)")]
    [SerializeField] private TextMeshProUGUI positionText;
    [SerializeField] private TextMeshProUGUI rotationText;
    [SerializeField] private TextMeshProUGUI scaleText;

    [Header("Configuração")]
    public int decimalPrecision = 0;

    private string format;

    private void Start()
    {
        // Define o formato conforme o enum escolhido
        format = "F" + ((int)decimalPrecision).ToString();
    }

    private void Update()
    {
        Vector3 pos = transform.position;
        Vector3 rot = transform.rotation.eulerAngles;
        Vector3 scl = transform.localScale;

        if (positionText != null)
            positionText.text = $"Posição: ({pos.x.ToString(format)}, {pos.y.ToString(format)}, {pos.z.ToString(format)})";

        if (rotationText != null)
            rotationText.text = $"Rotação: ({rot.x.ToString(format)}, {rot.y.ToString(format)}, {rot.z.ToString(format)})";

        if (scaleText != null)
            scaleText.text = $"Escala: ({scl.x.ToString(format)}, {scl.y.ToString(format)}, {scl.z.ToString(format)})";
    }
}
