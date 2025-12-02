using UnityEngine;

public class SnakeTypeBeat : MonoBehaviour
{
    private float snakeTime = 0f;
    public float snakeFrequency = 2f;
    public float snakeAmplitude = 0.5f;
    private float startY;

    private void Start()
    {
        startY = transform.position.y;
    }

    private void Update()
    {
        SnakeWave();
    }

    private void SnakeWave()
    {
        snakeTime += Time.deltaTime;

        float wave = Mathf.Sin(snakeTime * snakeFrequency) * snakeAmplitude;

        Vector3 pos = transform.position;
        pos.y = startY + wave;
        transform.position = pos;
    }
}
