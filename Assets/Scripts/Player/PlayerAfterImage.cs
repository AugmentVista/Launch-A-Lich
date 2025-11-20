using UnityEngine;

public class AfterImage : MonoBehaviour
{
    private SpriteRenderer sr;
    private Color startColor;

    public float lifetime = 0.15f;
    public float fadeSpeed = 8f;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        startColor = sr.color;
    }

    private void Update()
    {
        // Fade out
        sr.color = Color.Lerp(sr.color, new Color(startColor.r, startColor.g, startColor.b, 0), fadeSpeed * Time.deltaTime);

        lifetime -= Time.deltaTime;

        if (lifetime <= 0f)
            Destroy(gameObject);
    }

    public void SetSprite(Sprite sprite)
    {
        sr.sprite = sprite;
    }
}

