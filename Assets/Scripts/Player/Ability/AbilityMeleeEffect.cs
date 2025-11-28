using System.Collections;
using UnityEngine;

public class AbilityMeleeEffect : MonoBehaviour
{
    private Animator parentAnimator;
    private Rigidbody2D playerRb;

    [Header("Movement Curve")]
    public AnimationCurve movementCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Strength Settings")]
    public float abilityStrength = 10f;
    public bool downForce = false;

    private float animDuration;
    private float elapsed;
    private bool pushing;

    private float relativeStrength;

    private void Start()
    {
        StartCoroutine(InitializeAfterAnimatorUpdates());
    }

    private IEnumerator InitializeAfterAnimatorUpdates()
    {
        parentAnimator = GetComponentInParent<Animator>();

        // Wait 1 frame so animator switches to newly played animation
        yield return null;

        if (parentAnimator != null)
        {
            animDuration = parentAnimator.GetCurrentAnimatorStateInfo(0).length;
            pushing = true;
        }
        else
        {
            Debug.LogWarning("Parent animator not found, destroying after 0.6667 seconds");
            animDuration = 0.667f;
        }

        Destroy(gameObject, animDuration);
    }

    public void SetPlayerRb(Rigidbody2D rb)
    {
        playerRb = rb;
    }

    private void Update()
    {
        if (PlayerResultsManager.globalPlayerSpeedY < 0 && !downForce)
        {
            relativeStrength = Mathf.Max(abilityStrength, PlayerResultsManager.currentHeight);
        }
        else
        {
            relativeStrength = abilityStrength;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                PlayerAbility playerAbility = GetComponentInParent<PlayerAbility>();
                playerAbility.KillConfirmed(enemy);
                enemy.hitByPlayerAbility = true;
                enemy.isDead = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (!pushing || playerRb == null) { return; }

        elapsed += Time.fixedDeltaTime;
        float time = Mathf.Clamp01(elapsed / animDuration);

        float multiplier = movementCurve.Evaluate(time);

        Vector2 dir = CalculateLogarithmicDirection();
        Vector2 force = dir * relativeStrength * multiplier;

        // Apply continuous force
        playerRb.AddForce(force, ForceMode2D.Force);

        if (time >= 1.0f) { pushing = false; }
    }

    /// <summary>
    /// Logarithmic Direction (Up-Right or Down-Right depending on downForce)
    ///
    /// Direction is based entirely on inputX:
    ///
    /// Up-Right angles (downForce = false):
    ///  x = 2  →  +73.8°
    ///  x = 3  →  +69.2°
    ///  x = 4  →  +66.1°
    ///  x = 5  →  +62.7°
    ///  x = 6  →  +60.1°
    ///  x = 7  →  +57.6°
    ///  x = 8  →  +55.0°
    ///  x = 9  →  +53.1°
    ///  x = 10 →  +51.4°
    ///  x = 12 →  +48.3°
    ///  x = 15 →  +44.7°
    ///  x = 20 →  +37.8°
    ///  x = 25 →  +33.4°
    ///  x = 30 →  +30.0°
    ///
    /// Down-Right angles (downForce = true):
    ///  Same angles as above, but negative.
    ///  Example: x = 6 → -60.1°
    ///
    /// Use higher X for flatter arcs, lower X for steeper arcs.
    /// </summary>
    private Vector2 CalculateLogarithmicDirection()
    {
        float x = 15f;
        if (downForce) { x = 30f; }
        float y = downForce ? -5f * Mathf.Log(x + 2f) : 5f * Mathf.Log(x + 2f);

        return new Vector2(x, y).normalized;
    }
}
