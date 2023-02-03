using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CritterCommandControl : MonoBehaviour
{
    private NavMeshAgent _agent;
    public CritterPod Pod{get; private set;}

    private Queue<CritterCommand> _commands = new Queue<CritterCommand>();
    private CritterCommand _currentCommand;

    private bool _inPatrol;


    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        Pod = GetComponent<CritterPod>();
    }

    void Update()
    {
        ProcessCommandQueue();
    }

    public void ClearCommands()
    {
        _currentCommand = null;
        _commands.Clear();
    }

    public void SetSelected(bool isSelected)
    {
        Pod.SetSelectedShader(isSelected);
    }

    public void QueueMoveCommand(Vector3 position)
    {
        _commands.Enqueue(item: new CommandMove(position, _agent));
    }

    public void QueueEnterCommand(TreeGrowth tree)
    {
        if(Pod.CritterData.canEnterTrees && Pod.InTree != tree)
            _commands.Enqueue(item: new CommandEnter(tree, this));
        else
            QueueMoveCommand(tree.transform.position);
    }

    public void QueuePatrolCommand(Vector3 end_position)
    {
        // Setup a Patrol stuff?
        _commands.Enqueue(item: new CommandPatrol(end_position, this));
    }

    public void QueuePatrolCommandLoop(Vector3 start_positon, Vector3 end_position)
    {
        // Finished a Patrol Leg - increment the thing
        _commands.Enqueue(item: new CommandPatrol(start_positon, end_position, this));
    }

    // Occupy tree if empty
    public void OccupyTree(TreeGrowth tree)
    {
        if(!tree.CanEnter)
            return;

        tree.EnterTree(this);
        Pod.SetInTree(tree);
    }

    private void ProcessCommandQueue()
    {
        if(_currentCommand != null) 
        {
            if(_currentCommand.IsFinished == false)
                return;
            else
                _currentCommand.OnArrive();
        }

        _currentCommand = null;
        
        if(_commands.Count <= 0)
            return;

        if(Pod.InTree)
            Pod.SetOnGround();

        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();
    }

}
