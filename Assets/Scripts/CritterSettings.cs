using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CritterSettings", order = 3)]
public class CritterSettings : ScriptableObject
{
    public CritterTypeData patherData;
    public CritterTypeData diggieData;
    public CritterTypeData chopData;

}
