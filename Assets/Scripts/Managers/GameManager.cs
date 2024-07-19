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
    Coroutine _saturationCoroutine;
    Coroutine _destroyWorldCoroutine;
    Coroutine _updateStatusBar;
    int _groupStatus = 0;
    TMP_Text _countdownText;

    RawImage _screenshotImage;

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
        _playerCount = GameObject.FindGameObjectsWithTag("Player").Length -1;
        _enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        totalBalls = _playerCount + _enemyCount;
        UpdateStatus();
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

    public void UpdateStatus() {
        float oldStatus = status;
        totalBalls = _playerCount + _enemyCount;
        status = (float)_playerCount / totalBalls * 100;
        if (_saturationCoroutine != null) StopCoroutine(_saturationCoroutine);
        if (status != oldStatus) {
            if (_updateStatusBar != null) StopCoroutine(_updateStatusBar);
            _updateStatusBar = StartCoroutine(UpdateStatusBar(oldStatus));
        }
        if (status >= 75 && _destroyWorldCoroutine == null) {
            _destroyWorldCoroutine = StartCoroutine(DestroyWorld());
        } else if (status >= 60 && _groupStatus == 2) {
            _saturationCoroutine = StartCoroutine(ChangeSaturationCoroutine(2f, -40f));
            StartGroupCrazy();
        } else if (status >= 40 && _groupStatus == 1) {
            _saturationCoroutine = StartCoroutine(ChangeSaturationCoroutine(2f, -20f));
            StartGroupAlert();
        } else if (status >= 20 && _groupStatus == 0) {
            _saturationCoroutine= StartCoroutine(ChangeSaturationCoroutine(2f, 0f));
            StartGroupPatrol();
        }
    }

    public void ChangePlayerCount() {
        _playerCount = GameObject.FindGameObjectsWithTag("Player").Length - 1;
        _enemyCount = GameObject.FindGameObjectsWithTag("Enemy").Length;
        UpdateStatus();
    }

    void StartGroupPatrol() {
        _groupStatus = 1;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemyController.StopAllCoroutines();
            enemyController.StartGroupPatrol();
        }
    }

    void StartGroupAlert() {
        _groupStatus = 2;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            enemy.GetComponent<SyncController>().ChangeBalls(2);
            enemyController.StopAllCoroutines();
            enemyController.StartGroupAlert();
        }
    }

    void StartGroupCrazy() {
        _groupStatus = 3;
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies) {
            enemy.GetComponent<SyncController>().ChangeBalls(3);
            //EnemyController enemyController = enemy.GetComponent<EnemyController>();
            //enemyController.StopAllCoroutines();
            //enemyController.StartGroupCrazy();
        }
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
        _groupStatus = 0;
        _saturationCoroutine = null;
        _destroyWorldCoroutine = null;
        _updateStatusBar = null;
        _countdownText = null;
    }

    void TakePicture(string panelName) {
        GameObject hud = GameObject.Find("HUD");
        if (hud == null) return;
        GameObject panel = hud.transform.Find(panelName).gameObject;
        if (panel == null) return;

        _screenshotImage = panel.transform.Find("Screenshot").GetComponent<RawImage>();
        CaptureScreenshot();
        StartCoroutine(ShowPanel(panel));
        if (_destroyWorldCoroutine != null) StopCoroutine(_destroyWorldCoroutine);
        if (_countdownText != null) _countdownText.text = "";
    }

    IEnumerator ShowPanel(GameObject panel) {
        yield return new WaitForSecondsRealtime(0.2f);
        panel.SetActive(true);
    }

    void CaptureScreenshot() {
        StartCoroutine(LoadScreenshot());
    }

    IEnumerator LoadScreenshot() {
        // Espera un frame para que la captura de pantalla se complete
        yield return new WaitForEndOfFrame();

        // Crea una nueva textura con las dimensiones de la pantalla
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);

        // Lee los datos de la pantalla en la textura
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // Aplica la textura a la imagen del canvas
        _screenshotImage.texture = texture;
    }
}
