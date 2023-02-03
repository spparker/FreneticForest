using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal class CommandEnter : CritterCommand
{
    public const float DELTA_FROM_TARGET = 0.6f;
    private readonly TreeGrowth _tree;
    private readonly CritterCommandControl _ccc;
    private readonly NavMeshAgent _agent;

    public CommandEnter(TreeGrowth tree, CritterCommandControl ccc)
    {
        _tree = tree;
        _ccc = ccc;
        _agent = _ccc.GetComponent<NavMeshAgent>();
    }

    public override bool IsFinished => _agent.remainingDistance <= DELTA_FROM_TARGET * _tree.Radius;

    public override void Execute()
    {
        //Debug.Log(string.Format("Executing Move to : {0}", _destination));
        _agent.SetDestination(_tree.transform.position);
    }

    public override void OnArrive()
    {
        _ccc.OccupyTree(_tree);
    }
}
