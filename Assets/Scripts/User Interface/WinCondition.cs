using UnityEngine;

public class WinCondition : MonoBehaviour
{
    [SerializeField] private UIManager UI;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UI.SetVictory();
        }
    }
}
