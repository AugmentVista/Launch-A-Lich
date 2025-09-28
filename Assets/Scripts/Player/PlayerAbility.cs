using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public GameObject prefabToSpawn;

    public Camera mainCamera;
    public float cooldown = 1f;

    private bool abilityEnabled = false;
    private float lastUseTime = -Mathf.Infinity;

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        PlayerStateMachine.OnRolling += EnableAbility;
        PlayerStateMachine.OnFlying += EnableAbility;
        PlayerStateMachine.OnStopped += DisableAbility;
        PlayerStateMachine.OnInactive += DisableAbility;
        PlayerStateMachine.OnReadyToLaunch += DisableAbility;
    }

    private void OnDestroy()
    {
        PlayerStateMachine.OnRolling -= EnableAbility;
        PlayerStateMachine.OnFlying -= EnableAbility;
        PlayerStateMachine.OnStopped -= DisableAbility;
        PlayerStateMachine.OnInactive -= DisableAbility;
        PlayerStateMachine.OnReadyToLaunch -= DisableAbility;
    }

    private void Update()
    {
        if (!abilityEnabled) return;

        // Check cooldown
        if (Input.GetMouseButtonDown(0) && prefabToSpawn != null)
        {
            if (Time.time >= lastUseTime + cooldown)
            {
                SpawnAbility();
                lastUseTime = Time.time;
            }
            else
            {
                Debug.Log("Ability on cooldown");
            }
        }
    }

    private void SpawnAbility()
    {
        Vector3 mousePos = Input.mousePosition;

        // Convert screen point to world point
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
    }

    private void EnableAbility() => abilityEnabled = true;
    private void DisableAbility() => abilityEnabled = false;
}