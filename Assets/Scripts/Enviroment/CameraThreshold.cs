using UnityEngine;

public class CameraThreshold : MonoBehaviour
{
    [SerializeField] private CameraFollow cam;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cam.applyLeftOffsetBias = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cam.applyLeftOffsetBias = false;
        }
    }
}
