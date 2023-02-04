using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour
{
    TreeGrowth treeObject;
    Material rootsMaterial;

    public float Radius => treeObject.Radius;

    void Awake()
    {
        treeObject = GetComponentInParent<TreeGrowth>();
    }

    void Start()
    {
        rootsMaterial = GetComponent<Renderer>().material;
        
        rootsMaterial.SetFloat("_GroundHeight", 0);
    }

    // Stop growth when colliding with another trees roots
    public void OnTriggerEnter(Collider other)
    {
        if(treeObject.CanGrow 
        && other.GetComponent<Roots>() != null)
            StopGrowth();
    }

    // Shrink? when overlapping with another trees roots
    // THIS GET VERY LAGGY
    /*public void OnTriggerStay(Collider other)
    {
        if(treeObject.CanGrow 
        && other.GetComponent<Roots>() != null)
            StopGrowth();
    }*/

    private void StopGrowth()
    {
        treeObject.SignalStopGrowth();
    }
}
