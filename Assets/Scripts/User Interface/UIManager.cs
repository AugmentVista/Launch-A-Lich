using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [SerializeField] Respawner respawner;

    [Header("UI Screens")]
    public GameObject Menu;
    public GameObject Instructions;
    public GameObject Options;
    public GameObject Shop;
    public GameObject Pause;
    public GameObject Gameplay;
    public GameObject Results;
    public GameObject Credits;
    public GameObject Victory;

    [SerializeField] private GameObject LastScreenActive;

    [Range(0.25f, 1f)]
    public float time;

    private float minTime = 0.25f;

    private float maxTime = 1f;

    bool gaming = false;

    private void OnEnable()
    {
        PlayerStateMachine.OnGrounded += gameTime;
        PlayerStateMachine.OnFlying += gameTime;
    }

    private void OnDisable()
    {
        PlayerStateMachine.OnGrounded -= gameTime;
        PlayerStateMachine.OnFlying -= gameTime;
    }

    private void Start()
    {
        SetUIFalse();
        Time.timeScale = 0;
        Menu.gameObject.SetActive(true);
    }

    private void gameTime()
    {
        gaming = true;
    }

    private void Update()
    {
        if (Gameplay.gameObject.activeSelf && gaming) 
        {
            float speed = PlayerResultsManager.globalPlayerSpeedX;

            // Clamp the raw speed between 25 and 150
            float clampedSpeed = Mathf.Clamp(speed, 25f, 150f);

            // Normalize (100 -> 0, 25 -> 1)
            float normalizedSpeed = Mathf.InverseLerp(100f, 25f, clampedSpeed);

            // Lerp between minTime and maxTime
            float relativeTime = Mathf.Lerp(minTime, maxTime, normalizedSpeed);

            time = relativeTime;
            Time.timeScale = relativeTime;

        }
        if (Input.GetKeyDown(KeyCode.Escape) && !Pause.activeSelf && Gameplay.gameObject.activeSelf)
        {
            SetScreen(Pause);
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && Pause.activeSelf)
        {
            SetScreen(Gameplay);
        }
        else if (Gameplay.gameObject.activeSelf)
        {
            Time.timeScale = 1f;
        }
    }

    public void SetUIFalse()
    {
        Options.gameObject.SetActive(false);
        Gameplay.gameObject.SetActive(false);
        Menu.gameObject.SetActive(false);
        Pause.gameObject.SetActive(false);
        Credits.gameObject.SetActive(false);
        Results.gameObject.SetActive(false);
        Shop.gameObject.SetActive(false);
        Instructions.gameObject.SetActive(false);
        Victory.gameObject.SetActive(false);
    }


    private GameObject GetCurrentActiveScreen()
    {
        if (Options.activeSelf) return Options;
        if (Instructions.activeSelf) return Instructions;
        if (Menu.activeSelf) return Menu;
        if (Gameplay.activeSelf) return Gameplay;
        if (Pause.activeSelf) return Pause;
        if (Credits.activeSelf) return Credits;
        if (Results.activeSelf) return Results;
        if (Shop.activeSelf) return Shop;
        if (Victory.activeSelf) return Victory;
        return null;
    }

    private void SetScreen(GameObject newScreen)
    {
        if (newScreen == null)
            return;

        // Save the currently active screen before switching
        if (LastScreenActive != null && LastScreenActive != newScreen && LastScreenActive.activeSelf)
        {
            LastScreenActive = GetCurrentActiveScreen();
        }

        if (newScreen == Gameplay)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        SetUIFalse();
        newScreen.SetActive(true);
    }


    public void SetVictory()
    {
        SetScreen(Victory);
    }

    // All Buttons start with B to make them easier to find in unity

    public void B_Play()
    {
        LastScreenActive = GetCurrentActiveScreen();

        SetScreen(Instructions);
    }

    public void B_CreditsMenu()
    {
        LastScreenActive = GetCurrentActiveScreen();

        SetScreen(Credits);
    }

    public void B_OptionsMenu()
    {
        LastScreenActive = GetCurrentActiveScreen();

        SetScreen(Options);
    }

    public void B_Pause()
    {
        LastScreenActive = GetCurrentActiveScreen();

        SetScreen(Pause);
    }

    public void B_Results()
    {
        LastScreenActive = GetCurrentActiveScreen();

        SetScreen(Results);
    }

    public void B_Return()
    {
        SetScreen(LastScreenActive);
    }

    public void B_ToMainMenu()
    {
        SetScreen(Menu);
    }

    public void B_OpenShop()
    {
        SetScreen(Shop);

        if (respawner != null)
        {
            respawner.RespawnPlayer();
        }
        
    }
    public void B_Resume()
    {
        SetScreen(Gameplay);
    }

    public void B_Continue()
    {
        SetUIFalse();
        SetScreen(Gameplay);
    }

    public void B_ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void B_Quiting()
    {
        Application.Quit();
    }

}