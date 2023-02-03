using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrowth : MonoBehaviour
{
    public const float INITIAL_RADIUS = 0.7f;

    private float _growRate;
    private float _currentAge;
    private float _currentSize = 1;
    public CritterCommandControl OccupyingCritters{get; private set;}

    public bool CanEnter => !OccupyingCritters;
    public void EnterTree(CritterCommandControl ccc){OccupyingCritters = ccc;}
    public void LeaveTree(){OccupyingCritters = null;}
    public bool CanGrow {get; private set;}
    public float Top => _currentSize;
    public float Radius => _currentSize * INITIAL_RADIUS;

    void Start()
    {
        _currentAge = 0f;
        CanGrow = true;
        _growRate = ForestManager.Instance.ForestSettings.treeGrowthRate;
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

    public void SetStartingSize(float size)
    {
        _currentSize = size;
    }

    public void SignalStopGrowth()
    {
        CanGrow = false;
    }
}
