using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuckImpactManager : MonoBehaviour
{
    [SerializeField]
    AudioClip chosenClip;
    [SerializeField]
    float volume;

    public void PlayImpactSound(Vector3 position)
    {
        SoundUtilities.PlaySound(chosenClip, volume, position);
    }
}
