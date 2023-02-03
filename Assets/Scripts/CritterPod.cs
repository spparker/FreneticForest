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
    private TreeNetwork.NetworkEdge _curEdge;

    private Vector3 _enter_vec;

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

    public void StartPatrol(Vector3 p1, Vector3 p2)
    {
        InPatrol = true;
        if(CritterData.type == CritterManager.CritterType.PATHER)
            SetupForPath(p1, p2);
        else if(CritterData.type == CritterManager.CritterType.DIGGIE)
            SetupForDig();
    }

    public void EndPatrolPass()
    {
        //InPatrol = false;
        if(CritterData.type == CritterManager.CritterType.PATHER)
            CompletePathPass();
        else if(CritterData.type == CritterManager.CritterType.DIGGIE)
            CompleteDigPass();
    }

    public void SetInTree(TreeGrowth tree)
    {
        _enter_vec = tree.transform.position - transform.position;
        _enter_vec.Normalize();
        InTree = tree;
        _agent.enabled = false;
        _coll.enabled = false;
        transform.position = new Vector3(tree.transform.position.x, tree.Top ,tree.transform.position.z);
    }

    public void SetOnGround()
    {
        //WE NEED TO NOT GET STUCK IN THE BIG TREEES COMING DOWN
        transform.position -= _enter_vec * InTree.Radius;
        InTree.LeaveTree();
        InTree = null;

        //transform.position = new Vector3(transform.position.x, 0, transform.position.y);
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

    // Find or create nodes and edge
    private void SetupForPath(Vector3 p1, Vector3 p2)
    {
        _curEdge = ForestManager.Instance.HomeNetwork.GetOrCreateEdge(p1, p2);
        if(_curEdge == null)
            Debug.Log("Cannot add additional edge");
    }

    private void CompletePathPass()
    {
        // Increase weight
        _curEdge?.Strengthen();
        //Debug.Log("Edge "+ _curEdge + " Strengthened to " + _curEdge.weight);
    }

    private void SetupForDig()
    {
        // Do we need to do anything?
    }

    private void CompleteDigPass()
    {
        // Deepend the hole
    }
}
