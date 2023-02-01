using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterManager : MonoBehaviour
{

    private CritterCommandControl curSelected;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ListenForClick();
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

    private void HandleNewSelection(CritterCommandControl ccc)
    {
        if(curSelected != null)
            curSelected.IsSelected = false;

        ccc.IsSelected = true;
        curSelected = ccc;
    }

    private void HandleCommandClick(RaycastHit hitInfo)
    {
        if(curSelected == null)
            return;

        // Left Shift maintains a command queue
        if(!Input.GetKey(KeyCode.LeftShift))
        {
            curSelected.ClearCommands();
        }

        curSelected.QueueCommand(hitInfo.point);
    }
}
