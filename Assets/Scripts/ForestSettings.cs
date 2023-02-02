using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ForestSettings", order = 1)]
public class ForestSettings : ScriptableObject
{
    // Number of trees to start
    public int numberOfTrees;

    // How fast trees grow (relative to real time)
    public float treeGrowthRate;

    // Number of assisting critter groups
    public CritterSettings critters;
    
    public int numberOfPathers;
    public int numberOfDiggies;
    public int numberOfChopChops;

    // X,Z dimension x5
    public float forestScale; 

    // Distance away from edge of the forest
    public float edgeBufferSize;

    // Minutes per day
    public float minutesPerDay;
}
