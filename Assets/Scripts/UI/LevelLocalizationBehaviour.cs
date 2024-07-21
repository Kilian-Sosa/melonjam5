using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class LevelLocalizationBehaviour : MonoBehaviour
{
    [SerializeField] LocalizedString localStringLevel;
    [SerializeField] TextMeshProUGUI textComp;

    void Start() {
        localStringLevel.Arguments = new object[] { PlayerPrefs.GetInt("level", 1) };
        textComp.text = localStringLevel.GetLocalizedString();
    }
}
