using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    private float _growRate;
    private float _currentAge;
    private float _currentSize;
    public bool CanGrow {get; private set;}

    void Start()
    {
        _currentAge = 0f;
        _currentSize = 1f;
        CanGrow = true;
        _growRate = ForestSetup.Instance.ForestSettings.treeGrowthRate;
    }

    // Update is called once per frame
    void Update()
    {
        _currentAge += Time.deltaTime;
        if(!CanGrow)
            return;

        _currentSize += Time.deltaTime * _growRate;
        transform.localScale = new Vector3(_currentSize, _currentSize, _currentSize);
    }

    public void SignalStopGrowth()
    {
        CanGrow = false;
    }
}
