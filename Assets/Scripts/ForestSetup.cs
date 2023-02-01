using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ForestSetup : MonoBehaviour
{
    public ForestSettings ForestSettings;
    public GameObject Tree_Prefab;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(ForestSettings.forestScale,
                                         1f, ForestSettings.forestScale);
        float max_pos = (ForestSettings.forestScale * 5f) - ForestSettings.edgeBufferSize;

        for(int i=0;i<ForestSettings.numberOfTrees;i++)
        {
            Vector3 spawn_pos = new Vector3(Random.Range(-max_pos,max_pos),
                                        0, Random.Range(-max_pos, max_pos));
            Instantiate(Tree_Prefab, spawn_pos, Quaternion.identity);
        }

        RebuildNavMesh();

        //Plane Cut Position
        Camera.main.transform.position = new Vector3(Camera.main.transform.position.x,
                                         Camera.main.transform.position.y, -max_pos - 5);

        //Overhead Position
        //Camera.main.transform.position = new Vector3(0, max_pos + 5, 0);
        //Camera.main.transform.rotation = Quaternion.Euler(90,0,0);

        PositionMinimapCamera();
    }

    private void RebuildNavMesh()
    {
        var navSurface = GetComponent<NavMeshSurface>();
        navSurface.BuildNavMesh();
    }

    private void PositionMinimapCamera()
    {
        var minimapCam = GameObject.Find("Minimap Camera").GetComponent<Camera>();
        minimapCam.orthographicSize = 5 * ForestSettings.forestScale;
    }
}
