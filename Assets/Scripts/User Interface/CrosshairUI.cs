using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    [Header("Crosshair Texture")]
    public Texture2D crosshairTexture;
    public Vector2 hotspot = Vector2.zero;
    public CursorMode cursorMode = CursorMode.Auto;

    private void OnEnable()
    {
        PlayerStateMachine.OnFlying += SetCursorToCrosshair;
        PlayerStateMachine.OnStopped += SetCursorDefault;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnFlying -= SetCursorToCrosshair;
        PlayerStateMachine.OnStopped -= SetCursorDefault;
    }

    public void SetCursorToCrosshair()
    {
        if (crosshairTexture != null)
        {
            Cursor.SetCursor(crosshairTexture, hotspot, cursorMode);
        }

        Cursor.visible = true;

        Cursor.lockState = CursorLockMode.None;
    }

    public void SetCursorDefault()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
   
}
