using System.Collections;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] float remainingTime = 100f;
    Coroutine _timerCoroutine;
    TextMeshProUGUI _timerText, _countdownText;
    CameraShake cameraShake;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
        //AudioManager.Instance.PlayMusic("mainTheme");
        PlayerPrefs.SetInt("levelAmount", 7);
    }

    public void GameOver() {
        //AudioManager.Instance.StopSFX();
        _countdownText.text = "";
        _countdownText.gameObject.GetComponent<Animator>().enabled = false;
        StopAllCoroutines();
        //AudioManager.Instance.StopSFX();
        //AudioManager.Instance.PlayMusic("gameOverTheme");
    }

    public void GameWon() {
        //AudioManager.Instance.StopSFX();
        _countdownText.text = "";
        _countdownText.gameObject.GetComponent<Animator>().enabled = false;
        StopAllCoroutines();
        //AudioManager.Instance.PlayMusic("gameWonTheme");
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
