using UnityEngine;
using TMPro;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class ProjectileView : MonoBehaviour
    {
        [SerializeField] VectorArrowView resultArrow, horizontalArrow, verticalArrow;
        public void Bind(VectorArrowView result, VectorArrowView horizontal, VectorArrowView vertical)
        { resultArrow = result; horizontalArrow = horizontal; verticalArrow = vertical; }

        VectorArrowView gravityArrow;
        TMP_Text[] labels;
        FlightSample lastSample;
        float lastGravity;
        bool hasSample, visible=true;

        void EnsureAnnotations()
        {
            if(labels!=null) return;
            gravityArrow=Instantiate(verticalArrow,transform);
            gravityArrow.name="Gravity Vector";
            var tint=new MaterialPropertyBlock(); tint.SetColor("_BaseColor",Color.yellow);
            foreach(var renderer in gravityArrow.GetComponentsInChildren<Renderer>(true)) renderer.SetPropertyBlock(tint);
            labels=new TMP_Text[4];
            for(int i=0;i<4;i++)
            {
                var go=new GameObject("Vector Value "+i,typeof(TextMeshPro));
                go.transform.SetParent(transform,false);
                var text=go.GetComponent<TextMeshPro>();
                text.fontSize=2.2f; text.alignment=TextAlignmentOptions.Center;
                text.rectTransform.sizeDelta=new Vector2(2.4f,.35f);
                labels[i]=text;
            }
        }

        public void Show(in FlightSample sample, float gravity=9.81f)
        {
            EnsureAnnotations();
            lastSample=sample; lastGravity=gravity; hasSample=true;
            transform.position = sample.Position;
            if(!visible){HideVectors();return;}
            resultArrow.SetVector(sample.Velocity);
            horizontalArrow.SetVector(new Vector3(sample.Velocity.x, 0f, 0f));
            verticalArrow.SetVector(new Vector3(0f, sample.Velocity.y, 0f));
            gravityArrow.SetVector(Vector3.down*gravity);
            Label(0,resultArrow,$"V = {sample.Velocity.magnitude:0.00} m/s",new Color32(40,78,160,255),new Vector3(.2f,.25f,0));
            Label(1,horizontalArrow,$"Vx = {sample.Velocity.x:0.00} m/s",new Color32(62,211,156,255),new Vector3(.3f,-.2f,0));
            Label(2,verticalArrow,$"Vy = {sample.Velocity.y:0.00} m/s",new Color32(217,54,69,255),new Vector3(-.8f,.1f,0));
            Label(3,gravityArrow,$"g = {gravity:0.00} m/s²",Color.yellow,new Vector3(.8f,-.2f,0));
        }

        void Label(int index,VectorArrowView arrow,string value,Color color,Vector3 offset)
        {
            var label=labels[index]; label.gameObject.SetActive(true); label.text=value; label.color=color;
            label.transform.rotation=Quaternion.identity;
            label.transform.localScale=Vector3.one/Mathf.Max(.001f,transform.lossyScale.x);
            label.transform.position=(arrow.gameObject.activeSelf ? arrow.transform.Find("Head").position : transform.position)+offset;
        }

        public void SetVectorsVisible(bool value)
        { visible=value; if(!value) HideVectors(); else if(hasSample) Show(lastSample,lastGravity); }

        public void HideVectors()
        {
            if (resultArrow != null) resultArrow.gameObject.SetActive(false);
            if (horizontalArrow != null) horizontalArrow.gameObject.SetActive(false);
            if (verticalArrow != null) verticalArrow.gameObject.SetActive(false);
            if (gravityArrow != null) gravityArrow.gameObject.SetActive(false);
            if(labels!=null) foreach(var label in labels) label.gameObject.SetActive(false);
        }
    }
}
