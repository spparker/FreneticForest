using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterPod : MonoBehaviour
{
    public const float BASE_RADIUS_SIZE = 0.5f;
    public const float POD_RADIUS_PER = 0.15f;
    public const float TO_CAPSULE_RADIUS = 1.5f;
    
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
            var offset = Random.insideUnitCircle * (BASE_RADIUS_SIZE + (POD_RADIUS_PER * CritterData.numberOfIndividuals));
            var spawn_pos = new Vector3(transform.position.x + offset.x,
                                        -CritterData.CritterSprite_Prefab.transform.position.y,
                                        transform.position.z + offset.y);
                                        
            var individual = Instantiate(CritterData.CritterSprite_Prefab, spawn_pos, Quaternion.identity);
            individual.transform.parent = transform;

            MyCritter_List.Add(individual);
        }

        ScaleClickWithPodSize();
    }

    private void ScaleClickWithPodSize()
    {
        _coll.radius = (BASE_RADIUS_SIZE + (POD_RADIUS_PER * MyCritter_List.Count)) * TO_CAPSULE_RADIUS;
        _agent.radius = _coll.radius;
    }
}
