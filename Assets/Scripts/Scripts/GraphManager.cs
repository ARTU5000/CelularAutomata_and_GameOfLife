using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public List<GameObject> nodes;
    public List<GameObject> edges;
    Graph graph;

    private void Start()
    {
        graph = new Graph();
    }

    void IterateNodes()
    {
        foreach (GameObject node in nodes)
        {
            graph.Add(node.GetComponent<GO_Node>().n);
        }
    }
}
