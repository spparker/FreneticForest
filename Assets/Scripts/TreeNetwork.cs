using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeNetwork : MonoBehaviour
{
    const float DECAY_RATE = 0.01f;
    const float DEATH_POINT = -0.1f;

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
        {
            RemoveEdgeFromNode(del.b);
            RemoveEdgeFromNode(del.a);
            _edges.Remove(del);
            _edgeCount--;
        }
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
        _nodes.Remove(nn);
        _nodeCount--;
    }

    public int CreateNode(Roots root)
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new int[MAX_EDGES],
            root = root,
            position = root.transform.position
        };

        //Debug.Log("Created node: " + new_node.id);
        _nodes.Add(new_node);

        return new_node.id;
    }

    public int CreateNode(Vector3 position)
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount++,
            numEdges = 0,
            edges = new int[MAX_EDGES],
            root = null,
            position = position
        };

        _nodes.Add(new_node);

        return new_node.id;
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
        _edges.Add(new_edge);

        _edgeCount++;
        src.edges[src.numEdges++] = new_edge.id;
        dst.edges[dst.numEdges++] = new_edge.id;
        return true;
    }

    public class NetworkNode
    {
        public int id;
        public int numEdges;
        public int[] edges;
        public Roots root;
        public Vector3 position;
    }

    public class NetworkEdge
    {
        public int id;
        public float weight;
        public float distance;
        public NetworkNode a;
        public NetworkNode b;
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
