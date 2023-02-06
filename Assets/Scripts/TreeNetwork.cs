using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNetwork : MonoBehaviour
{
    const float DECAY_RATE = 0.0044f;
    const float DEATH_POINT = -0.3f;

    const float NODE_JOIN_DIST = 8f;
    const float DEFAULT_NODE_DEPTH = -1f;
    const float PASS_WEIGHT_INCREASE = 0.22f;
    const float MAX_EDGE_WEIGHT = 1.0f;

    const float BASE_EDGE_RAD = 0.1f;
    const float MAX_EDGE_RAD = 1.3f;

    const int MAX_EDGES = 8;
    NetworkNode _origin;

    List<NetworkEdge> _edges;
    List<NetworkNode> _nodes;

    public NetworkNode Origin => _nodes[0];
    public NetworkNode NewestNode => _nodes[^1];

    // Always incrementing ID system, not actual count
    private int _edgeCount = 0;
    private int _nodeCount = 0;

    private float _updateCounter;
    private const float UPDATE_RATE = 1;

    public int TotalSize {get; private set;} // Number of Rooted Nodes
    public float TotalWisdom {get; private set;} // Combined Ages
    public float TotalStress {get; private set;} // Something Minus Stress?

    private List<int> EdgesThisPass; // Tracking traveled edges each iteration
    private List<int> NodesThisPass;


    // Start is called before the first frame update
    void Awake()
    {
        _edges = new List<NetworkEdge>();
        _nodes = new List<NetworkNode>();
    }

    void Start()
    {
        _origin = _nodes[0];
    }

    // Update is called once per frame
    void Update()
    {
        List<NetworkEdge> delEdges = new List<NetworkEdge>();
        foreach(NetworkEdge ne in _edges)
        {
            ne.weight -= Time.deltaTime * DECAY_RATE;
            if(ne.weight <= DEATH_POINT)
                delEdges.Add(ne);
            UpdateEdges(ne);
        }

        foreach(NetworkEdge del in delEdges)
            KillEdge(del);

        _updateCounter += Time.deltaTime;
        if(_updateCounter >= UPDATE_RATE)
            UpdateStats();
    }

    private void UpdateStats()
    {
        _updateCounter = 0;
        EdgesThisPass = new List<int>();
        NodesThisPass = new List<int>();

        TotalSize = 0;
        TotalWisdom = 0;
        TotalStress = 0;

        TravelNodeRecursive(_origin);
    }

    private void TravelNodeRecursive(NetworkNode node)
    {
        //Debug.Log("At Node: " + node.id);
        AddNodeScores(node);
        foreach(NetworkEdge edge in node.edges)
        {
            //Debug.Log("(" + node.id + ")Checking Edge: " + edge.id);
            if(EdgesThisPass.Contains(edge.id))
                continue;
            
            EdgesThisPass.Add(edge.id);
            NetworkNode next = edge.a;
            if(next == node)
                next = edge.b;
            
            if(!NodesThisPass.Contains(next.id))
                TravelNodeRecursive(next);
        }
    }

    private void AddNodeScores(NetworkNode node)
    {
        NodesThisPass.Add(node.id);
        var node_root = node.root;
        if(node_root)
        {
            TotalSize += 1;
            TotalWisdom += node_root.Tree.Age;
            TotalStress += node_root.Tree.StressLevel;
        }
    }

    private void UpdateEdges(NetworkEdge ne)
    {
        UpdateRootedNodePosition(ne.a);
        UpdateRootedNodePosition(ne.b);
        UpdateEdgeRotation(ne);
        ScaleEdgeToStrength(ne);
    }

    // Keep node at root depth
    private void UpdateRootedNodePosition(NetworkNode n)
    {
        if(n.root == null)
            return;

        n.position = n.root.transform.position;
        n.objNode.transform.position = n.position;
    }

    // Tilt to the new nodes
    private void UpdateEdgeRotation(NetworkEdge ne)
    {
        var objPos = ne.a.position - (ne.a.position - ne.b.position)/2;
        ne.objEdge.transform.position = objPos;
        ne.objEdge.transform.LookAt(ne.a.position);
        ne.objEdge.transform.Rotate(90,0,0);
    }

    // Scale visual edge radius to connection weight
    private void ScaleEdgeToStrength(NetworkEdge ne)
    {
        var radius = BASE_EDGE_RAD + ne.weight * MAX_EDGE_RAD;
        ne.objEdge.transform.localScale = new Vector3(radius, ne.objEdge.transform.localScale.y, radius);
    }

    private void KillEdge(NetworkEdge ne)
    {
        RemoveEdgeFromNode(ne.b);
        RemoveEdgeFromNode(ne.a);
        Destroy(ne.objEdge);
        _edges.Remove(ne);
        //_edgeCount--;
    }

    // Assume points are flattened to surface here
    public NetworkEdge GetOrCreateEdge(Vector3 p1, Vector3 p2)
    {
        NetworkNode node1 = BestOrNewNode(p1);
        NetworkNode node2 = BestOrNewNodeExcluding(p2, node1);
        
        NetworkEdge edge = FindEdge(node1, node2);
        edge ??= AddEdge(node1, node2);
        return edge;
    }

    private NetworkEdge FindEdge(NetworkNode n1, NetworkNode n2)
    {
        foreach(var e in _edges)
        {
            if((e.a == n1 || e.b == n1) && (e.a == n2 || e.b == n2))
            {
                //Debug.Log("Found Existing Edge:" + e.id);
                return e;
            }
        }
        return null;
    }

    private NetworkNode BestOrNewNodeExcluding(Vector3 p, NetworkNode excluded)
    {
        var ret_node = FindNearbyNode(p, excluded);
        ret_node ??= FindNearbyRoots(p, excluded.root);
        ret_node ??= CreateNode(p);

        return ret_node;
    }

    private NetworkNode BestOrNewNode(Vector3 p)
    {
        var ret_node = FindNearbyNode(p);
        ret_node ??= FindNearbyRoots(p);
        ret_node ??= CreateNode(p);

        return ret_node;
    }

    private NetworkNode FindNearbyNode(Vector3 p1, NetworkNode excluded = null)
    {
        float min_dist = 999;
        NetworkNode nearest = null;
        foreach(var n in _nodes)
        {
            if(n == excluded)
                continue;
            Vector3 nearest_surface = new Vector3(n.position.x, 0, n.position.z);
            var dist = Vector3.Magnitude(nearest_surface - p1);
            float add = 0;
            if(n.root)
                add = n.root.Tree.Radius;
            if(dist <= NODE_JOIN_DIST + add
            && dist < min_dist)
            {
                min_dist = dist;
                nearest = n;
            }
        }

        if(nearest != null)
        {
            //Debug.Log("Found Nearby Node: " + nearest.id + "|Root?" + nearest.root);
            return nearest;
        }
        return null;
    }

    // Search Roots without Nodes and create and return if found
    private NetworkNode FindNearbyRoots(Vector3 p, Roots excluded = null)
    {
        Roots nearest = ForestManager.Instance.FindNearestRoots(p, excluded);
        Vector3 nearest_surface = new Vector3(nearest.transform.position.x, 0, nearest.transform.position.z);
        //Debug.Log("Found nearest Root: " + nearest.transform.parent.name + " @ " + Vector3.Magnitude(nearest_surface - p) );

        if(Vector3.Magnitude(nearest_surface - p) <= NODE_JOIN_DIST + nearest.Tree.Radius)
        {
            //Debug.Log("Found Nearby Root: " + nearest.gameObject.name);
            return CreateNode(nearest); // If it had one we would've found it on Step 1
        }

        return null;
    }

    private void RemoveEdgeFromNode(NetworkNode nn)
    {
        //Debug.Log("Removing Edge from node " +  nn.id);
        nn.numEdges--;

        if(nn.numEdges <= 0 && nn.id != 0)
            KillNode(nn);
    }

    private void KillNode(NetworkNode nn)
    {
        Destroy(nn.objNode);
        _nodes.Remove(nn);
    }

    public NetworkNode CreateNode(Roots root)
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new List<NetworkEdge>(),
            root = root,
            position = root.transform.position
        };

        new_node.objNode = SpawnNodeObject(new_node.position);
        _nodes.Add(new_node);

        //Debug.Log("Created Root node " + new_node.id + " on Roots of " + root);
        return new_node;
    }

    // Create a node by position at the default depth
    public NetworkNode CreateNode(Vector3 position)
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new List<NetworkEdge>(),
            root = null,
            position = new Vector3(position.x, DEFAULT_NODE_DEPTH, position.z)
        };

        new_node.objNode = SpawnNodeObject(new_node.position);
        _nodes.Add(new_node);

        //Debug.Log("Created Non-root node " + new_node.id);
        return new_node;
    }

    private GameObject SpawnNodeObject(Vector3 pos)
    {
        var obj = Instantiate(ForestManager.Instance.Node_Prefab, pos, Quaternion.identity);
        obj.transform.SetParent(ForestManager.Instance.HomeNetwork.transform);
        return obj;
    }

    public NetworkEdge AddEdge(NetworkNode src, NetworkNode dst)
    {
        if(src.numEdges >= MAX_EDGES
        || dst.numEdges >= MAX_EDGES)
            return null;

        var dist = Vector3.Magnitude(src.position-dst.position);

        NetworkEdge new_edge = new NetworkEdge
        {
            distance = dist,
            weight = 0,
            a = src,
            b = dst,
            id = _edgeCount
        };

        new_edge.objEdge = SpawnEdgeObject(new_edge.a.position, new_edge.b.position);
        _edges.Add(new_edge);

        //Debug.Log("Created Edge " + new_edge.id + " from " + src.id + " to " + dst.id);

        _edgeCount++;
        src.numEdges++;
        src.edges.Add(new_edge);
        dst.numEdges++;
        dst.edges.Add(new_edge);
        return new_edge;
    }

    private GameObject SpawnEdgeObject(Vector3 a, Vector3 b)
    {
        var objPos = a - (a - b)/2;
        var obj = Instantiate(ForestManager.Instance.Edge_Prefab, objPos, Quaternion.identity);

        var mag = Vector3.Magnitude(a - b); // Divide by 2 for cylinder height
        obj.transform.localScale = new Vector3(obj.transform.localScale.x, mag / 2, obj.transform.localScale.z);
        obj.transform.LookAt(a);
        obj.transform.Rotate(90,0,0);

        obj.transform.SetParent(ForestManager.Instance.HomeNetwork.transform);
        return obj;
    }

    public class NetworkNode
    {
        public int id;
        public int numEdges;
        //public int[] edges;
        public List<NetworkEdge> edges;
        public Roots root;
        public Vector3 position;
        public GameObject objNode;
    }

    public class NetworkEdge
    {
        public int id;
        public float weight;
        public float distance;
        public NetworkNode a;
        public NetworkNode b;
        public GameObject objEdge;

        public void Strengthen()
        {
            weight += PASS_WEIGHT_INCREASE;
            if(weight > MAX_EDGE_WEIGHT)
                weight = MAX_EDGE_WEIGHT;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        foreach(var n in _nodes)
        {
            // Draws a blue line from this transform to the target
            Gizmos.DrawSphere(n.position, 0.5f);
        }
        foreach(var e in _edges)
        {
            // Draws a blue line from this transform to the target
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(e.a.position, e.b.position);
        }
    }
}
