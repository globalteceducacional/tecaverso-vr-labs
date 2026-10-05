using UnityEngine;

namespace Tecaverso.Labs.ObliqueLaunch
{
    public sealed class TelescopicLaunchBase : MonoBehaviour
    {
        [SerializeField] Transform[] stages;
        [SerializeField] Transform[] collars;
        [SerializeField] Transform platform;
        public void Bind(Transform[] sections, Transform[] rings, Transform top)
        { stages=sections; collars=rings; platform=top; }
        public void SetHeight(float height)
        {
            height=Mathf.Clamp(height,0f,10f);
            for(int i=0;i<stages.Length;i++)
            {
                float segment=height/stages.Length;
                stages[i].gameObject.SetActive(height>.05f);
                stages[i].localPosition=Vector3.up*(segment*i);
                stages[i].localScale=new Vector3(1f,segment,1f);
                collars[i].gameObject.SetActive(height>.15f);
                collars[i].localPosition=Vector3.up*(segment*(i+1)-.04f);
            }
            platform.localPosition=Vector3.up*(height-.06f);
        }
    }
}
