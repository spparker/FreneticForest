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
    LinkedListNode<CardinalDir> currentDir; // Direction the camera is placed (eg. SOUTH would be looking north)

    private Vector3 _objToCamNorth;
    private Vector3 _objToCamSouth;
    private Vector3 _objToCamEast;
    private Vector3 _objToCamWest;


    public float IsoBuffer => ForestSetup.Instance.ArenaSize * 0.6f;
    public float IsoCamHeight => ForestSetup.Instance.ArenaSize * 1f;

    Transform mainCamTransform;
    Transform miniMapCamTransform;


    public const float ZOOM_RATE = 0.05f;
    public const float MIN_ZOOM_DIST = 0.1f;
    //private float _maxZoomDistance; // Find from initial setup
    private Vector3 _curLookPos; // Current spot on map that is targeted
    private float _curZoom = 1;

    void Awake()
    {
        CardinalDir[] dirs = {CardinalDir.SOUTH, CardinalDir.EAST, CardinalDir.NORTH, CardinalDir.WEST};
        directionCycle = new LinkedList<CardinalDir>(dirs);
    }

    void Start()
    {
        mainCamTransform = Camera.main.transform;
        _curLookPos = new Vector3(ForestSetup.Instance.HomeTree.transform.position.x, 
                                    ForestSetup.Instance.HomeTree.transform.position.y,
                                    ForestSetup.Instance.HomeTree.transform.position.z);

        currentDir = directionCycle.First; // Start at South
        SetupMainCamera( ForestSetup.Instance.ForestSettings.forestScale * 5f);
        SetupMinimapCamera();

        //_maxZoomDistance = Vector3.Magnitude(_curLookPos - mainCamTransform.position);
    }

    void Update()
    {
        // Map Zoom
        CheckForZoom();

        // Map Rotation
        if(Input.GetKeyDown(KeyCode.Q))
            RotateLeft();

        if(Input.GetKeyDown(KeyCode.E))
            RotateRight();

        // Map Panning
        /*if(Input.GetKeyDown(KeyCode.W))
            PanForward();

        if(Input.GetKeyDown(KeyCode.S))
            PanBackwards();

        if(Input.GetKeyDown(KeyCode.A))
            PanLeft();

        if(Input.GetKeyDown(KeyCode.D))
            PanRight();*/

        UpdateCameraPosition(currentDir.Value);
    }

    private void UpdateCameraPosition(CardinalDir dir)
    {
        if(dir == CardinalDir.NORTH && _objToCamNorth != null)
        {
            mainCamTransform.position = _curLookPos + _objToCamNorth * _curZoom;
            mainCamTransform.LookAt(_curLookPos);
        }
        else if(dir == CardinalDir.SOUTH && _objToCamSouth != null)
        {
            mainCamTransform.position = _curLookPos + _objToCamSouth * _curZoom;
            mainCamTransform.LookAt(_curLookPos);
        }
        else if(dir == CardinalDir.EAST && _objToCamEast != null)
        {
            mainCamTransform.position = _curLookPos + _objToCamEast * _curZoom;
            mainCamTransform.LookAt(_curLookPos);
        }
        else if(dir == CardinalDir.WEST && _objToCamWest != null)
        {
            mainCamTransform.position = _curLookPos + _objToCamWest * _curZoom;
            mainCamTransform.LookAt(_curLookPos);
        }
    }

    private void CheckForZoom()
    {
        _curZoom = Mathf.Clamp(_curZoom - Input.mouseScrollDelta.y * ZOOM_RATE
                                , MIN_ZOOM_DIST, 1.0f);
    }

    private void RotateLeft()
    {
        miniMapCamTransform.Rotate(Vector3.forward, 90);
        if(currentDir.Next == null)
            currentDir = directionCycle.First;
        else
            currentDir = currentDir.Next;
        PositionMainCameraCardinalDirection(currentDir.Value);
        //_curZoom = 1.0f;
    }

    private void RotateRight()
    {
        miniMapCamTransform.Rotate(Vector3.forward, -90);
        if(currentDir.Previous == null)
            currentDir = directionCycle.Last;
        else
            currentDir = currentDir.Previous;
        PositionMainCameraCardinalDirection(currentDir.Value);
        //_curZoom = 1.0f;
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
            if(_objToCamNorth == Vector3.zero)
                _objToCamNorth = ForestSetup.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
        else if(dir == CardinalDir.SOUTH)
        {
            mainCamTransform.position = new Vector3(0, IsoCamHeight,
                                    -ForestSetup.Instance.ArenaSize - IsoBuffer);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
            if(_objToCamSouth == Vector3.zero)
                _objToCamSouth = ForestSetup.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
        else if(dir == CardinalDir.EAST)
        {
            mainCamTransform.position = new Vector3(ForestSetup.Instance.ArenaSize + IsoBuffer,
                                                IsoCamHeight, 0);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
            if(_objToCamEast == Vector3.zero)
                _objToCamEast = ForestSetup.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
        else if(dir == CardinalDir.WEST)
        {
            mainCamTransform.position = new Vector3(-ForestSetup.Instance.ArenaSize - IsoBuffer,
                                                IsoCamHeight, 0);
            mainCamTransform.LookAt(ForestSetup.Instance.HomeTree.transform);
            if(_objToCamWest == Vector3.zero)
                _objToCamWest = ForestSetup.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
    }

    private void SetupMinimapCamera()
    {
        var minimap_cam = GameObject.Find("Minimap Camera");
        miniMapCamTransform = minimap_cam.transform; //Starts Facing South
        minimap_cam.GetComponent<Camera>().orthographicSize = 5 * ForestSetup.Instance.ForestSettings.forestScale;
    }
}
