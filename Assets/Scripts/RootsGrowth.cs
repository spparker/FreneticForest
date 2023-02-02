using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootsGrowth : MonoBehaviour
{
    TreeGrowth treeObject;
    Material rootsMaterial;

    void Awake()
    {
        treeObject = GetComponentInParent<TreeGrowth>();
    }

    void Start()
    {
        rootsMaterial = GetComponent<Renderer>().material;
        
        rootsMaterial.SetFloat(name="_GroundHeight", 0);
    }

    // Stop growth when colliding with another trees roots
    public void OnTriggerEnter(Collider other)
    {
        if(treeObject.CanGrow 
        && other.GetComponent<RootsGrowth>() != null)
            StopGrowth();
    }

    // Shrink? when overlapping with another trees roots
    public void OnTriggerStay(Collider other)
    {
        if(treeObject.CanGrow 
        && other.GetComponent<RootsGrowth>() != null)
            StopGrowth();
    }

    private void StopGrowth()
    {
        treeObject.SignalStopGrowth();
    }
}
