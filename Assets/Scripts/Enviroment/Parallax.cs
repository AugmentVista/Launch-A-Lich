using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] SpeedLimit speedLimit;

    [SerializeField] Material mat;

    public float speed = 0.2f;
    float distance;

    [Range(0f, 1.0f)]
    public float layerSpeedModifier;

    [SerializeField] float minPlayerSpeed = 0f;
    [SerializeField] float maxPlayerSpeed = 50f;


    void Start()
    {
        mat = GetComponent<Renderer>().material;
        speed = 0f;
        distance = 0f;

        if (speedLimit == null)
            speedLimit = FindFirstObjectByType<SpeedLimit>();
    }

    // Update is called once per frame
    void Update()
    {
        float playerSpeed = PlayerResultsManager.globalPlayerSpeedX;
        float baseline = speedLimit != null ? speedLimit.maxSpeedX : 100f;

        float normalizedSpeed;
        if (playerSpeed <= baseline)
        {
            normalizedSpeed = Mathf.InverseLerp(0f, baseline, playerSpeed);
        }
        else
        {
            // Smooth compression for excess speeds
            normalizedSpeed = 1f - Mathf.Exp(-((playerSpeed - baseline) / baseline));
            normalizedSpeed = Mathf.Clamp01(normalizedSpeed);
        }

        float targetSpeed = normalizedSpeed * layerSpeedModifier;
        speed = Mathf.Lerp(speed, targetSpeed, Time.deltaTime * 5f);

        if (playerSpeed < 0.001f) { speed = 0f; }
            

        distance += Time.deltaTime * speed;
        mat.SetTextureOffset("_MainTex", Vector2.right * distance);
    }
}
