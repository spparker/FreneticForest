using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/TreeGrowthData", order = 3)]
public class TreeGrowthData : ScriptableObject
{
    //Overgrow
    public float OvergrowStress = 0.05f;
    public float OvergrowRate = 0.05f;
    public float OvergrowVisualThreshold = 0.8f;

    //Stress
    public float StressVisualThreshold = 0.5f;
    public float StressPerInvader = 0.01f;
    public float StressRate = 0.0001f;

    //Occupying Critter Assist
    public float TrimRate = -0.2f;
    public float CalmRate = -0.001f;

    // Add Images here 
    public Sprite HealthyImage;
    public Sprite DiseasedImage; // Not Used??
    public Sprite BabyImage;
}
