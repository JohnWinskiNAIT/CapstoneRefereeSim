using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaybackUI : MonoBehaviour
{
    [SerializeField]
    Button stopStartButton;
    [SerializeField]
    Image stopStartIcon;
    [SerializeField]
    Slider playbackBar;

    int sliderPosition;

    PlaybackManager manager;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<PlaybackManager>();
        playbackBar.maxValue = manager.TotalCount;
    }

    private void Update()
    {
        if (sliderPosition != manager.CurrentPosition && manager.Playback)
        {
            UpdateSlider();
        }

        EnableBar();

        if (manager.Playback)
        {
            stopStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Pause";
        }
        else
        {
            stopStartButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play";
        }
    }

    void EnableBar()
    {
        if (manager.Playback)
        {
            playbackBar.interactable = false;
        }
        else
        {
            playbackBar.interactable = true;
        }
    }

    public void PausePlayback()
    {
        manager.TogglePlayback(!manager.Playback);
    }

    public void DragSlider()
    {
        manager.SetToPosition((int)playbackBar.value);
    }

    void UpdateSlider()
    {
        if (manager.CurrentPosition > playbackBar.maxValue)
        {
            playbackBar.value = playbackBar.maxValue;
        }
        else
        {
            playbackBar.value = manager.CurrentPosition;
        }
    }
}
