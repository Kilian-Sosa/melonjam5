using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SettingsSetup : MonoBehaviour {
     void Start() {
        
        float savedVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.5f);
        
        AudioManager.Instance.ChangeVolume(savedVolume);
        
        float savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        AudioManager.Instance.ChangeSFXVolume(savedSFX);

        int savedLanguage = PlayerPrefs.GetInt("Language", 0);
        StartCoroutine(SetLocale(savedLanguage));
    }

    IEnumerator SetLocale(int localeId) {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
    }
}
