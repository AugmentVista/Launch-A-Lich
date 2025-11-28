using UnityEngine;

public class PlayerAbility : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public GameObject prefabToSpawn2;
    [SerializeField] ManaPool abilityCooldown;
    [SerializeField] private Rigidbody2D playerRb;

    public Camera mainCamera;

    private bool abilityEnabled = false;
    private float lastUseTime = -Mathf.Infinity;
    public float LastUseTime => lastUseTime;

    public Vector2 Offset = new Vector2(0f,4f);

    public float boostUpgradeValue;
    public float boostUpgradeCount;
    private float boostUpgradesActive = 0;

    public float maxMana = 1f;     // UI fill amount is 0 → 1
    public float currentMana = 1f; // starts full
    public float manaRegenRate = 0.1f; // per second
    public float manaCost = 1f;    // cost per ability use (full bar)

    private void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    private void OnEnable()
    {
        PlayerStateMachine.OnGrounded += EnableAbility;
        PlayerStateMachine.OnFlying += EnableAbility;
        PlayerStateMachine.OnStopped += DisableAbility;
        PlayerStateMachine.OnInactive += DisableAbility;
        PlayerStateMachine.OnReadyToLaunch += DisableAbility;
        PlayerStateMachine.OnReadyToLaunch += ResetMana;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= EnableAbility;
        PlayerStateMachine.OnFlying -= EnableAbility;
        PlayerStateMachine.OnStopped -= DisableAbility;
        PlayerStateMachine.OnInactive -= DisableAbility;
        PlayerStateMachine.OnReadyToLaunch -= DisableAbility;
        PlayerStateMachine.OnReadyToLaunch += ResetMana;
    }

    public void ResetMana()
    {
        currentMana = maxMana;
        abilityCooldown.UpdateMana(currentMana);
    }

    private void Update()
    {
        if (!abilityEnabled || Time.timeScale == 0) return;

        // Passive regen
        currentMana += manaRegenRate * Time.deltaTime;
        currentMana = Mathf.Clamp01(currentMana);

        // UI update
        if (abilityCooldown != null)
            abilityCooldown.UpdateMana(currentMana);

        // Left click ability UP
        if (Input.GetMouseButtonDown(0) && prefabToSpawn != null)
        {
            TryUseAbility(prefabToSpawn);
        }

        // Right click ability DOWN
        if (Input.GetMouseButtonDown(1) && prefabToSpawn2 != null)
        {
            TryUseAbility(prefabToSpawn2);
        }
    }

    public void AddMana(float amount)
    {
        currentMana = Mathf.Clamp01(currentMana + amount);
    }

    private void TryUseAbility(GameObject prefab)
    {
        float cost = manaCost;

        AbilityManaCost costComponent = prefab.GetComponent<AbilityManaCost>();
        if (costComponent != null)
            cost = costComponent.cost;

        if (currentMana < cost)
            return;

        currentMana -= cost;

        if (abilityCooldown != null)
            abilityCooldown.UpdateMana(currentMana);

        SpawnAbility(prefab);
    }

    private void SpawnAbility(GameObject prefab)
    {
        //Vector3 mousePos = Input.mousePosition;
        //Vector3 worldPos = mainCamera.ScreenToWorldPoint(mousePos);
        //worldPos.z = 0f;

        //GameObject instance = Instantiate(prefab, worldPos, Quaternion.identity);
        //AbilityFollow follow = instance.GetComponent<AbilityFollow>();
        //if (follow != null)
        //    follow.SetScreenspaceClick(mousePos);

        //AbilityEffect ability = instance.GetComponent<AbilityEffect>();

        GameObject instance = Instantiate(prefab, transform.position, Quaternion.identity, transform);

        AbilityMeleeEffect ability = instance.GetComponent<AbilityMeleeEffect>();
        if (ability != null)
        {
            // Apply the upgrade bonus to ability strength
            ability.abilityStrength += ApplyBoostUpgrade();
            ability.SetPlayerRb(playerRb);
        }
    }

    public void UpgradeBoost(float improvementMod, float purchaseCount)
    {
        boostUpgradeCount = purchaseCount;
        boostUpgradeValue = improvementMod;

        boostUpgradesActive = ApplyBoostUpgrade();
    }

    float ApplyBoostUpgrade()
    {
        // Ternary Operator condition ? valueIfTrue : valueIfFalse;
        return boostUpgradeCount > 0 ? boostUpgradeCount * boostUpgradeValue : 0;
    }

    private void EnableAbility() => abilityEnabled = true;
    private void DisableAbility() => abilityEnabled = false;
}