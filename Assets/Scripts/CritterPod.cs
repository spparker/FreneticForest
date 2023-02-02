using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterPod : MonoBehaviour
{
    public const float POD_RADIUS_PER = 0.2f;
    public const float TO_CAPSULE_RADIUS = 3f;
    
    public GameObject CritterSprite_Prefab;
    public CritterTypeData CritterData;
    List<GameObject> MyCritter_List = new List<GameObject>();

    private NavMeshAgent _agent;
    private CapsuleCollider _coll;

    void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _coll = GetComponent<CapsuleCollider>();
    }

    void Start()
    {   

        _agent.speed = CritterData.moveSpeed;

        for(int i=0;i<CritterData.numberOfIndividuals;i++)
        {
            var offset = Random.insideUnitCircle * POD_RADIUS_PER * CritterData.numberOfIndividuals;
            var spawn_pos = transform.position + new Vector3(offset.x, 0, offset.y);
            var individual = Instantiate(CritterSprite_Prefab, spawn_pos, Quaternion.identity);
            individual.transform.parent = transform;
            MyCritter_List.Add(individual);
        }

        ScaleClickWithPodSize();
    }

    private void ScaleClickWithPodSize()
    {
        _coll.radius = POD_RADIUS_PER * MyCritter_List.Count * TO_CAPSULE_RADIUS;
    }
}
