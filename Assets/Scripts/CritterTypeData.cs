using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CritterTypeData", order = 2)]
public class CritterTypeData : ScriptableObject
{
    // Is this the enemy?
    public bool enemy;

    // AKA hit points
    public int numberOfIndividuals;

    // Speed of the pack
    public float moveSpeed;

    // Number of Individuals removed each combat cycle
    public int damageOutput; 
}
