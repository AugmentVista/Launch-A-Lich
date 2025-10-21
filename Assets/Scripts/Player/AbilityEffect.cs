using UnityEngine;

public class AbilityEffect : MonoBehaviour
{
    private Animator animator;

    private Rigidbody2D playerRb;

    public float abilityStrength;

    private void Start()
    {
        animator = GetComponent<Animator>();
        float clipLength = animator.GetCurrentAnimatorStateInfo(0).length;
        Destroy(gameObject, clipLength);
    }

    public void SetPlayerRb(Rigidbody2D rb)
    {
        playerRb = rb;
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerRb != null)
            {
                ApplyExp2Force(3f, abilityStrength);
                Debug.LogWarning("ABILITY HIT THE PLAYER");
            }
        }
    }

    /// <summary>
    ///  if inputX = 2, Angle Above X-Axis = 63.4°
    /// if inputX = 3, Angle Above X-Axis = 69.4°
    /// if inputX = 4, Angle Above X-Axis = 76°
    /// if inputX = 5, Angle Above X-Axis = 81.1°
    /// if inputX = 6, Angle Above X-Axis = 84.6°
    /// if inputX = 7, Angle Above X-Axis = 86.9°
    /// if inputX = 8, Angle Above X-Axis = 88.2°
    /// if inputX = 9, Angle Above X-Axis = 88.9°
    /// </summary>
    /// <param name="inputX"></param>
    /// <param name="magnitude"></param>
    public void ApplyExp2Force(float inputX, float magnitude)
    {
        float y = Mathf.Pow(2f, inputX);
        Vector2 direction = new Vector2(inputX, y).normalized;

        Vector2 force = direction * magnitude;
        playerRb.AddForce(force, ForceMode2D.Impulse);

        Debug.Log($"[Exp2] inputX: {inputX}, y: {y}, direction: {direction}, force: {force}");
    }
}