using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScreenChangingButtons : MonoBehaviour
{
    [Header("UI Screens")]
    public GameObject Options;
    public GameObject Menu;
    public GameObject Gameplay;
    public GameObject Pause;
    public GameObject Credits;
    public GameObject Results;
    public GameObject Shop;

    [SerializeField] private GameObject LastScreenActive;

    private void Start()
    {
        SetUIFalse();
        Time.timeScale = 0;
        Menu.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (Gameplay.gameObject.activeSelf) { Time.timeScale = 1; }
        if (Input.GetKeyDown(KeyCode.Escape) && !Pause.activeSelf)
        {
            SetScreen(Pause);
        }

        else if (Input.GetKeyDown(KeyCode.Escape) && Pause.activeSelf)
        {
            SetScreen(Gameplay);
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
    }

    /// <summary>
    /// This method is called by other scripts when they need to change the screen
    /// </summary>
    /// <param name="screenToChangeTo"></param>
    public void ExternalScreenChange(string screenToChangeTo)
    {
    }

    private GameObject GetCurrentActiveScreen()
    {
        if (Options.activeSelf) return Options;
        if (Menu.activeSelf) return Menu;
        if (Gameplay.activeSelf) return Gameplay;
        if (Pause.activeSelf) return Pause;
        if (Credits.activeSelf) return Credits;
        if (Results.activeSelf) return Results;
        if (Shop.activeSelf) return Shop;
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

    // All Buttons start with B to make them easier to find in unity

    public void B_Play()
    {
        LastScreenActive = GetCurrentActiveScreen();

        SetScreen(Gameplay);
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
    }
    public void B_Resume()
    {
        SetScreen(Gameplay);
    }

    public void B_Continue()
    {
        Respawner.hasPlayerReturnedToLaunchpad = true;
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