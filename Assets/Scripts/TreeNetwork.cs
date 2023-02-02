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
        _origin = new NetworkNode()
        {
            id = _nodeCount,
            numEdges = 0,
            edges = new int[MAX_EDGES]
        };

        _edges = new List<NetworkEdge>();
        _nodes = new List<NetworkNode>{ _origin };
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
        }
    }

    private void RemoveEdgeFromNode(NetworkNode nn)
    {
        if(nn.numEdges <= 1)
            KillNode(nn);
        else
            nn.numEdges--;
    }

    private void KillNode(NetworkNode nn)
    {
        _nodes.Remove(nn);
    }

    public int CreateNode()
    {
        var new_node = new NetworkNode()
        {
            id = _nodeCount,
            numEdges = 0,
            edges = new int[MAX_EDGES]
        };

        _nodeCount++;
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

    public struct NetworkNode
    {
        public int id;
        public int numEdges;
        public int[] edges;
    }

    public class NetworkEdge
    {
        public int id;
        public float weight;
        public float distance;
        public NetworkNode a;
        public NetworkNode b;
    }
}
