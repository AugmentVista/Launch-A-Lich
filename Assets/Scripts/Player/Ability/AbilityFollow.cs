using UnityEngine;

public class AbilityFollow : MonoBehaviour
{
    private Vector3 screenspaceClickPos;
    [Range(2f, 8f)] public float followLag;

    // Called once at spawn arugment provided by PlayerAbility
    public void SetScreenspaceClick(Vector3 screenPos)
    {
        screenspaceClickPos = screenPos;
    }

    private void LateUpdate()
    {
        // Convert the original click position into worldspace each frame
        Vector3 targetWorld = Camera.main.ScreenToWorldPoint(screenspaceClickPos);
        targetWorld.z = 0f;


        transform.position = Vector3.Lerp(transform.position,targetWorld,Time.deltaTime * followLag);
    }
}
