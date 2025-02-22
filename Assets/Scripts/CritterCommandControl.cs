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

    public void SetSelected(bool isSelected, Color colorOverride)
    {
        Pod.SetSelectColor(colorOverride);
        Pod.SetSelectedShader(isSelected);
    }

    public void SetSelected(bool isSelected)
    {
        Pod.SetSelectedShader(isSelected);
    }

    public void QueueMoveCommand(Vector3 position)
    {
        _commands.Enqueue(item: new CommandMove(position, _agent));
    }

    public void QueueAttackCommand(CritterPod target)
    {
        _commands.Enqueue(item: new CommandAttack(target, this));
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

    public void SendHome()
    {
        ClearCommands();
        QueueMoveCommand(ForestManager.Instance.HomeTree.transform.position);
    }

    // Occupy tree if empty
    public void OccupyTree(TreeGrowth tree)
    {
        if(Pod.CritterData.type == CritterManager.CritterType.CHOPCHOP && !tree.CanEnter)
            return;

        if(Pod.CritterData.type == CritterManager.CritterType.DIGGIE && !tree.CanBurrow)
            return;

        // Check for enemy in tree
        if(tree.Invaded && !Pod.CritterData.enemy)
        { // Autoattack nearest invader
            //Debug.Log("Auto Attacking tree inhabitant");
            var invader = CritterManager.Instance.GetNearestOfType(CritterManager.CritterType.INVADER,transform.position);
            if(invader)
                GetComponent<CritterCommandControl>().QueueAttackCommand(invader.Pod);
        }
        else
        {
            tree.EnterTree(this);
            Pod.SetInTree(tree);
        }
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

        if(Pod.InCombat)
            Pod.EndCombat();

        _currentCommand = _commands.Dequeue();
        _currentCommand.Execute();
    }

}
