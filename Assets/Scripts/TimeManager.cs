using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private GameObject _directionalLight;
    private GameObject _moonLight;
    private float _rotationPerSecond;

    void Awake()
    {
        _directionalLight = GameObject.Find("Directional Light");
        _moonLight = GameObject.Find("Moon Light");
        _moonLight.SetActive(false);
    }

    void Start()
    {   
        //360 degrees / (60s/m * mpd) = rotation per second
        _rotationPerSecond = 6f / ForestManager.Instance.ForestSettings.minutesPerDay;

        //Rewind half day
        _directionalLight.transform.Rotate(Vector3.up, 90f);
    }

    void Update()
    {
        Debug.Log("x: " + _directionalLight.transform.rotation.eulerAngles.x);
        if(_directionalLight.transform.rotation.eulerAngles.x > 280)
            _moonLight.SetActive(true);
        else
            _moonLight.SetActive(false);

        // Day / Night Cycle
        _directionalLight.transform.Rotate(Vector3.up, -_rotationPerSecond * Time.deltaTime);


    }
}
