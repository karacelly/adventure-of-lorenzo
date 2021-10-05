using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public TMP_Dropdown resDropdown;
    Resolution[] resolutions;

    void Start()
    {
        int CurrentResolutionIndex = 0;
        resolutions = Screen.resolutions;

        resDropdown.ClearOptions();

        List<string> options = new List<string>();

        for (int i = 0; i < resolutions.Length; i++)
        {
            string Option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(Option);

            if (resolutions[i].width == Screen.width &&
                resolutions[i].height == Screen.height)
            {
                CurrentResolutionIndex = i;
            }
        }

        resDropdown.AddOptions(options);
        resDropdown.value = CurrentResolutionIndex;
        resDropdown.RefreshShownValue();
    }

    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume (float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    public void SetQuality (int idx)
    {
        QualitySettings.SetQualityLevel(idx);
    }

    public void SetFullscreen (bool fullScreen)
    {
        Screen.fullScreen = fullScreen;
    }
}
