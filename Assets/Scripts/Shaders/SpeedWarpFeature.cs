using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SpeedWarpFeature : ScriptableRendererFeature
{
    class SpeedWarpPass : ScriptableRenderPass
    {
        private readonly Material warpMaterial;
        private readonly string profilerTag;
        private RTHandle tempTexture;

        public SpeedWarpPass(Material material, string tag)
        {
            warpMaterial = material;
            profilerTag = tag;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (warpMaterial == null)
                return;

            if (renderingData.cameraData.isSceneViewCamera ||
                renderingData.cameraData.isPreviewCamera)
                return;

            var colorTarget = renderingData.cameraData.renderer.cameraColorTargetHandle;
            if (colorTarget == null)
                return;

            // Allocate a temp texture matching the camera
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            RenderingUtils.ReAllocateIfNeeded(ref tempTexture, desc, name: "_TempSpeedWarpTexture");

            // Compute warp strength
            float speed = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);
            float warpFactor = Mathf.InverseLerp(40f, 100f, speed);
            float warpX = Mathf.Lerp(1f, 2.4f, warpFactor);
            float warpY = Mathf.Lerp(1f, 0.8f, warpFactor);

            warpMaterial.SetFloat("_WarpX", warpX);
            warpMaterial.SetFloat("_WarpY", warpY);

            var cmd = CommandBufferPool.Get(profilerTag);

            // ---- Correct ping-pong pattern ----
            // 1️⃣ Copy camera → temp
            Blitter.BlitCameraTexture(cmd, colorTarget, tempTexture);

            // 2️⃣ Apply warp shader temp → camera
            Blitter.BlitCameraTexture(cmd, tempTexture, colorTarget, warpMaterial, 0);
            // ----------------------------------

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            tempTexture?.Release();
        }
    }

    [SerializeField] private Material warpMaterial;
    [SerializeField] private RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;

    private SpeedWarpPass pass;

    public override void Create()
    {
        pass = new SpeedWarpPass(warpMaterial, "SpeedWarpPass")
        {
            renderPassEvent = passEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (warpMaterial == null)
            return;

        pass.ConfigureInput(ScriptableRenderPassInput.Color);
        renderer.EnqueuePass(pass);
    }
}