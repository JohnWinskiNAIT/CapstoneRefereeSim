using System.Collections;
using System.Collections.Generic;
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
        playbackBar.value = manager.CurrentPosition;
    }
}
