using UnityEngine;

namespace Tecaverso.Labs.Thermology
{
    [RequireComponent(typeof(ParticleSystem))]
    public sealed class GasParticles : MonoBehaviour
    {
        public ThermologyLab lab;
        public float radius=.70f;
        public bool reduced;
        const int Count=96;
        ParticleSystem system;
        readonly ParticleSystem.Particle[] particles=new ParticleSystem.Particle[Count];
        readonly Vector3[] positions=new Vector3[Count], velocities=new Vector3[Count];
        readonly float[] flash=new float[Count];
        void Start()
        {
            system=GetComponent<ParticleSystem>();system.Stop(true,ParticleSystemStopBehavior.StopEmittingAndClear);
            var main=system.main;main.simulationSpace=ParticleSystemSimulationSpace.Local;main.maxParticles=Count;
            var emission=system.emission;emission.enabled=false;
            ResetCloud();
        }
        public void ResetCloud()
        {
            var random=new System.Random(519);
            for(int i=0;i<Count;i++)
            {
                float angle=(float)random.NextDouble()*Mathf.PI*2, r=Mathf.Sqrt((float)random.NextDouble())*(radius-.04f);
                positions[i]=new Vector3(Mathf.Cos(angle)*r,.04f+(float)random.NextDouble()*(lab.GasHeight-.08f),Mathf.Sin(angle)*r);
                velocities[i]=new Vector3((float)random.NextDouble()-.5f,(float)random.NextDouble()-.5f,(float)random.NextDouble()-.5f).normalized;
                flash[i]=0;
            }
        }
        void LateUpdate()
        {
            if(system==null||lab==null)return;
            float dt=lab.Running?Mathf.Min(Time.deltaTime,.04f):0, h=lab.GasHeight-.035f;
            float speed=.60f*Mathf.Sqrt((float)lab.Model.Temperature/300f);
            var tint=Color.Lerp(new Color(.20f,.73f,1),new Color(1,.36f,.13f),Mathf.InverseLerp(250,450,(float)lab.Model.Temperature));
            for(int i=0;i<Count;i++)
            {
                var p=positions[i]+velocities[i]*speed*dt;bool hit=false;
                if(p.y>h){p.y=h;velocities[i].y=-Mathf.Abs(velocities[i].y);hit=true;}
                if(p.y<.035f){p.y=.035f;velocities[i].y=Mathf.Abs(velocities[i].y);hit=true;}
                var radial=new Vector3(p.x,0,p.z);
                if(radial.magnitude>radius-.035f){var normal=radial.normalized;p.x=normal.x*(radius-.035f);p.z=normal.z*(radius-.035f);velocities[i]=Vector3.Reflect(velocities[i],normal);hit=true;}
                positions[i]=p;flash[i]=hit?.12f:Mathf.Max(0,flash[i]-dt);
                particles[i].position=p;particles[i].startSize=flash[i]>0?.075f:.055f;
                particles[i].startColor=flash[i]>0?Color.white:tint;
                particles[i].startLifetime=particles[i].remainingLifetime=1000;
            }
            system.SetParticles(particles,reduced?32:Count);
        }
    }
}
