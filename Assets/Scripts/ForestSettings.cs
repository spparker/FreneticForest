using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ForestSettings", order = 1)]
public class ForestSettings : ScriptableObject
{
    // Number of trees to start
    public int numberOfTrees;

    // X,Z dimension x5
    public float forestScale; 

    // Distance away from edge of the forest
    public float edgeBufferSize;

}
