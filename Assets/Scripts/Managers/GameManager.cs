using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    [SerializeField] float remainingTime = 100f;
    Coroutine _timerCoroutine;
    TextMeshProUGUI _timerText;
    TMP_Text _countdownText;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
        //AudioManager.Instance.PlayMusic("mainTheme");
        PlayerPrefs.SetInt("levelAmount", 7);
    }

    public void GameOver() {
        StopAllCoroutines();
        //AudioManager.Instance.StopSFX();
        //AudioManager.Instance.PlayMusic("gameOverTheme");
    }

    public void GameWon() {
        StopAllCoroutines();
        //AudioManager.Instance.PlayMusic("gameWonTheme");
    }

    IEnumerator DestroyWorld() {
        int count = 20;
        CameraShake cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        GameObject countdown = GameObject.Find("CountDown");
        _countdownText = countdown.GetComponent<TMP_Text>();
        countdown.GetComponent<Animator>().enabled = true;
        StartCoroutine(ChangeSaturationCoroutine(count));
        AudioManager.Instance.PlaySFX("countdown");
        while (count > 0) {
            cameraShake.Shake(0.5f, 0.7f);
            _countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }
        AudioManager.Instance.StopSFX();
        _countdownText.text = "";
        countdown.GetComponent<Animator>().enabled = false;
        GameOver();
    }

    public IEnumerator ChangeSaturationCoroutine(float duration, float endSaturation = -100f) {
        PostProcessVolume postProcessingVolume = FindObjectOfType<PostProcessVolume>();
        postProcessingVolume.profile.TryGetSettings(out ColorGrading colorAdjustments);
        float startSaturation = colorAdjustments.saturation.value;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            float newSaturation = Mathf.Lerp(startSaturation, endSaturation, elapsedTime / duration);
            colorAdjustments.saturation.value = newSaturation;
            yield return null;
        }

        colorAdjustments.saturation.value = endSaturation;
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
}
