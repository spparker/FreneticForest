using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterCommandControl : MonoBehaviour
{
    private NavMeshAgent _agent;
    private CritterPod _pod;

    private Queue<CritterCommand> _commands = new Queue<CritterCommand>();
    private CritterCommand _currentCommand;

    private bool _patrolMode;
    private Vector3 _patrolEndPosition;
    private Vector3 _patrolStartPosition;
    public const float PATROL_TARGET_DELTA = 0.5f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _pod = GetComponent<CritterPod>();
    }

    void Update()
    {
        ProcessCommandQueue();
    }

    public void ClearCommands()
    {
        _currentCommand = null;
        _patrolMode = false;
        _commands.Clear();
    }

    public void SetSelected(bool isSelected)
    {
        _pod.SetSelectedShader(isSelected);
    }

    public void QueueMoveCommand(Vector3 position)
    {
        _commands.Enqueue(item: new CritterMoveCommand(position, _agent));
    }

    // Loop move commands between two positions
    public void SetToPatrol(Vector3 end_position)
    {
        ClearCommands();
        _patrolMode = true;
        _patrolStartPosition = _agent.transform.position;
        _patrolEndPosition = end_position;
        SetNextPatrolDestination();
    }

    private void SetNextPatrolDestination()
    {
        if(Vector3.Magnitude(_agent.transform.position - _patrolStartPosition) <= PATROL_TARGET_DELTA)
            QueueMoveCommand(_patrolEndPosition);
        else
            QueueMoveCommand(_patrolStartPosition);
    }

    private void ProcessCommandQueue()
    {
        if(_currentCommand != null && _currentCommand.IsFinished == false)
            return;

        if(_patrolMode)
            SetNextPatrolDestination();
        
        if(_commands.Count <= 0)
            return;

        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();
    }

}
