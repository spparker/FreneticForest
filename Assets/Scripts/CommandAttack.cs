using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

internal class CommandAttack : CritterCommand
{
    public const float DELTA_FROM_TARGET = 0.5f;
    private readonly CritterPod _target;
    private readonly CritterCommandControl _ccc;
    private readonly NavMeshAgent _agent;
    private readonly float _reach_delta;

    public CritterPod GetTarget => _target;

    public CommandAttack(CritterPod target, CritterCommandControl ccc)
    {
        _target = target;
        _ccc = ccc;
        _agent = _ccc.GetComponent<NavMeshAgent>();
        _reach_delta = DELTA_FROM_TARGET + _ccc.Pod.ColliderRadius + _target.ColliderRadius;
        if(_target.InTree)
            _reach_delta += _target.InTree.Radius;
    }

    public override bool IsFinished {
        get {
            if(_target == null)
                return true;
            var dist_to_target = Vector3.Magnitude(_ccc.transform.position - _target.transform.position);
            if(dist_to_target <= _reach_delta)
                return true;
            _agent.SetDestination(_target.transform.position);
                return false;}
    }

    public override void Execute()
    {
        _agent.SetDestination(_target.transform.position);
    }

    public override void OnArrive()
    {
        if(_target != null)
            _ccc.Pod.EnterCombatWith(_target);
    }
}
