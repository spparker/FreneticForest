using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CritterTypeData", order = 2)]
public class CritterTypeData : ScriptableObject
{
    // What we're calling this
    public string critterName;

    // Need Definitional Type
    public CritterManager.CritterType type;

    // Is this the enemy?
    public bool enemy;

    // Can they occupy a tree
    public bool canEnterTrees;

    // AKA hit points
    public int numberOfIndividuals;

    // Speed of the pack
    public float moveSpeed;

    // Number of Individuals removed each combat cycle
    public int damageOutput;

    // Image of the critter
    public GameObject CritterSprite_Prefab;
    public CritterSoundPack Sounds;
    public GameObject FX_Prefab;
}
