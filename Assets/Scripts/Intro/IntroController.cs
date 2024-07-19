using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroController : MonoBehaviour {
    [SerializeField] GameObject[] texts;
    CameraBehaviour _cameraController;
    bool _direction = false;
    [SerializeField] GameObject playerSection2;
    [SerializeField] GameObject section3;
    [SerializeField] GameObject section4;


    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject startButton;
    int index = 0;
    Coroutine _saturationCoroutine;

    void Start() {
        // AudioManager.Instance.PlayMusic("intro");
        texts[index].SetActive(true);
        prevButton.SetActive(false);

        _cameraController = Camera.main.GetComponent<CameraBehaviour>();
    }

    public void ShowNext() {
        _direction = true;
        texts[index].SetActive(false);
        if (index == 3) index = 0;
        texts[++index].SetActive(true);

        if (index > 0) prevButton.SetActive(true);

        Toggle(index);
    }

    public void ShowPrev() {
        _direction = false;
        texts[index].SetActive(false);
        if (index == 0) index = 3;
        texts[--index].SetActive(true);

        Toggle(index);
    }

    void Toggle(int index) {
        ToggleButtons(index);
        TogglePlayerSection2Animator(index);
        ToggleSection3Animator(index);
        ToggleSection4Animator(index);
        ToggleSaturation(index);
    }

    public void ToggleButtons(int index) {
        if (!AudioManager.Instance.IsPlayingCountDown()) AudioManager.Instance.PlaySFX("buttonClicked");
        prevButton.SetActive(index != 0);
        nextButton.SetActive(index != 3);
        //startButton.SetActive(index == 3);

        _cameraController.Rotate(_direction);
    }

    public void TogglePlayerSection2Animator(int i) => playerSection2.SetActive(i == 1);

    public void ToggleSection3Animator(int i) {
        section3.SetActive(i == 2);
        section3.transform.Find("Player").GetComponent<Animator>().Play("playerIntroSection3");
    }

    public void ToggleSection4Animator(int i) => section4.SetActive(i == 3);

    public void ToggleSaturation(int i) {
        if (_saturationCoroutine != null) StopCoroutine(_saturationCoroutine);
        if (i == 3) _saturationCoroutine = StartCoroutine(GameManager.Instance.ChangeSaturationCoroutine(5));
        else StartCoroutine(GameManager.Instance.ChangeSaturationCoroutine(2, 5));
    }
}
