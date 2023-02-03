using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForestManager : MonoBehaviour
{
    public const float SECONDS_BETWEEN_REBUILD = 20f;
    public ForestSettings ForestSettings;

    public GameObject Tree_Prefab;
    public GameObject Node_Prefab;
    public GameObject Edge_Prefab;

    public GameObject HomeTree{ get; private set; }
    public TreeNetwork HomeNetwork{ get; private set;}

    private List<GameObject> _trees;

    public float ArenaSize => ForestSettings.forestScale * 5f;

    public static ForestManager Instance{ get; private set; }

    public NavMeshSurface NavSurface;
    public NavMeshSurface BurrowSurface;
    public NavMeshSurface JumperSurface;
    private MeshRenderer _meshRenderer;

    private float _timeSinceRebuild = 0;
    private float max_pos;

    public const float TRANSITION_RATE = 0.5f;
    public const float SLOW_THRESHOLD = 0.95f; // Lowerbound of slow transition
    private float _alpha = 1;
    private bool _inTransition;
    private int _transDir = 1;

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
        _meshRenderer = GetComponent<MeshRenderer>();
        transform.localScale = new Vector3(ForestSettings.forestScale,
                                         1f, ForestSettings.forestScale);
        BurrowSurface.gameObject.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        JumperSurface.gameObject.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        max_pos = ArenaSize - ForestSettings.edgeBufferSize;

        HomeTree = Instantiate(Tree_Prefab, Vector3.zero, Quaternion.identity);
        HomeTree.GetComponent<TreeGrowth>().SetStartingSize(ForestSettings.homeTreeStartingSize);
        HomeNetwork = gameObject.AddComponent<TreeNetwork>();
        HomeNetwork.CreateNode(HomeTree.GetComponentInChildren<Roots>());
        _trees = new List<GameObject>();

        SpawnTrees();
        RebuildNavMesh();

        SpawnCritters();

        InitialBranchOut();

        SpawnEnemy();
    }

    void Update()
    {
        _timeSinceRebuild += Time.deltaTime;

        if(_timeSinceRebuild >= SECONDS_BETWEEN_REBUILD)
            RebuildNavMesh();

        if(_inTransition)
            UpdateSurfaceOpacity();
    }

    private void InitialBranchOut()
    {
        //var nearest_neighbor = FindNearestRootsPosition(HomeTree.transform.position);
        var nearest_roots = FindNearestRoots(HomeTree.transform.position);

        //var nearest_neighbor.FindTreeById(1);

        HomeNetwork.CreateNode(nearest_roots);
        HomeNetwork.AddEdge(0, 1, Vector3.Magnitude(HomeTree.transform.position - nearest_roots.transform.position));

        Debug.Log("Added Nearest Neighbor at: " + nearest_roots.transform.position);
    }

    public Roots FindNearestRoots(Vector3 pos, Roots excluded = null)
    {
        float min_dist = 99999;
        Roots min_roots = null;
        foreach(var tree in _trees)
        {
            var root = tree.GetComponentInChildren<Roots>();
            if(root == excluded)
                continue;

            var dist = Vector3.SqrMagnitude(pos - tree.transform.position);
            //Debug.Log("Checking " + tree + "|" + dist);
            if(dist < min_dist)
            {
                min_dist = dist;
                min_roots = root;
            }
        }
        return min_roots;
    }

    // Return nearest tree position to a tree
    private Vector3 FindNearestRootsPosition(Vector3 pos)
    {
        float min_dist = 99999;
        GameObject min_tree = null;
        foreach(var tree in _trees)
        {
            var dist = Vector3.SqrMagnitude(pos - tree.transform.position);
            //Debug.Log("Checking " + tree + "|" + dist);
            if(dist < min_dist)
            {
                min_dist = dist;
                min_tree = tree;
            }
        }
        return min_tree.transform.position;
    }

    public void ToggleSurface()
    {
        //_meshRenderer.enabled = !_meshRenderer.enabled;
        _transDir = -_transDir;
        _inTransition = true;
    }

    private void UpdateSurfaceOpacity()
    {
        var delta = _transDir * TRANSITION_RATE * Time.deltaTime;
        if(_alpha > SLOW_THRESHOLD)
            delta *= 0.1f;

        _alpha += delta;
        if(_alpha > 1)
        {
            _alpha = 1;
            _inTransition = false;
        }
        else if(_alpha < 0)
        {
            _alpha = 0;
            _inTransition = false;
        }

        _meshRenderer.material.color = new Color(_meshRenderer.material.color.r,
        _meshRenderer.material.color.g, _meshRenderer.material.color.b, _alpha);
    }

    private void SpawnTrees()
    {
        var TreeHolder = new GameObject("Trees").transform;

        for(int i=1;i<ForestSettings.numberOfTrees;i++)
        {
            Vector3 spawn_pos = new Vector3(Random.Range(-max_pos,max_pos),
                                        0, Random.Range(-max_pos, max_pos));
            var new_tree = Instantiate(Tree_Prefab, spawn_pos, Quaternion.identity);
            new_tree.transform.SetParent(TreeHolder);
            new_tree.transform.name = "Tree" + i;
            _trees.Add(new_tree);
        }
    }

    private void SpawnCritters()
    {
        CritterManager.Instance.SpawnCritterFromData(ForestSettings.critters.patherData, ForestSettings.numberOfPathers, max_pos);
        CritterManager.Instance.SpawnCritterFromData(ForestSettings.critters.diggieData, ForestSettings.numberOfDiggies, max_pos);
        CritterManager.Instance.SpawnCritterFromData(ForestSettings.critters.chopData, ForestSettings.numberOfChopChops, max_pos);
    }

    private void SpawnEnemy()
    {
        CritterManager.Instance.SpawnCritterFromData(ForestSettings.critters.invaderData, 1, max_pos);
    }

    public void RebuildNavMesh()
    { // TODO: This is too slow now on big maps
        NavSurface.BuildNavMesh();
        //BurrowSurface.navMeshData = NavSurface.navMeshData;
        BurrowSurface.BuildNavMesh(); // will need to rebuild for better burrow movement
        JumperSurface.BuildNavMesh();
        _timeSinceRebuild = 0;
    }
}
