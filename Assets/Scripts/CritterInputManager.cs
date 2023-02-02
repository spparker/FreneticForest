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
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo))
            {
                var gameCollider = hitInfo.collider;
                if(gameCollider == null)
                    return;
                
                var ccc = gameCollider.GetComponent<CritterCommandControl>();
                if(ccc == null)
                    return;

                HandleNewSelection(ccc);
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if(Physics.Raycast(ray, out var hitInfo))
            {
                HandleCommandClick(hitInfo);
            }
        }
    }

    public void HandleNewSelection(CritterCommandControl ccc)
    {
        curSelected?.SetSelected(false);
        ccc.SetSelected(true);
        curSelected = ccc;
    }

    private void HandleCommandClick(RaycastHit hitInfo)
    {
        if(curSelected == null)
            return;

        if(Input.GetKey(KeyCode.LeftControl))
            curSelected.SetToPatrol(hitInfo.point);
        else
        {
            // Left Shift maintains a command queue
            if(!Input.GetKey(KeyCode.LeftShift))
            {
                curSelected.ClearCommands();
            }

            curSelected.QueueMoveCommand(hitInfo.point);
        }
    }
}
