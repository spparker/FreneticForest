using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterManager : MonoBehaviour
{
    public enum CritterType
    {
        PATHER = 0,
        DIGGIE = 1,
        CHOPCHOP = 2,
        INVADER = 3,
    }

    public const int ENEMY_LAYER = 11;
    public const int DIGGIE_LAYER = 8;
    public const string DIGGIE_AGENT_STRING = "Burrower";
    public const string CHOPCHOP_AGENT_STRING = "ChopChop";

    public const float COMBAT_TICK_LENGTH = 4f; // Second per Damage

    public static CritterManager Instance{ get; private set; }

    public GameObject Critter_Prefab;
    public GameObject Dug_Prefab;

    List<CritterCommandControl>[] _critterTypeLists;

    private CritterType _curCycleList;
    private int _curListPos;

    List<CritterPod> _activeCombatList; // Attacking Pod and start time

    public CritterInputManager Input{get;private set;}

    public int PatherCount => _critterTypeLists[(int)CritterType.PATHER].Count;
    public int DiggieCount => _critterTypeLists[(int)CritterType.DIGGIE].Count;
    public int ChopChopCount => _critterTypeLists[(int)CritterType.CHOPCHOP].Count;

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;

        Input = GetComponent<CritterInputManager>();
        _activeCombatList = new List<CritterPod>();

        _critterTypeLists = new List<CritterCommandControl>[4];
        _critterTypeLists[(int)CritterType.PATHER] = new List<CritterCommandControl>();
        _critterTypeLists[(int)CritterType.DIGGIE] = new List<CritterCommandControl>();
        _critterTypeLists[(int)CritterType.CHOPCHOP] = new List<CritterCommandControl>();
        _critterTypeLists[(int)CritterType.INVADER] = new List<CritterCommandControl>();
    }

    void Update()
    {
        // COMBAT SYSTEM
        var destroy = new List<CritterPod>();
        var end = new List<CritterPod>();
        foreach(var critter in _activeCombatList)
        {
            critter.CombatTime += Time.deltaTime;
            if(critter.CombatTime > COMBAT_TICK_LENGTH)
            {
                if(critter.InCombat == null)
                {
                    //Debug.Log("Someone else destroyed my target");
                    end.Add(critter);
                    continue;
                }

                Input.PlayCritterAudio(critter.InCombat.CritterData.Sounds.SoundFight); // Play Invader Hurt noise
                //Debug.Log("Handle Combat for " + critter.name + " against " + critter.InCombat.name);
                critter.CombatTime = 0;
                bool hasWon = critter.InCombat.TakeDamage(critter.CritterData.damageOutput);
                if(hasWon)
                    destroy.Add(critter.InCombat);

                if(critter.TakeDamage(critter.InCombat.CritterData.damageOutput))
                    destroy.Add(critter);
                else if(hasWon)
                    end.Add(critter);
            }
        }

        foreach(var dead in destroy)
        {
            _activeCombatList.Remove(dead);
            CritterDeath(dead);
        }
        foreach(var winner in end)
            winner.EndCombat();
    }

    private void CritterDeath(CritterPod dead)
    {
        //Debug.Log("Death of: " + dead.name);
        _critterTypeLists[(int)dead.CritterData.type].Remove(dead.GetComponent<CritterCommandControl>());
        Input.ClearSelected(dead);
        if(dead.InTree)
            dead.InTree.LeaveTree(dead);
        Destroy(dead.gameObject);
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

    public void SpawnCritterAroundLocation(CritterTypeData data, int num, float radius, Vector3 pos)
    {
        var Holder = new GameObject(data.critterName).transform;

        for(int i=0;i<num;i++)
        {
            var x = Random.Range(radius, 3*radius);
            var z = Random.Range(radius, 3*radius);
            if(i % 2 == 0)
                x =-x;
            if(i%3 == 0)
                z = -z;

            Vector3 spawn_pos = new Vector3(pos.x + x, 0, pos.z + z);
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

    public void SpawnCritterAroundHomeTree(CritterTypeData data, int num, float radius)
    {
        SpawnCritterAroundLocation(data,num, radius, Vector3.zero);
    }

    public void SendAllCrittersHome()
    {
        foreach(var c in _critterTypeLists[(int)CritterType.PATHER])
            c.SendHome();

        foreach(var c in _critterTypeLists[(int)CritterType.DIGGIE])
            c.SendHome();

        foreach(var c in _critterTypeLists[(int)CritterType.CHOPCHOP])
            c.SendHome();

        foreach(var c in _critterTypeLists[(int)CritterType.INVADER])
        {
            if(c.Pod.InTree)
                c.Pod.InTree.LeaveTree(c.Pod);
            Destroy(c.gameObject);
        }
        _critterTypeLists[(int)CritterType.INVADER] = new List<CritterCommandControl>();
    }

    private void SetupEnemy(GameObject enemy_critter)
    {
        enemy_critter.layer = ENEMY_LAYER;
        enemy_critter.AddComponent<InvaderAI>();
        //Set to selected
        enemy_critter.GetComponent<CritterCommandControl>().SetSelected(true, Color.red);
    }

    // For AI to find players
    public CritterCommandControl GetNearestOfRandom(Vector3 pos)
    {
        int r = Random.Range(0,3);
        return GetNearestOfType((CritterType)r, pos);
    }

    public CritterCommandControl GetNearestOfType(CritterType ct, Vector3 pos)
    {
        if(_critterTypeLists[(int)ct].Count < 1)
            return null;
        float min_dist = 999;
        CritterCommandControl ret = _critterTypeLists[(int)ct][0]; // rip
        foreach (CritterCommandControl crit in _critterTypeLists[(int)ct])
        {
            float dist = Vector3.Magnitude(crit.transform.position - pos );
            if(dist < min_dist)
            {
                min_dist = dist;
                ret = crit;
            }
        }
        //Debug.Log("Nearest of type " + ct + ": " + ret);
        return ret;
    }

    public CritterCommandControl GetNextOfType(CritterType ct)
    {
        if(_critterTypeLists[(int)ct].Count < 1)
            return null;
            
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

    public void NotifyEnterCombat(CritterPod attacker)
    {
        _activeCombatList.Add(attacker);
    }

    public void NotifyEndCombat(CritterPod attacker)
    {
        _activeCombatList.Remove(attacker);
    }
}
