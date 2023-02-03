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

    public TreeGrowth InTree {get; private set;}

    public bool InPatrol {get; private set;}

    private Vector3 _enter_pos;

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

    void Update()
    {
        if(InTree)
            transform.position = new Vector3(InTree.transform.position.x, InTree.Top , InTree.transform.position.z);
    }

    public void StartPatrol()
    {
        InPatrol = true;
    }

    public void SetInTree(TreeGrowth tree)
    {
        _enter_pos = transform.position;
        InTree = tree;
        _agent.enabled = false;
        _coll.enabled = false;
        transform.position = new Vector3(tree.transform.position.x, tree.Top ,tree.transform.position.z);
    }

    public void SetOnGround()
    {
        //WE NEED TO NOT GET STUCK IN THE BIG TREEES COMING DOWN
        //ForestManager.Instance.NavSurface.

        //transform.position = (InTree.transform.position - _enter_pos)  * InTree.Top; // Scale exit position;
        InTree.LeaveTree();
        InTree = null;
        transform.position = _enter_pos;
        _coll.enabled = true;
        _agent.enabled = true;
    }

    public void SetSelectedShader(bool isSelected)
    {
        int set_val = isSelected? 1: 0;
        //TODO: Share a material
        foreach( var crit in MyCritter_List)
        {
            var crit_mat = crit.GetComponent<SpriteRenderer>().material;
            crit_mat.SetInt("_IsSelected", set_val);
        }
    }

    private void ScaleClickWithPodSize()
    {
        _coll.radius = (BASE_RADIUS_SIZE + (POD_RADIUS_PER * MyCritter_List.Count)) * TO_CAPSULE_RADIUS;
        _agent.radius = _coll.radius;
    }
}
