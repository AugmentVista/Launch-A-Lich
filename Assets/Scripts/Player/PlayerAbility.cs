using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public GameObject prefabToSpawn;
    [SerializeField] AbilityCooldownBar abilityCooldown;

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

        if (abilityCooldown == null)
        {
            Debug.LogError("Dependency between PlayerAbility and AbilityCooldownBar is broken. Assign it in the inspector.");
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

        if (Input.GetMouseButtonDown(0) && prefabToSpawn != null)
        {
            if (Time.time >= lastUseTime + cooldown)
            {
                SpawnAbility();
                lastUseTime = Time.time;

                // Start cooldown bar
                if (abilityCooldown != null)
                {
                    abilityCooldown.StartCooldown();
                }
            }
            else
            {
                Debug.Log("Ability is still on cooldown.");
            }
        }
    }

    private void SpawnAbility()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        Instantiate(prefabToSpawn, worldPos, Quaternion.identity);
    }

    private void EnableAbility() => abilityEnabled = true;
    private void DisableAbility() => abilityEnabled = false;
}
