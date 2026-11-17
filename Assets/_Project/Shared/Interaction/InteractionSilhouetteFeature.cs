using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Tecaverso.Interaction
{
    public sealed class InteractionSilhouetteFeature : ScriptableRendererFeature
    {
        public Shader silhouetteShader;
        Material material;
        SilhouettePass pass;
        public override void Create()
        {
            CoreUtils.Destroy(material);
            if(silhouetteShader)material=CoreUtils.CreateEngineMaterial(silhouetteShader);
            pass=new SilhouettePass{renderPassEvent=RenderPassEvent.BeforeRenderingTransparents};
        }
        public override void AddRenderPasses(ScriptableRenderer renderer,ref RenderingData renderingData)
        {
            if(!material || renderingData.cameraData.cameraType!=CameraType.Game)return;
            bool active=false;
            foreach(var state in XRInteractionMotion.Instances)if(state && state.Amount>.001f && state.Surfaces.Count>0){active=true;break;}
            if(!active)return;
            pass.material=material;pass.ConfigureInput(ScriptableRenderPassInput.Depth);renderer.EnqueuePass(pass);
        }
        protected override void Dispose(bool disposing){CoreUtils.Destroy(material);}
        sealed class SilhouettePass : ScriptableRenderPass
        {
            public Material material;
            struct Draw {public Renderer renderer;public int submeshes;public Color color;}
            sealed class MaskData {public Material material;public List<Draw> draws;}
            sealed class EdgeData {public Material material;public TextureHandle mask;}
            static readonly int Tint=Shader.PropertyToID("_InteractionTint");
            static readonly int Visibility=Shader.PropertyToID("_InteractionVisibilityTexture");
            public override void RecordRenderGraph(RenderGraph graph,ContextContainer frameData)
            {
                var resources=frameData.Get<UniversalResourceData>();var camera=frameData.Get<UniversalCameraData>().camera;
                if(!resources.cameraDepthTexture.IsValid())return;
                var draws=new List<Draw>();
                for(int priority=0;priority<2;priority++)foreach(var state in XRInteractionMotion.Instances)
                {
                    if(!state || state.Amount<=.001f || state.PersistentSelected!=(priority==1))continue;
                    foreach(var r in state.Surfaces)
                    {
                        if(!r || !r.enabled || !r.gameObject.activeInHierarchy || r.forceRenderingOff || (camera.cullingMask&(1<<r.gameObject.layer))==0)continue;
                        var mesh=r is SkinnedMeshRenderer skinned?skinned.sharedMesh:r.GetComponent<MeshFilter>()?.sharedMesh;
                        if(mesh)draws.Add(new Draw{renderer=r,submeshes=mesh.subMeshCount,color=state.HighlightColor});
                    }
                }
                if(draws.Count==0)return;
                var descriptor=resources.activeColorTexture.GetDescriptor(graph);
                descriptor.name="Interaction silhouette mask";descriptor.colorFormat=GraphicsFormat.R8G8B8A8_UNorm;
                descriptor.depthBufferBits=DepthBits.None;descriptor.msaaSamples=MSAASamples.None;
                descriptor.bindTextureMS=false;descriptor.memoryless=RenderTextureMemoryless.None;
                descriptor.clearBuffer=true;descriptor.clearColor=Color.clear;descriptor.filterMode=FilterMode.Bilinear;
                var mask=graph.CreateTexture(descriptor);
                descriptor.name="Interaction visible samples";descriptor.colorFormat=GraphicsFormat.R8_UNorm;
                var visibility=graph.CreateTexture(descriptor);
                using(var builder=graph.AddRasterRenderPass<MaskData>("Interaction / visible surfaces",out var data))
                {
                    data.material=material;data.draws=draws;
                    builder.UseTexture(resources.cameraDepthTexture,AccessFlags.Read);builder.SetRenderAttachment(mask,0,AccessFlags.Write);
                    builder.SetRenderAttachment(visibility,1,AccessFlags.Write);
                    builder.SetGlobalTextureAfterPass(visibility,Visibility);
                    builder.AllowGlobalStateModification(true);
                    builder.SetRenderFunc(static (MaskData d,RasterGraphContext context)=>
                    {
                        foreach(var draw in d.draws)
                        {
                            if(!draw.renderer)continue;context.cmd.SetGlobalColor(Tint,draw.color);
                            for(int s=0;s<draw.submeshes;s++)context.cmd.DrawRenderer(draw.renderer,d.material,s,0);
                        }
                    });
                }
                using(var builder=graph.AddRasterRenderPass<EdgeData>("Interaction / silhouette edge",out var data))
                {
                    data.material=material;data.mask=mask;builder.UseTexture(mask,AccessFlags.Read);
                    builder.UseTexture(visibility,AccessFlags.Read);
                    builder.SetRenderAttachment(resources.activeColorTexture,0,AccessFlags.ReadWrite);
                    builder.SetRenderFunc(static (EdgeData d,RasterGraphContext context)=>Blitter.BlitTexture(context.cmd,d.mask,new Vector4(1,1,0,0),d.material,1));
                }
            }
        }
    }
}
