using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AudioManager : MonoBehaviour {
    public static AudioManager Instance;
    [SerializeField] AudioSource _sfxSource;
    [SerializeField] AudioSource _musicSource;

    readonly Dictionary<string, AudioClip> _sfxClips = new();
    readonly Dictionary<string, AudioClip> _musicClips = new();

    void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSFXClips();
            LoadMusicClips();
        } else Destroy(gameObject);
    }

    void LoadSFXClips() {
        _sfxClips["countdown"] = Resources.Load<AudioClip>("SFX/countdownSFX");
        _sfxClips["buttonClicked"] = Resources.Load<AudioClip>("SFX/buttonClickedSFX");
        _sfxClips["path"] = Resources.Load<AudioClip>("SFX/pathSFX");
        _sfxClips["pathMoved"] = Resources.Load<AudioClip>("SFX/pathMovedSFX");
        _sfxClips["animalSaved"] = Resources.Load<AudioClip>("SFX/animalSavedSFX");
    }

    void LoadMusicClips() {
        _musicClips["mainTheme"] = Resources.Load<AudioClip>("Music/mainTheme");
        _musicClips["menuTheme"] = Resources.Load<AudioClip>("Music/menuTheme");
        _musicClips["gameOverTheme"] = Resources.Load<AudioClip>("Music/gameOverTheme");
        _musicClips["victoryTheme"] = Resources.Load<AudioClip>("Music/victoryTheme");
    }

    public void PlaySFX(string clipName) {
        if (_sfxClips.ContainsKey(clipName)) {
            _sfxSource.clip = _sfxClips[clipName];
            _sfxSource.Play();
        } else Debug.LogWarning("El AudioClip " + clipName + " no se encontr� en el diccionario de sfxClips.");
    }

    public void PlayMusic(string clipName) {
        if (_musicClips.ContainsKey(clipName)) {
            _musicSource.clip = _musicClips[clipName];
            _musicSource.Play();
        } else Debug.LogWarning("El AudioClip " + clipName + " no se encontr� en el diccionario de musicClips.");
    }

    public bool IsPlayingCountDown() => _sfxSource.clip != null && _sfxSource.isPlaying && _sfxSource.clip.name == "countdownSFX";

    public void StopMusic() =>  _musicSource.Stop();

    public void StopSFX() => _sfxSource.Stop();

    public void ChangeVolume(float value) {
        _sfxSource.volume = value;
        _musicSource.volume = value;
    }

    public void ChangeSFXVolume(float value) {
        _sfxSource.volume = value;
    }
}