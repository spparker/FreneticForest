using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameStageData", order = 6)]
public class GameStageData : ScriptableObject
{
    // Goal network size for this Stage
    public int GoalNetworkSize;

    // What you get once you reach the goal
    public int RewardBaa;
    public int RewardPatta;
    public int RewardMasha;

    public float MinEnemySpawnTime;
    public float MaxEnemeySpawnTime;
    public int EnemySpawnNumber;
}
