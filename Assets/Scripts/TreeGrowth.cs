using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    public const float GROW_RATE = 0.2f;
    private float _currentAge;
    private float _currentSize;
    public bool CanGrow {get; private set;}

    void Start()
    {
        _currentAge = 0f;
        _currentSize = 1f;
        CanGrow = true;
    }

    // Update is called once per frame
    void Update()
    {
        _currentAge += Time.deltaTime;
        if(!CanGrow)
            return;

        _currentSize += Time.deltaTime * GROW_RATE;
        transform.localScale = new Vector3(_currentSize, _currentSize, _currentSize);
    }

    public void SignalStopGrowth()
    {
        CanGrow = false;
    }
}
