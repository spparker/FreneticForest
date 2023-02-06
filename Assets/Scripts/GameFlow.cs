using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFlow : MonoBehaviour
{
    public enum Stage
    {
        INTRO,
        FIRST,
        SECOND,
        THIRD,
        OUTRO,
    }

    [HideInInspector] public GameStageData StageData;
    private Stage _currentStage;

    private bool _triggerOutro;
    private bool _inWinSequence;

    [SerializeField] private GameObject WinMessage;

    [SerializeField] private GameStageData StageZeroData;
    [SerializeField] private GameStageData StageOneData;
    [SerializeField] private GameStageData StageTwoData;
    [SerializeField] private GameStageData StageThreeData;

    [SerializeField] private AudioSource IntroLayer;
    [SerializeField] private AudioSource StageOneLayer;
    [SerializeField] private AudioSource StageTwoLayer;
    [SerializeField] private AudioSource StageThreeLayer;
    [SerializeField] private AudioSource OutroTrack;


    // Start is called before the first frame update
    void Start()
    {
        _currentStage = Stage.INTRO;
        StageData = StageZeroData;
        StageOneLayer.mute = true;
        StageTwoLayer.mute = true;
        StageThreeLayer.mute = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(_triggerOutro)
        {
            if(_inWinSequence)
            {
                if(!OutroTrack.isPlaying)
                    WinMessage.SetActive(false);
            }
            else if(!IntroLayer.isPlaying)
            {
                OutroTrack.Play();
                _inWinSequence = true;
                CritterManager.Instance.SendAllCrittersHome();
                WinMessage.SetActive(true);
            }
        }
        else if(ForestManager.Instance.HomeNetwork.TotalSize >= StageData.GoalNetworkSize)
            AdvanceStage();
    }

    private void AdvanceStage()
    {
        // Give Reward
        ForestManager.Instance.SpawnRewardAtNewestNode(StageData.RewardBaa, StageData.RewardPatta, StageData.RewardMasha);
        
        // Update Stage and Data

        SetupNextStage();

        // Set New Spawn Rate
        ForestManager.Instance.SetEnemySpawnValues(StageData.MinEnemySpawnTime, StageData.MaxEnemeySpawnTime, StageData.EnemySpawnNumber);
    }

    private void SetupNextStage()
    {
        _currentStage++;
        if(_currentStage == Stage.FIRST)
        {
            StageData = StageOneData; 
            StageOneLayer.mute = false;
        }
        else if(_currentStage == Stage.SECOND)
        {
            StageData = StageTwoData; 
            StageTwoLayer.mute = false;
        }
        else if(_currentStage == Stage.THIRD)
        {
            StageData = StageThreeData; 
            StageThreeLayer.mute = false;
        }
        else
            TriggerOutro();
    }

    private void TriggerOutro()
    {
        _triggerOutro = true;
        IntroLayer.loop = false;
        StageOneLayer.loop = false;
        StageTwoLayer.loop = false;
        StageThreeLayer.loop = false;
    }
}
