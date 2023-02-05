using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Roots : MonoBehaviour
{
    public TreeGrowth Tree {get; private set;}
    Material rootsMaterial;
    GameObject rootMask;

    void Awake()
    {
        Tree = GetComponentInParent<TreeGrowth>();
        rootMask = transform.Find("RootsMask").gameObject;
    }

    void Start()
    {
        rootsMaterial = GetComponent<Renderer>().material;
        
        rootsMaterial.SetFloat("_GroundHeight", 0);
    }

    void Update()
    {
        if(Tree.BurrowedCritters)
        {
            rootMask.SetActive(true);
            var flat_to_cam = Vector3.ProjectOnPlane(Camera.main.transform.position - Tree.transform.position, Vector3.up);
            flat_to_cam.Normalize();
            rootMask.transform.position = new Vector3(transform.position.x, rootMask.transform.position.y, transform.position.z) + flat_to_cam * Tree.Radius;
        }
        else
            rootMask.SetActive(false);
    }

    // Stop growth when colliding with another trees roots
    public void OnTriggerEnter(Collider other)
    {
        if(Tree.CanGrow 
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
        Tree.SignalStopGrowth();
    }
}
