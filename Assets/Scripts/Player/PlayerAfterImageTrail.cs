using UnityEngine;

public class PlayerAfterImageTrail : MonoBehaviour
{
    public GameObject afterImagePrefab;

    public float maxSpawnRatePerSecond; // 0.1
    public float minSpawnRatePerSecond; // 0.25

    public float slowSpeed = 10f; // lowest trail density
    public float fastSpeed = 100f; // highest trail density

    private float timer = 0f;

    private SpriteRenderer spriteRenderer;

    public PlayerStateMachine playerStateMachine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        // Only create afterimages during gameplay
        if (playerStateMachine.playerState == PlayerStateMachine.PlayerState.Flying || playerStateMachine.playerState == PlayerStateMachine.PlayerState.Grounded)
        {
            timer += Time.deltaTime;

            float speed = Mathf.Abs(PlayerResultsManager.globalPlayerSpeedX);

            // Normalize speed (0 -> 1) to use as a range
            float normalizedSpeed = Mathf.InverseLerp(slowSpeed, fastSpeed, speed);

            // Convert the normalized value into a value that can be lerped
            float spawnInterval = Mathf.Lerp(minSpawnRatePerSecond, maxSpawnRatePerSecond, normalizedSpeed);

            if (timer >= spawnInterval)
            {
                SpawnAfterImage();
                timer = 0f;
            }
        }
    }

    void SpawnAfterImage()
    {
        GameObject afterImage = Instantiate(afterImagePrefab, transform.position, transform.rotation);

        afterImage.GetComponent<AfterImage>().SetSprite(spriteRenderer.sprite);

        // scale sprite trail to match the size of the player sprite
        afterImage.transform.localScale = transform.localScale;

        afterImage.GetComponent<SpriteRenderer>().flipX = spriteRenderer.flipX;
    }
}
