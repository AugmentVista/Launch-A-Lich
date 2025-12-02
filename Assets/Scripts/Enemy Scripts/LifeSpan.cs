using UnityEngine;

public class LifeSpan : MonoBehaviour
{
    private void Start()
    {
        SelfDestruct();
    }

    public void SelfDestruct()
    {
        Destroy(gameObject, 30f);
    }
}