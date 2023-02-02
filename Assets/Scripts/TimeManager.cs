using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private GameObject _directionalLight;
    private float _rotationPerSecond;

    void Awake()
    {
        _directionalLight = GameObject.Find("Directional Light");
    }

    void Start()
    {   
        //360 degrees / (60s/m * mpd) = rotation per second
        _rotationPerSecond = 6f / ForestSetup.Instance.ForestSettings.minutesPerDay;
    }

    void Update()
    {
        // Day / Night Cycle
        _directionalLight.transform.Rotate(Vector3.up, -_rotationPerSecond * Time.deltaTime);
    }
}
