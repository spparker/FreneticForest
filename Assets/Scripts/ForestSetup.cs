using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForestSetup : MonoBehaviour
{
    public ForestSettings ForestSettings;
    public GameObject Tree_Prefab;
    public GameObject Critter_Prefab;

    public GameObject HomeTree{ get; private set; }

    public float ArenaSize => ForestSettings.forestScale * 5f;

    public static ForestSetup Instance{ get; private set; }

    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(ForestSettings.forestScale,
                                         1f, ForestSettings.forestScale);

        float max_pos = ArenaSize - ForestSettings.edgeBufferSize;
        HomeTree = Instantiate(Tree_Prefab, Vector3.zero, Quaternion.identity);

        SpawnTrees(max_pos);
        RebuildNavMesh();

        SpawnCritters(max_pos);
    }

    private void SpawnTrees(float max_pos)
    {
        var TreeHolder = new GameObject("Trees").transform;

        for(int i=1;i<ForestSettings.numberOfTrees;i++)
        {
            Vector3 spawn_pos = new Vector3(Random.Range(-max_pos,max_pos),
                                        0, Random.Range(-max_pos, max_pos));
            var new_tree = Instantiate(Tree_Prefab, spawn_pos, Quaternion.identity);
            new_tree.transform.SetParent(TreeHolder);
        }
    }

    private void SpawnCritters(float max_pos)
    {
        var CritterHolder = new GameObject("Critters").transform;

        for(int i=0;i<ForestSettings.numberOfCritters;i++)
        {
            Vector3 spawn_pos = new Vector3(Random.Range(-max_pos,max_pos),
                                        0, Random.Range(-max_pos, max_pos));
            var new_tree = Instantiate(Critter_Prefab, spawn_pos, Quaternion.identity);
            new_tree.transform.SetParent(CritterHolder);
        }

    }

    private void RebuildNavMesh()
    {
        var navSurface = GetComponent<NavMeshSurface>();
        navSurface.BuildNavMesh();
    }

    public struct TreeNode
    {
        public int Id;
        public Vector3 Position;
    }

    public struct TreeConnection
    {
        public float Strength;
        public TreeNode NodeA;
        public TreeNode NodeB;
    }
}
