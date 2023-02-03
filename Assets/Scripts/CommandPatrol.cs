using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal class CommandPatrol : CritterCommand
{
    public const float DELTA_FROM_TARGET = 0.5f;
    private Vector3? _pointStart;
    private readonly Vector3 _pointEnd;
    private readonly CritterCommandControl _ccc;
    private readonly NavMeshAgent _agent;


    public CommandPatrol( Vector3 finish, CritterCommandControl ccc)
    {
        _pointStart = null; // Initial command needs to find position on execute
        _pointEnd = finish;
        _ccc = ccc;
        _agent = _ccc.GetComponent<NavMeshAgent>();
    }

    public CommandPatrol(Vector3 start, Vector3 finish, CritterCommandControl ccc)
    {
        _pointStart = start;
        _pointEnd = finish;
        _ccc = ccc;
        _agent = _ccc.GetComponent<NavMeshAgent>();
    }

    public override bool IsFinished => _agent.remainingDistance <= DELTA_FROM_TARGET;

    public override void Execute()
    {
        if(_pointStart == null)
            _pointStart = _ccc.transform.position;
        //Debug.Log(string.Format("Executing Move to : {0}", _destination));
        _agent.SetDestination(_pointEnd);
    }

    public override void OnArrive()
    {
        _ccc.QueuePatrolCommandLoop(_pointEnd, _pointStart.Value);
    }
}
