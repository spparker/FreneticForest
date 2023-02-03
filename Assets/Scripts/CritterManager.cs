using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterManager : MonoBehaviour
{
    public enum CritterType
    {
        PATHER = 0,
        DIGGIE,
        CHOPCHOP,
        INVADER
    }

    public const int ENEMY_LAYER = 11;
    public const int DIGGIE_LAYER = 8;
    public const string DIGGIE_AGENT_STRING = "Burrower";
    public const string CHOPCHOP_AGENT_STRING = "ChopChop";

    public static CritterManager Instance{ get; private set; }

    public GameObject Critter_Prefab;
    public GameObject Dug_Prefab;

    List<CritterCommandControl>[] _critterTypeLists;

    private CritterType _curCycleList;
    private int _curListPos;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        _critterTypeLists = new List<CritterCommandControl>[4];
        _critterTypeLists[(int)CritterType.PATHER] = new List<CritterCommandControl>();
        _critterTypeLists[(int)CritterType.DIGGIE] = new List<CritterCommandControl>();
        _critterTypeLists[(int)CritterType.CHOPCHOP] = new List<CritterCommandControl>();
        _critterTypeLists[(int)CritterType.INVADER] = new List<CritterCommandControl>();
    }

    public void SpawnCritterFromData(CritterTypeData data, int num, float max_pos)
    {
        var Holder = new GameObject(data.critterName).transform;

        for(int i=0;i<num;i++)
        {
            Vector3 spawn_pos = new Vector3(Random.Range(-max_pos,max_pos),
                                        0, Random.Range(-max_pos, max_pos));
            var new_critter = Instantiate(Critter_Prefab, spawn_pos, Quaternion.identity);
            
            
            var pod = new_critter.GetComponent<CritterPod>();
            pod.CritterData = data;

            if(data.type == CritterType.DIGGIE)
            {
                new_critter.layer = DIGGIE_LAYER;
                pod.SetAgentToType(DIGGIE_AGENT_STRING);
            }
            else if(data.type == CritterType.CHOPCHOP)
                pod.SetAgentToType(CHOPCHOP_AGENT_STRING);
            new_critter.transform.name = data.critterName + i;
            new_critter.transform.SetParent(Holder);
            if(data.enemy)
                SetupEnemy(new_critter);
            _critterTypeLists[(int)data.type].Add(new_critter.GetComponent<CritterCommandControl>());
        }
    }

    private void SetupEnemy(GameObject enemy_critter)
    {
        enemy_critter.layer = ENEMY_LAYER;
    }

    public CritterCommandControl GetNextOfType(CritterType ct)
    {
        if(ct == _curCycleList)
        {
            _curListPos++;
            if(_curListPos >= _critterTypeLists[(int)ct].Count)
                _curListPos = 0;
            return _critterTypeLists[(int)ct][_curListPos];
        }
        else
        {
            _curListPos = 0;
            _curCycleList = ct;
            return _critterTypeLists[(int)ct][_curListPos];
        }
    }
}
