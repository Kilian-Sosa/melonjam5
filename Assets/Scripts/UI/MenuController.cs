using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public MenuController menuController;
    public GameObject credits;
    public GameObject settings;
    public GameObject menuPanel;
    public GameObject levelsPanel;

    void Start() {
        if (credits != null) AudioManager.Instance.PlayMusic("menuTheme"); // Play in the menu

        Transform buttonsContainer = levelsPanel.transform.GetChild(0);
        for (int i = 0; i <= PlayerPrefs.GetInt("levelsPassed", 0); i++) {
            GameObject levelButton = buttonsContainer.GetChild(i).gameObject;
            if (levelButton != null) levelButton.GetComponent<Button>().interactable = true;
        }
    }

    public void PerformAction(string action, string scene = "") {
        if (!AudioManager.Instance.IsPlayingCountDown()) AudioManager.Instance.PlaySFX("buttonClicked");

        switch (action) {
            case "StartGame":
                AudioManager.Instance.PlayMusic("mainTheme");
                SCManager.Instance.LoadScene("Game");
                break;
            case "ShowSettings":
                settings.SetActive(true);
                menuPanel.SetActive(false);
                break;
            case "ShowLevels":
                levelsPanel.SetActive(true);
                break;
            case "HideLevels":
                levelsPanel.SetActive(false);
                break;
            case "HideSettings":
                settings.SetActive(false);
                menuPanel.SetActive(true);
                menuPanel.GetComponent<Animator>().enabled = false;
                break;
            case "ShowCredits":
                credits.SetActive(true);
                menuPanel.SetActive(false);
                break;
            case "HideCredits":
                credits.SetActive(false);
                menuPanel.SetActive(true);
                menuPanel.GetComponent<Animator>().enabled = false;

                break;
            case "GoToRanking":
                SCManager.Instance.LoadScene("RankingScene");
                break;
            case "GoToMenu":
                SCManager.Instance.LoadScene("Menu");
                break;
            case "ExitGame":
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE_WIN
                Application.Quit();
#endif
                break;
        }
    }

    public void GoToIntro() => menuController.PerformAction("GoToIntro");

    public void StartGame(int level) {
        PlayerPrefs.SetInt("level", level);
        menuController.PerformAction("StartGame");
    }

    public void ShowSettings() => menuController.PerformAction("ShowSettings");

    public void ShowLevels() => menuController.PerformAction("ShowLevels");

    public void HideLevels() => menuController.PerformAction("HideLevels");

    public void HideSettings() => menuController.PerformAction("HideSettings");

    public void ShowCredits() => menuController.PerformAction("ShowCredits");

    public void HideCredits() => menuController.PerformAction("HideCredits");

    public void GoToRanking() => menuController.PerformAction("GoToRanking");

    public void GoToMenu() => menuController.PerformAction("GoToMenu");

    public void LoadScene(string scene) => menuController.PerformAction("LoadScene", scene);

    public void Resume() => menuController.PerformAction("Resume");

    public void ExitGame() => menuController.PerformAction("ExitGame");
}