using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CritterSoundPack", order = 4)]
public class CritterSoundPack : ScriptableObject
{
    public AudioClip SoundSelect;
    public AudioClip SoundTask;
    public AudioClip SoundCommand;
    public AudioClip SoundFight;
}
