using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterCommandControl : MonoBehaviour
{
    private NavMeshAgent _agent;

    private Queue<CritterCommand> _commands = new Queue<CritterCommand>();
    private CritterCommand _currentCommand;

    public bool IsSelected;

    private void Start() => _agent = GetComponent<NavMeshAgent>();

    void Update()
    {
        ProcessCommandQueue();
    }

    public void ClearCommands()
    {
        _currentCommand = null;
        _commands.Clear();
    }

    public void QueueCommand(Vector3 position)
    {
        _commands.Enqueue(item: new CritterMoveCommand(position, _agent));
    }

    private void ProcessCommandQueue()
    {
        if(_currentCommand != null && _currentCommand.IsFinished == false)
            return;
        
        if(_commands.Count <= 0)
            return;

        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();
    }

}
