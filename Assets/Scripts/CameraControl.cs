using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public enum CardinalDir
    {
        NORTH, // +Z
        SOUTH, // -Z
        EAST, // +X
        WEST // -X
    }

    LinkedList<CardinalDir> directionCycle = new LinkedList<CardinalDir>();
    LinkedListNode<CardinalDir> currentDir;


    public float IsoBuffer => ForestSetup.Instance.ArenaSize * 0.6f;
    public float IsoCamHeight => ForestSetup.Instance.ArenaSize * 0.6f;

    Transform mainCamTransform;
    Transform miniMapCamTransform;

    void Awake()
    {
        CardinalDir[] dirs = {CardinalDir.SOUTH, CardinalDir.EAST, CardinalDir.NORTH, CardinalDir.WEST};
        directionCycle = new LinkedList<CardinalDir>(dirs);
    }

    void Start()
    {
        mainCamTransform = Camera.main.transform;
        currentDir = directionCycle.First; // Start at South
        SetupMainCamera( ForestSetup.Instance.ForestSettings.forestScale * 5f);
        SetupMinimapCamera();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
            RotateLeft();

        if(Input.GetKeyDown(KeyCode.E))
            RotateRight();
    }

    public void RotateLeft()
    {
        miniMapCamTransform.Rotate(Vector3.forward, 90);
        if(currentDir.Next == null)
            currentDir = directionCycle.First;
        else
            currentDir = currentDir.Next;
        PositionMainCameraCardinalDirection(currentDir.Value);
    }

    public void RotateRight()
    {
        miniMapCamTransform.Rotate(Vector3.forward, -90);
        if(currentDir.Previous == null)
            currentDir = directionCycle.Last;
        else
            currentDir = currentDir.Previous;
        PositionMainCameraCardinalDirection(currentDir.Value);
    }

    private void SetupMainCamera(float max_pos)
    {
        //Plane Cut Position
        // Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
        //                                  Camera.main.transform.position.y, -max_pos - 5);

        //Overhead Position
        //Camera.main.transform.position = new Vector3(0, max_pos + 5, 0);
        //Camera.main.transform.rotation = Quaternion.Euler(90,0,0);

        //Isometric Position
        PositionMainCameraCardinalDirection(currentDir.Value);
    }

    private void PositionMainCameraCardinalDirection(CardinalDir dir)
    {
        if(dir == CardinalDir.NORTH)
        {
            mainCamTransform.position = new Vector3(0, IsoCamHeight,
                                    ForestSetup.Instance.ArenaSize + IsoBuffer);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
        }
        else if(dir == CardinalDir.SOUTH)
        {
            mainCamTransform.position = new Vector3(0, IsoCamHeight,
                                    -ForestSetup.Instance.ArenaSize - IsoBuffer);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
        }
        else if(dir == CardinalDir.EAST)
        {
            mainCamTransform.position = new Vector3(ForestSetup.Instance.ArenaSize + IsoBuffer,
                                                IsoCamHeight, 0);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
        }
        else if(dir == CardinalDir.WEST)
        {
            mainCamTransform.position = new Vector3(-ForestSetup.Instance.ArenaSize - IsoBuffer,
                                                IsoCamHeight, 0);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
        }
    }

    private void SetupMinimapCamera()
    {
        var minimap_cam = GameObject.Find("Minimap Camera");
        miniMapCamTransform = minimap_cam.transform; //Starts Facing South
        minimap_cam.GetComponent<Camera>().orthographicSize = 5 * ForestSetup.Instance.ForestSettings.forestScale;
    }
}
