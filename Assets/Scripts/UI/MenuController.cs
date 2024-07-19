using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
    public MenuController menuController;
    public GameObject credits;
    public GameObject settings;
    public GameObject menuPanel;

    void Start() {
        if (credits != null) AudioManager.Instance.PlayMusic("menuTheme"); // Play in the menu
    }

    public void PerformAction(string action, string scene = "") {
        if (!AudioManager.Instance.IsPlayingCountDown()) AudioManager.Instance.PlaySFX("buttonClicked");

        switch (action) {
            case "GoToIntro":
                AudioManager.Instance.PlayMusic("introTheme");
                SCManager.Instance.LoadScene("Intro");
                break;
            case "StartGame":
                AudioManager.Instance.PlayMusic("mainTheme");
                SCManager.Instance.LoadScene("Level1");
                break;
            case "ShowSettings":
                // SCManager.instance.LoadScene("GeneralSettingsScene");
                settings.SetActive(true);
                menuPanel.SetActive(false);
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
            case "Resume":
                GameManager.Instance.ResumeGame();
                break;
            case "LoadScene":
                GameManager.Instance.ResumeGame();
                SCManager.Instance.LoadScene(scene);
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

    public void StartGame() => menuController.PerformAction("StartGame");

    public void ShowSettings() => menuController.PerformAction("ShowSettings");

    public void HideSettings() => menuController.PerformAction("HideSettings");

    public void ShowCredits() => menuController.PerformAction("ShowCredits");

    public void HideCredits() => menuController.PerformAction("HideCredits");

    public void GoToRanking() => menuController.PerformAction("GoToRanking");

    public void GoToMenu() => menuController.PerformAction("GoToMenu");

    public void LoadScene(string scene) => menuController.PerformAction("LoadScene", scene);

    public void Resume() => menuController.PerformAction("Resume");

    public void ExitGame() => menuController.PerformAction("ExitGame");
}