using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderAI : MonoBehaviour
{
    private CritterCommandControl _ccc;
    private CritterCommandControl _critterTarget;
    private TreeGrowth _treeTarget;

    // Start is called before the first frame update
    void Start()
    {
        _ccc = GetComponent<CritterCommandControl>();
        //AttackRandom();
        ClimbRandom();
    }

    // Update is called once per frame
    void Update()
    {
        //if(_critterTarget == null)
        //    AttackRandom();
    }

    private void AttackRandom()
    {
        _critterTarget = CritterManager.Instance.GetNearestOfRandom(transform.position);
        if(_critterTarget)
            _ccc.QueueAttackCommand(_critterTarget.Pod);
    }
    
    private void ClimbRandom()
    {
        _treeTarget = ForestManager.Instance.FindNearestRoots(transform.position).GetComponentInParent<TreeGrowth>();
        if(_treeTarget)
            _ccc.QueueEnterCommand(_treeTarget);
    }
}
