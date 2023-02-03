using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNetwork : MonoBehaviour
{
    const float DECAY_RATE = 0.005f;
    const float DEATH_POINT = -0.1f;

    const float NODE_JOIN_DIST = 7f;
    const float DEFAULT_NODE_DEPTH = -1f;
    const float PASS_WEIGHT_INCREASE = 0.2f;
    const float MAX_EDGE_WEIGHT = 1.0f;

    const int MAX_EDGES = 8;
    NetworkNode _origin;

    List<NetworkEdge> _edges;
    List<NetworkNode> _nodes;

    private int _edgeCount = 0;
    private int _nodeCount = 0;


    // Start is called before the first frame update
    void Awake()
    {
        _edges = new List<NetworkEdge>();
        _nodes = new List<NetworkNode>();
    }

    void Start()
    {
        var home_root = ForestManager.Instance.HomeTree.GetComponentInChildren<Roots>();
        _origin = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new int[MAX_EDGES],
            root = home_root,
            position = home_root.transform.position
        };
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
        }

        foreach(NetworkEdge del in delEdges)
            KillEdge(del);
    }

    private void KillEdge(NetworkEdge ne)
    {
        RemoveEdgeFromNode(ne.b);
        RemoveEdgeFromNode(ne.a);
        Destroy(ne.objEdge);
        _edges.Remove(ne);
        _edgeCount--;
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

    private NetworkEdge FindEdge(NetworkNode a, NetworkNode b)
    {
        foreach(var e in _edges)
        {
            if((e.a == a || e.b == a) && (e.a == b || e.b == b))
            {
                Debug.Log("Found Edge:" + e.id);
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
            var dist = Vector3.Magnitude(n.position - p1);
            if(dist <= NODE_JOIN_DIST
            && dist < min_dist)
            {
                min_dist = dist;
                nearest = n;
            }
        }

        if(nearest != null)
        {
            Debug.Log("Found Nearby Node: " + nearest.id + "|Root?" + nearest.root);
            return nearest;
        }
        return null;
    }

    // Search Roots without Nodes and create and return if found
    private NetworkNode FindNearbyRoots(Vector3 p, Roots excluded = null)
    {
        Roots nearest = ForestManager.Instance.FindNearestRoots(p, excluded);
        Vector3 nearest_surface = new Vector3(nearest.transform.position.x, 0, nearest.transform.position.z);
        if(Vector3.Magnitude(nearest_surface - p) <= NODE_JOIN_DIST)
        {
            Debug.Log("Found Nearby Root: " + nearest.gameObject.name);
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
        _nodeCount--;
    }

    public NetworkNode CreateNode(Roots root)
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new int[MAX_EDGES],
            root = root,
            position = root.transform.position
        };

        var obj = Instantiate(ForestManager.Instance.Node_Prefab, new_node.position, Quaternion.identity);
        new_node.objNode = obj;

        //Debug.Log("Created node: " + new_node.id);
        _nodes.Add(new_node);

        //return new_node.id;
        return new_node;
    }

    // Create a node by position at the default depth
    public NetworkNode CreateNode(Vector3 position)
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new int[MAX_EDGES],
            root = null,
            position = new Vector3(position.x, DEFAULT_NODE_DEPTH, position.z)
        };

        var obj = Instantiate(ForestManager.Instance.Node_Prefab, new_node.position, Quaternion.identity);
        new_node.objNode = obj;

        _nodes.Add(new_node);

        Debug.Log("Created Non-root node " + new_node.id);

        return new_node;
    }

    public bool AddEdge(int src_id, int dst_id, float dist)
    {
        return AddEdge(_nodes[src_id],_nodes[dst_id], dist);
    }

    private bool AddEdge(NetworkNode src, NetworkNode dst, float distance)
    {
        if(src.numEdges >= MAX_EDGES
        || dst.numEdges >= MAX_EDGES)
            return false;

        NetworkEdge new_edge = new NetworkEdge
        {
            distance = distance,
            weight = 0,
            a = src,
            b = dst,
            id = _edgeCount
        };
        
        var objPos = new_edge.a.position - (new_edge.a.position - new_edge.b.position)/2;
        var obj = Instantiate(ForestManager.Instance.Edge_Prefab, objPos, Quaternion.identity);
        new_edge.objEdge = obj;

        _edges.Add(new_edge);

        _edgeCount++;
        src.edges[src.numEdges++] = new_edge.id;
        dst.edges[dst.numEdges++] = new_edge.id;
        return true;
    }

    private NetworkEdge AddEdge(NetworkNode src, NetworkNode dst)
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

        var objPos = new_edge.a.position - (new_edge.a.position - new_edge.b.position)/2;
        var obj = Instantiate(ForestManager.Instance.Edge_Prefab, objPos, Quaternion.identity);
        new_edge.objEdge = obj;

        _edges.Add(new_edge);

        Debug.Log("Created Edge " + new_edge.id);

        _edgeCount++;
        src.edges[src.numEdges++] = new_edge.id;
        dst.edges[dst.numEdges++] = new_edge.id;
        return new_edge;
    }

    public class NetworkNode
    {
        public int id;
        public int numEdges;
        public int[] edges;
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
