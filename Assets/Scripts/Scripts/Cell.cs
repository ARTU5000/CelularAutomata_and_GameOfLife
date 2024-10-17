using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    private float value;
    private int id;
    private List<Edge> edges;

    public Node(float value, int id, List<Edge> edges)
    {
        this.value = value;
        this.id = id;
        this.edges = edges;
    }

    public float GetValue()
    {
        return value;
    }

    public void SetValue(float _value)
    {
        value = _value;
    }

    public int GetId()
    {
        return id;
    }

    public void SetID(int _id)
    {
        id = _id;
    }

    public List<Edge> GetEdges()
    {
        return edges;
    }

    public void SetEdges(List<Edge> _edges)
    {
        edges = _edges;
    }
}
