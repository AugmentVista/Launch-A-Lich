using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class SpeedWarpEffect : MonoBehaviour
{
    public Material warpMaterial;
    public float maxPlayerSpeed = 100f;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (warpMaterial == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        // get player's current horizontal speed
        float speed = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);

        // normalize 0–1 between some low threshold and max
        float warpFactor = Mathf.InverseLerp(40f, maxPlayerSpeed, speed);

        // horizontal stretches up to +20%, vertical compresses to 90%
        float warpX = Mathf.Lerp(1f, 1.2f, warpFactor);
        float warpY = Mathf.Lerp(1f, 0.9f, warpFactor);

        // feed these to the shader
        warpMaterial.SetFloat("_WarpX", warpX);
        warpMaterial.SetFloat("_WarpY", warpY);

        // draw the warped frame to the screen
        Graphics.Blit(src, dest, warpMaterial);
    }
}
