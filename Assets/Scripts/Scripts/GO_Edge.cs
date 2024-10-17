using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GO_Edge : MonoBehaviour
{
    public GameObject Node1;
    public GameObject Node2;

    public Edge e;

    private void Awake()
    {
        e = new Edge(Node1.GetComponent<GO_Node>().n, Node2.GetComponent<GO_Node>().n, GetDistance());
    }

    float GetDistance()
    {
        return Vector2.Distance(Node1.transform.position,Node2.transform.position);
    }
}
