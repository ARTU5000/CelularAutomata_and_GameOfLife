using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Graph : MonoBehaviour
{
    private List<Node> nodes = new List<Node>();
    Node root;

    Queue <Node> NodesToCheck;
    
    public Graph(List<Node> _nodes)
    {
        nodes = _nodes;
        NodesToCheck = new Queue<Node>();
    }
    public Graph()
    {
        NodesToCheck = new Queue<Node>();
    }

        public void SetNodes(List<Node> _nodes)
    {  nodes = _nodes; }

    public List<Node> GetNodes()
    {
        return nodes;
    }

    public void SetRoot(ref Node _root)
    {
        root = _root;
    }

    public Node GetRoot()
    {
        return root;
    }

    public void BFS()
    {
        foreach (Edge e in root.GetEdges())
        {
            e.SetIsVisited(true);
            if(e.GetNodeTo() != root)
            {
                NodesToCheck.Enqueue(e.GetNodeTo());
            }
            if (e.GetNodeFrom() != root)
            {
                NodesToCheck.Enqueue(e.GetNodeTo());
            }
        }

        while (NodesToCheck.Count > 0);
        {
            BFS2(NodesToCheck.First());
        }
    }

    private void BFS2(Node n)
    {
        foreach (Edge e in n.GetEdges())
        {
            e.SetIsVisited(true);
            if (e.GetNodeTo() != n)
            {
                NodesToCheck.Enqueue(e.GetNodeTo());
            }
            if (e.GetNodeFrom() != n)
            {
                NodesToCheck.Enqueue(e.GetNodeTo());
            }
        }
    }

    private void Add(Node n)
    {
        nodes.Add(n);
    }
}
