using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public GameObject prefabToSpawn2;
    [SerializeField] AbilityCooldownBar abilityCooldown;
    [SerializeField] private Rigidbody2D playerRb;

    public Camera mainCamera;
    public float cooldown = 3f;

    private bool abilityEnabled = false;
    private float lastUseTime = -Mathf.Infinity;

    public float LastUseTime => lastUseTime;

    public float boostUpgradeValue;
    public float boostUpgradeCount;
    private float boostUpgradesActive = 0;

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
    }

    private void OnEnable()
    {
        PlayerStateMachine.OnGrounded += EnableAbility;
        PlayerStateMachine.OnFlying += EnableAbility;
        PlayerStateMachine.OnStopped += DisableAbility;
        PlayerStateMachine.OnInactive += DisableAbility;
        PlayerStateMachine.OnReadyToLaunch += DisableAbility;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= EnableAbility;
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
                SpawnAbility(prefabToSpawn);
                lastUseTime = Time.time;

                if (abilityCooldown != null)
                    abilityCooldown.StartCooldown();
            }
            else
            {
                Debug.Log("Ability is still on cooldown.");
            }
        }
        if (Input.GetMouseButtonDown(1) && prefabToSpawn2 != null)
        {
            if (Time.time >= lastUseTime + cooldown)
            {
                SpawnAbility(prefabToSpawn2);
                lastUseTime = Time.time;

                if (abilityCooldown != null)
                    abilityCooldown.StartCooldown();
            }
            else
            {
                Debug.Log("Ability is still on cooldown.");
            }
        }

    }

    private void SpawnAbility(GameObject prefab)
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        worldPos.z = 0f;

        GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
        AbilityEffect ability = instance.GetComponent<AbilityEffect>();

        if (ability != null)
        {
            // Apply the upgrade bonus to ability strength
            ability.abilityStrength += ApplyBoostUpgrade();
            ability.SetPlayerRb(playerRb);

            ability.SetPlayerTransform(playerRb.transform);
        }
    }

    public void UpgradeBoost(float improvementMod, float purchaseCount)
    {
        boostUpgradeCount = purchaseCount;
        boostUpgradeValue = improvementMod;

        boostUpgradesActive = ApplyBoostUpgrade();

        // Reduce cooldown based on number of upgrades (max 5 upgrades = -1s total)
        float baseCooldown = 3f;
        float cooldownReductionPerUpgrade = 0.2f;
        cooldown = Mathf.Clamp(baseCooldown - (boostUpgradeCount * cooldownReductionPerUpgrade), 1f, baseCooldown);
    }

    float ApplyBoostUpgrade()
    {
        // Ternary Operator condition ? valueIfTrue : valueIfFalse;
        return boostUpgradeCount > 0 ? boostUpgradeCount * boostUpgradeValue : 0;
    }

    private void EnableAbility() => abilityEnabled = true;
    private void DisableAbility() => abilityEnabled = false;
}