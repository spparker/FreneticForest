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

    public static CameraControl Instance{ get; private set; }

    LinkedList<CardinalDir> directionCycle = new LinkedList<CardinalDir>();
    LinkedListNode<CardinalDir> currentDir; // Direction the camera is placed (eg. SOUTH would be looking north)

    private Vector3 _objToCamNorth;
    private Vector3 _objToCamSouth;
    private Vector3 _objToCamEast;
    private Vector3 _objToCamWest;


    public float IsoBuffer => ForestManager.Instance.ArenaSize * 0.6f;
    public float IsoCamHeight => ForestManager.Instance.ArenaSize * 1f;

    Transform mainCamTransform;
    Transform miniMapCamTransform;
    private float _arenaMax;


    public const float ZOOM_RATE = 7f;
    public const float MIN_ZOOM_DIST = 1.2f;
    private float _curZoom = 1;

    public const float PAN_RATE = 40f;
    private Vector3 _curLookPos; // Current spot on map that is targeted



    void Awake()
    {
        if(Instance != null && Instance != this)
            Destroy(this);
        else
            Instance = this;
        
        CardinalDir[] dirs = {CardinalDir.SOUTH, CardinalDir.EAST, CardinalDir.NORTH, CardinalDir.WEST};
        directionCycle = new LinkedList<CardinalDir>(dirs);
    }

    void Start()
    {
        mainCamTransform = Camera.main.transform;
        _curLookPos = new Vector3(ForestManager.Instance.HomeTree.transform.position.x, 
                                    ForestManager.Instance.HomeTree.transform.position.y,
                                    ForestManager.Instance.HomeTree.transform.position.z);

        currentDir = directionCycle.First; // Start at South
        _arenaMax = ForestManager.Instance.ArenaSize;
        SetupMainCamera( _arenaMax );
        SetupMinimapCamera();
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
        if(Input.GetKey(KeyCode.W))
            PanForward();
        if(Input.GetKey(KeyCode.S))
            PanBackward();
        if(Input.GetKey(KeyCode.A))
            PanLeft();
        if(Input.GetKey(KeyCode.D))
            PanRight();

        UpdateCameraPosition(currentDir.Value);

        // Effects
        if(Input.GetKeyDown(KeyCode.Space))
            ForestManager.Instance.ToggleSurface();
    }

    public void LookAtPosition(Vector3 pos)
    {
        _curLookPos = pos;
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
        _curZoom = Mathf.Clamp(_curZoom - Input.mouseScrollDelta.y * ZOOM_RATE * Time.deltaTime
                                , (MIN_ZOOM_DIST/ _arenaMax ), 1.0f);
    }

    private void PanForward()
    {
        PanCamera(Vector3.ProjectOnPlane(mainCamTransform.forward, Vector3.up) * PAN_RATE * Time.deltaTime);
    }

    private void PanBackward()
    {
         PanCamera(-Vector3.ProjectOnPlane(mainCamTransform.forward, Vector3.up) * PAN_RATE * Time.deltaTime);
    }
    private void PanLeft()
    {
        PanCamera(-Vector3.ProjectOnPlane(mainCamTransform.right, Vector3.up) * PAN_RATE * Time.deltaTime);
    }
    private void PanRight()
    {
        PanCamera(Vector3.ProjectOnPlane(mainCamTransform.right, Vector3.up) * PAN_RATE * Time.deltaTime);
    }

    // Keep Movement inside arena, ensure no vertical component
    private void PanCamera(Vector3 delta)
    {
        Vector3 next_position = _curLookPos + delta;
        float x = Mathf.Clamp(next_position.x, -_arenaMax, _arenaMax);
        float z = Mathf.Clamp(next_position.z, -_arenaMax, _arenaMax);
        _curLookPos = new Vector3(x, 0, z);
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
                                    _arenaMax + IsoBuffer);
            mainCamTransform.LookAt(ForestManager.Instance.HomeTree.transform);
            if(_objToCamNorth == Vector3.zero)
                _objToCamNorth = ForestManager.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
        else if(dir == CardinalDir.SOUTH)
        {
            mainCamTransform.position = new Vector3(0, IsoCamHeight,
                                    -_arenaMax - IsoBuffer);
            mainCamTransform.LookAt(ForestManager.Instance.HomeTree.transform);
            if(_objToCamSouth == Vector3.zero)
                _objToCamSouth = ForestManager.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
        else if(dir == CardinalDir.EAST)
        {
            mainCamTransform.position = new Vector3(_arenaMax + IsoBuffer,
                                                IsoCamHeight, 0);
            mainCamTransform.LookAt(ForestManager.Instance.HomeTree.transform);
            if(_objToCamEast == Vector3.zero)
                _objToCamEast = ForestManager.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
        else if(dir == CardinalDir.WEST)
        {
            mainCamTransform.position = new Vector3(-_arenaMax - IsoBuffer,
                                                IsoCamHeight, 0);
            mainCamTransform.LookAt(ForestManager.Instance.HomeTree.transform);
            if(_objToCamWest == Vector3.zero)
                _objToCamWest = ForestManager.Instance.HomeTree.transform.position + mainCamTransform.position;
        }
    }

    private void SetupMinimapCamera()
    {
        var minimap_cam = GameObject.Find("Minimap Camera");
        miniMapCamTransform = minimap_cam.transform; //Starts Facing South
        minimap_cam.GetComponent<Camera>().orthographicSize = 5 * ForestManager.Instance.ForestSettings.forestScale;
    }
}
