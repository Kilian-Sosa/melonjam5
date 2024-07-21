using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] float remainingTime = 100f;
    CameraShake cameraShake;
    Coroutine _timerCoroutine;
    TextMeshProUGUI _timerText, _countdownText;
    RawImage _screenshotImage;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
        //AudioManager.Instance.PlayMusic("mainTheme");
        PlayerPrefs.SetInt("levelsPassed", 1);
    }

    public void GameOver() {
        //AudioManager.Instance.StopSFX();
        _countdownText.text = "";
        _countdownText.gameObject.GetComponent<Animator>().enabled = false;
        StopAllCoroutines();
        TakePicture("GameOverPanel");
        //AudioManager.Instance.PlayMusic("gameOverTheme");
    }

    public void GameWon() {
        //AudioManager.Instance.StopSFX();
        _countdownText.text = "";
        _countdownText.gameObject.GetComponent<Animator>().enabled = false;
        StopAllCoroutines();
        TakePicture("GameWonPanel");
        //AudioManager.Instance.PlayMusic("gameWonTheme");
    }

    void TakePicture(string panelName) {
        GameObject uiObj = GameObject.Find("UI");
        if (uiObj == null) return;
        Transform panel = uiObj.transform.Find(panelName);
        if (panel == null) return;

        _screenshotImage = panel.transform.Find("Screenshot").GetComponent<RawImage>();
        CaptureScreenshot();
        StartCoroutine(ShowPanel(panel.gameObject));
    }

    IEnumerator ShowPanel(GameObject panel) {
        yield return new WaitForSecondsRealtime(0.2f);
        panel.SetActive(true);
    }

    void CaptureScreenshot() {
        StartCoroutine(LoadScreenshot());
    }

    IEnumerator LoadScreenshot() {
        yield return new WaitForEndOfFrame();
        Texture2D texture = new(Screen.width, Screen.height, TextureFormat.RGB24, false);

        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        _screenshotImage.texture = texture;
    }

    public void StartTimer() {
        _timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
        _timerCoroutine = StartCoroutine(UpdateTimer());
    }

    public void StopTimer() => StopCoroutine(_timerCoroutine);

    IEnumerator UpdateTimer() {
        _timerText.text = FormatTime(remainingTime);
        while (remainingTime > 0) {
            yield return new WaitForSeconds(1);
            _timerText.text = FormatTime(--remainingTime);
            if (remainingTime <= 20) ShowCountdown();
        }
        GameOver();
    }

    string FormatTime(float time) {
        string minutes = (Mathf.Floor(Mathf.Round(time) / 60)).ToString();
        string seconds = (Mathf.Round(time) % 60).ToString();

        if (minutes.Length == 1) minutes = "0" + minutes;
        if (seconds.Length == 1) seconds = "0" + seconds;
        return minutes + ":" + seconds;
    }

    void ShowCountdown() {
        if (_countdownText == null) {
            cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
            GameObject countdown = GameObject.Find("CountDown");
            _countdownText = countdown.GetComponent<TextMeshProUGUI>();
            countdown.GetComponent<Animator>().enabled = true;
            //AudioManager.Instance.PlaySFX("countdown");
        }
        cameraShake.Shake(0.5f, 0.7f);
        _countdownText.text = remainingTime.ToString();
    }
}
