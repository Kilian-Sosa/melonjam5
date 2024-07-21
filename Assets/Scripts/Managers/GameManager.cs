using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }
    public int totalBalls = 0;
    public float status = 0;
    public GameObject HPBar;
    public bool isActive = false;
    int _playerCount = 0;
    int _enemyCount = 0;
    Coroutine _destroyWorldCoroutine;
    TMP_Text _countdownText;

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else Destroy(gameObject);
        AudioManager.Instance.PlayMusic("mainTheme");
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (isActive) PauseGame();
            else ResumeGame();
        }
        if (Input.GetKeyDown(KeyCode.F1)) SceneManager.LoadScene("Level1");
        if (Input.GetKeyDown(KeyCode.F2)) SceneManager.LoadScene("Level2");
        if (Input.GetKeyDown(KeyCode.F3)) SceneManager.LoadScene("Level3");
    }

    public void PauseGame() {
        GameObject hud = GameObject.Find("HUD");
        if (hud == null) return;
        GameObject pausePanel = hud.transform.Find("PausePanel").gameObject;
        pausePanel.SetActive(true);
        StopAllCoroutines();
        AudioManager.Instance.StopSFX();
        isActive = false;
    }

    public void ResumeGame() {
        GameObject hud = GameObject.Find("HUD");
        if (hud == null) return;
        GameObject pausePanel = hud.transform.Find("PausePanel").gameObject;
        isActive = true;
        HPBar = GameObject.Find("HPBar");
        if (HPBar == null) return;
        if (!pausePanel.activeSelf) HPBar.GetComponent<Image>().fillAmount = 0;
        pausePanel.SetActive(false);
        UpdateCounts();
    }

    public void UpdateCounts() {
        _playerCount = GameObject.FindGameObjectsWithTag("Player").Length - 1;
        _enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        totalBalls = _playerCount + _enemyCount;
    }

    public void GameOver() {
        StopAllCoroutines();
        AudioManager.Instance.StopSFX();
        AudioManager.Instance.PlayMusic("gameOverTheme");
        TakePicture("GameOverPanel");
        isActive = false;
    }

    public void GameWon() {
        StopAllCoroutines();
        AudioManager.Instance.PlayMusic("gameWonTheme");
        TakePicture("GameWonPanel");
        isActive = false;
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

    IEnumerator UpdateStatusBar(float oldStatus) {
        if (oldStatus > status) {
            for (float i = oldStatus; i >= status; i--) {
                HPBar.GetComponent<Image>().fillAmount = i / 100;
                yield return new WaitForSeconds(0.1f);
            }
        } else
            for (float i = oldStatus; i <= status; i++) {
                HPBar.GetComponent<Image>().fillAmount = i / 100;
                yield return new WaitForSeconds(0.1f);
            }
    }

    public void NextLevel() {
        AudioManager.Instance.PlayMusic("mainTheme");
        AudioManager.Instance.StopSFX();
        StopAllCoroutines();
        ResumeGame();
        status = 0;
        _destroyWorldCoroutine = null;
        _countdownText = null;
    }

    void TakePicture(string panelName) {
        GameObject hud = GameObject.Find("HUD");
        if (hud == null) return;
        GameObject panel = hud.transform.Find(panelName).gameObject;
        if (panel == null) return;

        StartCoroutine(ShowPanel(panel));
        if (_destroyWorldCoroutine != null) StopCoroutine(_destroyWorldCoroutine);
        if (_countdownText != null) _countdownText.text = "";
    }

    IEnumerator ShowPanel(GameObject panel) {
        yield return new WaitForSecondsRealtime(0.2f);
        panel.SetActive(true);
    }
}
