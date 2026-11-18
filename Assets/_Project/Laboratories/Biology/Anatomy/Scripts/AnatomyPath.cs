using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using Tecaverso.Interaction;

namespace Tecaverso.Labs.Biology.Anatomy
{
    // Physiological sequences are painted on the original mesh surfaces, in the
    // body's frame. A single clock owns both the visual phase and the caption.
    [DefaultExecutionOrder(100)]
    public sealed class AnatomyPath : MonoBehaviour
    {
        public Material surfaceMaterial;
        public bool Running { get; private set; }
        public bool InspectionSuspended { get; private set; }
        public bool Visible => started && !InspectionSuspended;
        public int Current { get; private set; } = -1;
        public bool Available => Current >= 0;
        public float Duration => Available ? cycles[Current][^1].end : 0;
        public float Phase => Available ? clock / Duration : 0;
        public int CurrentStage
        {
            get
            {
                if (!started) return -1;
                var steps = cycles[Current];
                for (int i = 0; i < steps.Length - 1; i++)
                    if (clock < steps[i].end) return i;
                return steps.Length - 1;
            }
        }
        public string Legend => !Available ? "Exploração anatômica estática."
            : started ? cycles[Current][CurrentStage].caption : overview[Current];

        // Optional source system permits the lung surfaces to act as translucent
        // exchange regions in the circulatory cycle. Their XR colliders stay off.
        enum Pattern { Sweep, RightHeart, LeftHeart, Hand, Colon }
        sealed class Surface
        {
            public readonly string name;
            public readonly Vector3 direction;
            public readonly Pattern pattern;
            public readonly int sourceSystem;
            public readonly float peakOpacity;
            public MeshFilter filter;
            public Vector4 along, lateral, vertical;
            public Surface(string name, Vector3 direction, Pattern pattern, int sourceSystem, float peakOpacity)
            {
                this.name = name;
                this.direction = direction;
                this.pattern = pattern;
                this.sourceSystem = sourceSystem;
                this.peakOpacity = peakOpacity;
            }
        }
        sealed class Step
        {
            public readonly float end;
            public readonly string caption;
            public readonly Color first, last;
            public readonly float bandWidth;
            public readonly Surface[] surfaces;
            public Step(float end, string caption, Color first, Color last, float bandWidth, params Surface[] surfaces)
            {
                this.end = end;
                this.caption = caption;
                this.first = first;
                this.last = last;
                this.bandWidth = bandWidth;
                this.surfaces = surfaces;
            }
        }

        static readonly string[] overview = {
            "Ar · inspiração e expiração nas vias aéreas e nos pulmões.",
            "Sangue · circuito pulmonar e trajeto sistêmico até a mão esquerda. Cores didáticas.",
            "Digestão · acompanhe o tubo digestório da boca à eliminação.",
            "Sinal sensitivo · da mão direita ao encéfalo. A luz representa atividade nervosa."
        };
        static readonly Color air = new Color(.30f,.83f,.96f,.20f);
        static readonly Color venous = new Color(.22f,.47f,.99f,.30f);
        static readonly Color arterial = new Color(1f,.28f,.23f,.32f);
        static readonly Color food = new Color(.99f,.62f,.13f,.27f);
        static readonly Color signal = new Color(.99f,.80f,.30f,.27f);
        static readonly Vector3 up = Vector3.up, down = Vector3.down;
        static Surface On(string name, Vector3 direction, Pattern pattern = Pattern.Sweep, int sourceSystem = -1, float peak = .90f)
            => new Surface(name, direction, pattern, sourceSystem, peak);

        // Times are seconds in a slowed, qualitative teaching sequence. Surface
        // projections sequence regions of each mesh; they do not identify single
        // vessels, alveoli, axons or a specific intestinal loop.
        readonly Step[][] cycles = {
            new[] {
                new Step(1.5f,"Inspiração · ar entra pelo nariz e segue pela faringe.", air,air,.56f,
                    On("NasalCavity",Vector3.forward),On("Pharynx",down)),
                new Step(3,"Inspiração · da laringe pela traqueia até os brônquios.", air,air,.48f,
                    On("Larynx_Internal",down),On("Larynx_External",down),On("Trachea",down)),
                new Step(5,"Inspiração · os brônquios distribuem o ar aos dois pulmões.", air,air,.72f,
                    On("Bronchi",down),On("Lung_Left",down,peak:.65f),On("Lung_Right",down,peak:.65f)),
                new Step(7,"Expiração · o ar sai dos pulmões em direção aos brônquios.", air,air,.72f,
                    On("Lung_Left",up,peak:.65f),On("Lung_Right",up,peak:.65f),On("Bronchi",up)),
                new Step(8.5f,"Expiração · o ar sobe pela traqueia e passa pela laringe.", air,air,.48f,
                    On("Trachea",up),On("Larynx_Internal",up),On("Larynx_External",up)),
                new Step(10,"Expiração · da faringe para o exterior pelo nariz.", air,air,.56f,
                    On("Pharynx",up),On("NasalCavity",Vector3.back))
            },
            new[] {
                new Step(1.7f,"Coração direito · sangue com menos oxigênio segue para os pulmões.", venous,venous,.74f,
                    On("Heart",up,Pattern.RightHeart,peak:.66f)),
                new Step(4.3f,"Pulmões · o sangue recebe oxigênio e retorna ao coração esquerdo.", venous,arterial,.86f,
                    On("Lung_Left",Vector3.right,sourceSystem:0,peak:.68f),On("Lung_Right",Vector3.left,sourceSystem:0,peak:.68f)),
                new Step(5.8f,"Coração esquerdo · o sangue oxigenado segue para a circulação sistêmica.", arterial,arterial,.74f,
                    On("Heart",up,Pattern.LeftHeart,peak:.66f)),
                new Step(8.4f,"Artérias · o sangue segue em direção à mão esquerda.", arterial,arterial,.48f,
                    On("ArterialSystem_Arm_Left",down)),
                new Step(10,"Tecidos da mão · o sangue fornece oxigênio; a rede venosa o recolhe.", arterial,venous,.84f,
                    On("ArterialSystem_Arm_Left",Vector3.right,Pattern.Hand),On("VenousSystem_Arm_Left",Vector3.right,Pattern.Hand)),
                new Step(13,"Veias · o sangue com menos oxigênio retorna ao coração direito.", venous,venous,.48f,
                    On("VenousSystem_Arm_Left",up))
            },
            new[] {
                new Step(1.8f,"Boca e faringe · o alimento é deglutido.", food,food,.64f,
                    On("OralCavity",Vector3.forward),On("Pharynx",down)),
                new Step(3,"Esôfago · o conteúdo entra no segmento superior.", food,food,.60f,On("Esophagus_Head",down)),
                new Step(5.2f,"Esôfago · o conteúdo segue pelo tórax até o estômago.", food,food,.43f,On("Esophagus_Torso",down)),
                new Step(7.6f,"Estômago · o conteúdo é misturado e segue para o intestino.", food,food,.80f,On("Stomach",down,peak:.70f)),
                new Step(10.9f,"Intestino delgado · digestão e absorção nas alças.", food,food,.64f,On("Small_Inestines",down,peak:.68f)),
                new Step(15,"Intestino grosso · pelos cólons ascendente, transverso e descendente até a eliminação.", food,food,.36f,
                    On("Colon",down,Pattern.Colon,peak:.66f))
            },
            new[] {
                new Step(3.5f,"Toque na mão direita · o sinal sensitivo sobe pelos nervos do membro.", signal,signal,.40f,
                    On("NervousSystem_Arm_Right",up)),
                new Step(5.2f,"Medula cervical · a informação sensitiva ascende em direção ao tronco encefálico.", signal,signal,.54f,
                    On("SpinalCord_Neck",up)),
                new Step(6.3f,"Tronco encefálico · a via deste exemplo cruza para o lado esquerdo.", signal,signal,.66f,
                    On("Brainstem_Thalamus_Right",up),On("Brainstem_Thalamus_Left",up)),
                new Step(8,"Encéfalo · o lobo parietal esquerdo indica a região do córtex sensitivo.", signal,signal,.70f,
                    On("Lobe_Parietal_Left",up,peak:.62f))
            }
        };

        static readonly int Travel = Shader.PropertyToID("_Travel");
        static readonly int Lateral = Shader.PropertyToID("_Lateral");
        static readonly int Vertical = Shader.PropertyToID("_Vertical");
        static readonly int Wave = Shader.PropertyToID("_Wave");
        static readonly int Tint = Shader.PropertyToID("_Tint");
        static readonly int Arrival = Shader.PropertyToID("_Arrival");
        MaterialPropertyBlock properties;
        bool started;
        float clock;

        void Awake()
        {
            properties = new MaterialPropertyBlock();
            var sources = new Dictionary<(int,string),MeshFilter>();
            var lab = GetComponentInParent<AnatomyLab>();
            foreach (var organ in lab.organs)
                foreach (var renderer in organ.surfaces)
                    sources[(organ.system,renderer.name)] = renderer.GetComponent<MeshFilter>();
            for (int i = 0; i < cycles.Length; i++)
                foreach (var step in cycles[i])
                    foreach (var surface in step.surfaces)
                    {
                        int system = surface.sourceSystem < 0 ? i : surface.sourceSystem;
                        surface.filter = sources[(system,surface.name)];
                        var meshToBody = transform.worldToLocalMatrix * surface.filter.transform.localToWorldMatrix;
                        var bounds = surface.filter.sharedMesh.bounds;
                        surface.along = Plane(meshToBody, bounds, surface.direction);
                        surface.lateral = Plane(meshToBody, bounds, Vector3.right);
                        surface.vertical = Plane(meshToBody, bounds, Vector3.up);
                    }
        }

        static Vector4 Plane(Matrix4x4 meshToBody, Bounds bounds, Vector3 bodyAxis)
        {
            Vector3 axis = meshToBody.transpose.MultiplyVector(bodyAxis);
            float span = Mathf.Abs(axis.x) * bounds.size.x + Mathf.Abs(axis.y) * bounds.size.y + Mathf.Abs(axis.z) * bounds.size.z;
            axis /= Mathf.Max(span,.000001f);
            return new Vector4(axis.x,axis.y,axis.z,.5f - Vector3.Dot(bounds.center,axis));
        }

        public void Choose(int system)
        {
            started = Running = InspectionSuspended = false;
            clock = 0;
            Current = system >= 0 && system < cycles.Length ? system : -1;
        }
        public void Toggle()
        {
            if (!Available || InspectionSuspended) return;
            started = true;
            Running = !Running;
        }
        public void SetInspectionSuspended(bool suspended) => InspectionSuspended = suspended;
        void OnDisable() => Choose(Current);

        public void Advance(float deltaSeconds)
        {
            if (Running && Visible && deltaSeconds > 0)
                clock = Mathf.Repeat(clock + deltaSeconds,Duration);
        }

        void LateUpdate()
        {
            if (!Visible) return;
            Advance(Time.unscaledDeltaTime);

            const float overlap = .18f;
            float start = 0;
            foreach (var step in cycles[Current])
            {
                if (clock >= start-overlap && clock <= step.end+overlap)
                {
                    float t = Mathf.InverseLerp(start,step.end,clock);
                    float fade = Mathf.SmoothStep(0,1,Mathf.InverseLerp(start-overlap,start+overlap,clock))
                               * Mathf.SmoothStep(0,1,Mathf.InverseLerp(step.end+overlap,step.end-overlap,clock));
                    Draw(step,t,fade);
                }
                start = step.end;
            }
        }

        void Draw(Step step,float t,float fade)
        {
            bool linear = QualitySettings.activeColorSpace == ColorSpace.Linear;
            Color tint = linear ? step.first.linear : step.first;
            Color arrival = linear ? step.last.linear : step.last;
            float width = step.bandWidth;
            // Only the decorative bright crest is reduced; the scientific movement stays visible.
            if (MotionPreferences.Reduced) width *= 1.35f;
            foreach (var surface in step.surfaces)
            {
                properties.SetVector(Travel,surface.along);
                properties.SetVector(Lateral,surface.lateral);
                properties.SetVector(Vertical,surface.vertical);
                properties.SetVector(Tint,tint);
                arrival.a = surface.peakOpacity;
                properties.SetVector(Arrival,arrival);
                properties.SetVector(Wave,new Vector4(Mathf.Lerp(-.03f,1.06f,t),fade,width,(float)surface.pattern));
                var draw = new RenderParams(surfaceMaterial) {
                    layer = surface.filter.gameObject.layer,
                    shadowCastingMode = ShadowCastingMode.Off, receiveShadows = false,
                    lightProbeUsage = LightProbeUsage.Off, reflectionProbeUsage = ReflectionProbeUsage.Off,
                    motionVectorMode = MotionVectorGenerationMode.ForceNoMotion, matProps = properties
                };
                Mesh mesh = surface.filter.sharedMesh;
                for (int sub = 0; sub < mesh.subMeshCount; sub++)
                    Graphics.RenderMesh(draw,mesh,sub,surface.filter.transform.localToWorldMatrix);
            }
        }
    }
}
