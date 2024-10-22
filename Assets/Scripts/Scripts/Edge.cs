using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Edge
{
    private Node nodeFrom;
    private Node nodeTo;
    private bool isVisited;
    private float peso;

    public Edge(Node _nodeFrom, Node _nodeTo, float _peso)
    {
        nodeFrom = _nodeFrom;
        nodeTo = _nodeTo;
        isVisited = false;
        peso = _peso;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void SetNodeTo(Node _nodeTo)
    {
        nodeTo = _nodeTo;
    }
    public Node GetNodeTo()
    {  return null; }

    public void SetNodeFrom(Node _nodeFrom)
    {
        nodeFrom = _nodeFrom;
    }
    public Node GetNodeFrom()
    { return nodeFrom; }

    public void SetIsVisited(bool visited)
    {
        isVisited = visited;
    }

    public bool IsVisited()
    {  return isVisited; }

    public void SetPeso(float _peso)
    {
        peso = _peso;
    }

    public float GetPeso()
    {  return peso; }
}
