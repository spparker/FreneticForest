using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterInputManager : MonoBehaviour
{

    private CritterCommandControl curSelected;


    // Update is called once per frame
    void Update()
    {
        ListenForKeys();
        ListenForClick();
    }
    
    private void ListenForKeys()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            HandleNewSelection(CritterManager.Instance.GetNextOfType(CritterManager.CritterType.CHOPCHOP));
            CameraControl.Instance.LookAtPosition(curSelected.transform.position);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            HandleNewSelection(CritterManager.Instance.GetNextOfType(CritterManager.CritterType.DIGGIE));
            CameraControl.Instance.LookAtPosition(curSelected.transform.position);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha3))
        {
            HandleNewSelection(CritterManager.Instance.GetNextOfType(CritterManager.CritterType.PATHER));
            CameraControl.Instance.LookAtPosition(curSelected.transform.position);
        }
    }

    private void ListenForClick()
    {
        if(Input.GetMouseButtonDown(0))
        {
            HandleSelectClick();
        }
        else if(Input.GetMouseButtonDown(1))
        {
            HandleCommandClick();
        }
    }

    private void HandleSelectClick()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hitInfo))
        {
            var gameCollider = hitInfo.collider;
            if(gameCollider == null)
                return;
            
            CritterCommandControl ccc;
            var tree = gameCollider.GetComponentInParent<TreeGrowth>();
            if(tree != null)
                ccc = tree.OccupyingCritters;
            else
                ccc = gameCollider.GetComponent<CritterCommandControl>();

            if(ccc == null || ccc.Pod.CritterData.enemy)
                return;

            HandleNewSelection(ccc);
        }
    }

    public void HandleNewSelection(CritterCommandControl ccc)
    {
        curSelected?.SetSelected(false);
        ccc.SetSelected(true);
        curSelected = ccc;
    }

    private void HandleCommandClick()
    {
        if(curSelected == null)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hitInfo))
        {
            //Debug.Log("Clicked on:" + hitInfo.transform.gameObject.name);
            if(Input.GetKey(KeyCode.LeftControl))
            {
                //curSelected.ClearCommands(); // Clear otherwise Location will be wrong (could change to drag and release)
                curSelected.QueuePatrolCommand(hitInfo.point);
            }
            else
            {
                // Left Shift maintains a command queue
                if(!Input.GetKey(KeyCode.LeftShift))
                {
                    curSelected.ClearCommands();
                }
                var tree = hitInfo.collider.GetComponentInParent<TreeGrowth>();
                if(tree != null)
                    curSelected.QueueEnterCommand(tree);
                else
                    curSelected.QueueMoveCommand(hitInfo.point);
            }
        }
    }
}
