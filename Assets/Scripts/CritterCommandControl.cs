using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterCommandControl : MonoBehaviour
{
    private NavMeshAgent _agent; // TODO: add this at runtime after NavMeshBake?

    private Queue<CritterCommand> _commands = new Queue<CritterCommand>();
    private CritterCommand _currentCommand;


    private void Start() => _agent = GetComponent<NavMeshAgent>();

    void Update()
    {
        ListenForCommands();
        ProcessCommandQueue();
    }

    private void ListenForCommands()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo))
            {
                // Left Shift maintains a command queue
                if(!Input.GetKey(KeyCode.LeftShift))
                {
                    _currentCommand = null;
                    _commands.Clear();
                }

                _commands.Enqueue(item: new CritterMoveCommand(hitInfo.point, _agent));
            }
        }
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
