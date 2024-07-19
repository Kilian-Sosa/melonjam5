using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class SettingsController : MonoBehaviour {
    [SerializeField] Slider _volumeSlider;
    [SerializeField] Slider _sfxSlider;
    [SerializeField] TMP_Dropdown _languageSelector;

    float _savedVolume;
    float _selectedVolume;

    float _savedSFX;
    float _selectedSFX;


    bool _active = false;
    int _selectedLanguage;
    int _savedLanguage;

    void Start() {
        _savedVolume = PlayerPrefs.GetFloat("BackgroundMusicVolume", 0.5f);
        _savedSFX = PlayerPrefs.GetFloat("SFXVolume", 0.8f);
        _savedLanguage = PlayerPrefs.GetInt("Language", 0);

        _selectedVolume = _savedVolume;
        _selectedSFX = _savedSFX;
        _selectedLanguage = _savedLanguage;

        LoadVolume(_savedVolume);
        LoadSFX(_savedSFX);
        LoadLanguage(_savedLanguage);
    }

    void LoadVolume(float volume) => _volumeSlider.value = volume;

    void LoadSFX(float sfx) => _sfxSlider.value = sfx;

    void LoadLanguage(int languageId) => _languageSelector.value = languageId;
    
    public void ChangeVolume() {
        _selectedVolume = _volumeSlider.value;
        AudioManager.Instance.ChangeVolume(_selectedVolume);
        PlayerPrefs.SetFloat("BackgroundMusicVolume", _selectedVolume);
    }

    public void ChangeSFX() {
        _selectedSFX = _sfxSlider.value;
        AudioManager.Instance.ChangeSFXVolume(_selectedSFX);
        PlayerPrefs.SetFloat("SFXVolume", _selectedSFX);
    }

    public void ChangeLanguage() {
        if (_active) return;
        _selectedLanguage = _languageSelector.value;
        StartCoroutine(SetLocale(_selectedLanguage));
        PlayerPrefs.SetInt("Language", _selectedLanguage);
    }

    IEnumerator SetLocale(int localeId) {
        _active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeId];
        _active = false;
    }

    public void SaveSettings() {
        PlayerPrefs.Save();
    }
}