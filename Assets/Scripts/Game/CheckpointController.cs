using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointController : MonoBehaviour {
    int animalsCount, maxAnimalsCount;

    void Start() {
        //AudioManager.Instance.PlaySFX("portal");

        // If T - __ play countdown sound
        //AudioManager.Instance.PlaySFX("countdown");
    }

    void OnTriggerEnter(Collider other) {
        //AudioManager.Instance.PlaySFX("portal");
        if (maxAnimalsCount == 0) maxAnimalsCount = other.gameObject.transform.parent.parent.childCount;
        animalsCount++;

        if (animalsCount == maxAnimalsCount) StartCoroutine(NextLevel());
    }

    IEnumerator NextLevel() {
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level") + 1);
        PlayerPrefs.Save();
        yield return new WaitForSeconds(1);
        SCManager.Instance.LoadScene("Game");
    }
}
