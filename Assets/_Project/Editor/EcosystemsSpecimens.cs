using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Tecaverso.Editor
{
    // Offline authoring. Each specimen is a single vertex-painted mesh; the ray selects its silhouette.
    internal sealed class EcosystemsSpecimens
    {
        readonly List<Vector3> positions = new();
        readonly List<Color> colors = new();
        readonly List<int> indices = new();
        static Vector3 V(float x,float y,float z) => new(x,y,z);
        static Color C(int rgb) => ((Color)new Color32((byte)(rgb>>16),(byte)(rgb>>8),(byte)rgb,255)).linear;

        int Vertex(Vector3 p, Color color)
        {
            int i=positions.Count; positions.Add(p); colors.Add(color); return i;
        }
        void Tri(int a,int b,int c) { indices.Add(a); indices.Add(b); indices.Add(c); }
        void Quad(int a,int b,int c,int d) { Tri(a,b,c); Tri(a,c,d); }

        // Follow a varying-radius spine. Rings are built in a local orthonormal frame,
        // so diagonal shins, antennae and tapered abdomens keep their cross-section.
        void Sweep(Vector3[] path, float[] radius, int rgb, float flatten=1, int sides=10)
        {
            int start=positions.Count;
            Color pigment=C(rgb);
            Vector3 reference=Mathf.Abs(Vector3.Dot((path[^1]-path[0]).normalized,Vector3.up))>.88f?Vector3.forward:Vector3.up;
            for(int i=0;i<path.Length;i++)
            {
                Vector3 axis=(path[Mathf.Min(i+1,path.Length-1)]-path[Mathf.Max(i-1,0)]).normalized;
                Vector3 left=Vector3.Cross(axis,reference).normalized;
                Vector3 up=Vector3.Cross(left,axis);
                for(int j=0;j<sides;j++)
                {
                    float a=j*2*Mathf.PI/sides;
                    Vertex(path[i]+radius[i]*(left*Mathf.Cos(a)+up*(Mathf.Sin(a)*flatten)),pigment);
                    if(i==0)continue;
                    int l=start+(i-1)*sides+j, n=start+(i-1)*sides+(j+1)%sides;
                    Quad(l,l+sides,n+sides,n);
                }
            }
            int first=Vertex(path[0],pigment), last=Vertex(path[^1],pigment);
            for(int j=0;j<sides;j++)
            {
                int n=(j+1)%sides;
                Tri(first,start+j,start+n);
                int end=start+(path.Length-1)*sides;
                Tri(last,end+n,end+j);
            }
        }
        void Oval(Vector3 at,Vector3 size,int rgb, float pitch=0, float yaw=0)
        {
            const int slices=16, rings=9;
            int start=positions.Count;
            Quaternion rotate=Quaternion.Euler(pitch,yaw,0);
            for(int r=0;r<=rings;r++)
            {
                float lat=Mathf.PI*r/rings;
                for(int j=0;j<slices;j++)
                {
                    float lon=2*Mathf.PI*j/slices;
                    Vector3 p=V(Mathf.Cos(lon)*Mathf.Sin(lat),Mathf.Cos(lat),Mathf.Sin(lon)*Mathf.Sin(lat));
                    Vertex(at+rotate*Vector3.Scale(size,p),C(rgb));
                    if(r==0)continue;
                    int a=start+(r-1)*slices+j,b=start+(r-1)*slices+(j+1)%slices;
                    Quad(a,b,b+slices,a+slices);
                }
            }
        }
        void Blade(Vector3 root, Vector3 tip, float width, int rgb)
        {
            int start=positions.Count;
            Vector3 across=Vector3.Cross(Vector3.up,tip-root).normalized*width;
            for(int i=0;i<=6;i++)
            {
                float t=i/6f, span=Mathf.Sin(Mathf.PI*t)*.72f+(1-t)*.28f;
                var p=Vector3.Lerp(root,tip,t);
                p.y+=Mathf.Sin(t*Mathf.PI)*Vector3.Distance(root,tip)*.18f;
                Vertex(p-across*span,C(rgb)*.76f); Vertex(p+Vector3.up*(width*.42f*span),C(rgb)); Vertex(p+across*span,C(rgb)*.88f);
                if(i==0)continue;
                int a=start+(i-1)*3;
                Quad(a,a+1,a+4,a+3); Quad(a+1,a+2,a+5,a+4);
            }
        }
        void Grass()
        {
            int[] greens={0x4c883d,0x75ad46,0x326447,0x94ad50,0x529746};
            for(int i=0;i<36;i++)
            {
                float a=i*2.4f,s=Mathf.Sin(a),c=Mathf.Cos(a),r=.018f+.015f*(i%4);
                var root=V(c*r,0,s*r);
                Blade(root,root+V(c*(.045f+.014f*(i%5)),.11f+.026f*(i%6),s*(.05f+.013f*(i%5))),.0085f,greens[i%5]);
            }
            for(int i=0;i<4;i++)
            {
                Vector3 p=V(-.045f+.029f*i,0,.018f*(i%2));
                Vector3 q=p+V(.028f,.25f-.013f*i,.022f*(i%3));
                Sweep(new[]{p,(p+q)*.5f,q},new[]{.003f,.002f,.001f},0x8d9c43);
                for(int j=0;j<8;j++)
                {
                    Vector3 b=Vector3.Lerp(p,q,.72f+j*.036f);
                    Oval(b+V((j%2==0?1:-1)*.004f,0,0),V(.004f,.009f,.003f),0xcbad62);
                }
            }
        }
        void Grasshopper()
        {
            const int back=0x6d9238, pale=0xc3b65f, dark=0x344829;
            Sweep(new[]{V(-.16f,.065f,0),V(-.125f,.078f,0),V(-.06f,.09f,0),V(.025f,.103f,0),V(.072f,.106f,0)},new[]{.001f,.018f,.028f,.037f,.026f},back,.87f);
            Oval(V(.038f,.107f,0),V(.053f,.041f,.043f),0x7ba343);
            Oval(V(.089f,.108f,0),V(.037f,.047f,.039f),0x849e43);
            foreach(int d in new[]{-1,1})
            {
                Oval(V(.099f,.123f,d*.032f),V(.015f,.014f,.009f),0x543f26);
                Oval(V(.102f,.128f,d*.039f),V(.004f,.004f,.003f),0xe2d9a6);
                var tip=V(-.159f,.079f,.019f*d); var shoulder=V(.030f,.124f,.029f*d);
                Sweep(new[]{tip,V(-.11f,.09f,.031f*d),V(-.057f,.102f,.036f*d),shoulder},new[]{.001f,.014f,.021f,.014f},0x9aa64f,.42f);
                Sweep(new[]{tip,V(-.048f,.107f,.044f*d),shoulder},new[]{.0012f,.0016f,.001f},pale);
                var hip=V(.001f,.074f,.027f*d); var knee=V(-.105f,.139f,.057f*d); var ankle=V(-.146f,.016f,.078f*d);
                Sweep(new[]{hip,Vector3.Lerp(hip,knee,.27f),knee},new[]{.011f,.023f,.008f},back,.78f);
                Sweep(new[]{knee,ankle,ankle+V(.047f,-.006f,.005f*d)},new[]{.007f,.0035f,.001f},0xa97841);
                for(int j=1;j<6;j++)
                {
                    var p=Vector3.Lerp(knee,ankle,j*.13f);
                    Sweep(new[]{p,p+V(-.009f,.006f,.002f*d)},new[]{.0025f,0},dark,1,5);
                }
                for(int j=0;j<2;j++)
                {
                    var p=V(.033f+.042f*j,.080f,.029f*d);
                    Sweep(new[]{p,p+V(.015f,-.016f,.021f*d),p+V(.004f,-.061f,.039f*d),p+V(.03f,-.070f,.049f*d)},new[]{.006f,.0055f,.0026f,.001f},dark);
                }
                Sweep(new[]{V(.108f,.141f,.012f*d),V(.143f,.166f,.019f*d),V(.185f,.18f,.035f*d)},new[]{.0024f,.0017f,.0007f},dark);
            }
        }
        void Toad()
        {
            const int olive=0x808548, deep=0x55643b, ochre=0xc5ad68;
            Oval(V(-.028f,.084f,0),V(.104f,.072f,.084f),olive);
            Oval(V(.064f,.1f,0),V(.081f,.055f,.078f),olive);
            Oval(V(.088f,.078f,0),V(.060f,.031f,.07f),ochre);
            for(int d=-1;d<=1;d+=2)
            {
                Oval(V(-.083f,.052f,.068f*d),V(.078f,.040f,.041f),deep,0,-18*d);
                Sweep(new[]{V(-.118f,.045f,.086f*d),V(-.071f,.021f,.105f*d),V(-.017f,.015f,.101f*d)},new[]{.023f,.016f,.009f},olive);
                Sweep(new[]{V(.035f,.087f,.064f*d),V(.045f,.039f,.092f*d),V(.091f,.018f,.098f*d)},new[]{.018f,.012f,.009f},deep);
                for(int j=0;j<4;j++)
                {
                    var hand=V(.09f,.017f,.098f*d);
                    var foot=V(-.016f,.015f,.101f*d);
                    var toe=V(.024f-.002f*j,-.008f,(j-1.25f)*.009f*d);
                    Sweep(new[]{hand,hand+toe},new[]{.0042f,.001f},ochre,1,6);
                    Sweep(new[]{foot,foot+toe},new[]{.004f,.001f},ochre,1,6);
                }
                Oval(V(.073f,.142f,.042f*d),V(.028f,.028f,.023f),deep);
                Oval(V(.084f,.147f,.055f*d),V(.019f,.017f,.014f),0xe4b950);
                Oval(V(.088f,.148f,.066f*d),V(.013f,.0042f,.004f),0x252e27);
                Oval(V(.091f,.153f,.067f*d),V(.0035f,.003f,.002f),0xffedc4);
                Oval(V(.038f,.109f,.073f*d),V(.014f,.013f,.005f),0x776d43);
                Sweep(new[]{V(.137f,.091f,.025f*d),V(.116f,.092f,.055f*d),V(.071f,.086f,.073f*d)},new[]{.001f,.002f,.002f},0x524d34);
                for(int j=0;j<8;j++)
                {
                    float x=-.103f+j*.017f;
                    Oval(V(x,.129f+.021f*Mathf.Sin(j*.43f),(.025f+.012f*(j%2))*d),V(.007f,.006f,.007f),0x606e3c);
                }
            }
        }
        void Decomposition()
        {
            Sweep(new[]{V(-.14f,.029f,0),V(-.09f,.038f,0),V(.082f,.027f,0),V(.139f,.031f,.012f)},new[]{.030f,.037f,.031f,.018f},0x705140);
            for(int i=0;i<5;i++)
            {
                float h=.08f+.014f*(i%3);
                var basePoint=V(-.1f+.046f*i,.025f,-.031f+.024f*(i%3));
                var top=basePoint+V(.009f,h,.005f);
                Sweep(new[]{basePoint,Vector3.Lerp(basePoint,top,.6f),top}, new[]{.012f,.008f,.010f},0xe0c59a);
                float w=.036f+.005f*(i%3);
                Oval(top,V(w,.014f,w*.90f),0xd2b78f);
                Oval(top+Vector3.up*.008f,V(w,.025f,w*.90f),0xaf6638);
            }
            Blade(V(.018f,.013f,-.09f),V(-.080f,.019f,-.081f),.029f,0xb98b43);
        }
        void MeadowBorder()
        {
            // Pigment on the soil gives the bed depth without a repeating texture or a moving effect.
            const int columns=36,rows=16;
            int start=positions.Count;
            for(int z=0;z<=rows;z++)
            {
                float depth=Mathf.Lerp(.008f,1.02f,z/(float)rows);
                float halfWidth=Mathf.Lerp(1.17f,1.27f,Mathf.Clamp01(Mathf.Min(z,rows-z)/2f));
                for(int x=0;x<=columns;x++)
                {
                    float lateral=Mathf.Lerp(-halfWidth,halfWidth,x/(float)columns);
                    float noise=Mathf.PerlinNoise(8f+lateral*3.2f,4f+depth*4.5f);
                    Color soil=Color.Lerp(C(0x465e48),C(0x718362),noise);
                    Vertex(V(lateral,.0028f,depth),soil);
                    if(z>0 && x>0)
                    {
                        int p=start+z*(columns+1)+x;
                        Quad(p-columns-2,p-1,p,p-columns-1);
                    }
                }
            }
            var seeds=new System.Random(5201);
            float Range(float a,float b)=>Mathf.Lerp(a,b,(float)seeds.NextDouble());
            foreach(var center in new[]{V(-1.11f,0,.84f),V(-.59f,0,.96f),V(.15f,0,.97f),V(.93f,0,.93f),V(1.17f,0,.50f)})
            {
                for(int i=0;i<8;i++)
                {
                    var p=center+V(Range(-.075f,.075f),.002f,Range(-.04f,.04f));
                    float w=Range(.012f,.04f);
                    Oval(p,V(w,Range(.008f,.023f),w*Range(.61f,1.1f)),i%3==0?0x74756b:0x829377);
                    if(i%3!=0)continue;
                    for(int j=0;j<9;j++)
                    {
                        float a=j*2.4f;
                        Blade(p,p+V(.03f*Mathf.Cos(a),Range(.03f,.075f),.035f*Mathf.Sin(a)),.004f,0x859a51);
                    }
                }
            }
        }
        public static Mesh Get(string name,int kind,bool update=false)
        {
            string path=EcosystemsAuthoring.Folder+"/Art/"+name+".asset";
            var stored=AssetDatabase.LoadAssetAtPath<Mesh>(path);
            if(stored && !update)return stored;
            var b=new EcosystemsSpecimens();
            switch(kind){case 0:b.Grass();break;case 1:b.Grasshopper();break;case 2:b.Toad();break;case 3:b.Decomposition();break;case 4:b.MeadowBorder();break;}
            var mesh=stored?stored:new Mesh{name=name};
            if(stored)mesh.Clear();
            mesh.SetVertices(b.positions); mesh.SetColors(b.colors); mesh.SetTriangles(b.indices,0);
            mesh.RecalculateNormals(); mesh.RecalculateBounds();
            if(stored)EditorUtility.SetDirty(mesh);else AssetDatabase.CreateAsset(mesh,path);
            return mesh;
        }
    }
}
