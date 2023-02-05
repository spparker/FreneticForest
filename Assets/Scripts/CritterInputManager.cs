using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CritterInputManager : MonoBehaviour
{

    private CritterCommandControl curSelected;

    private int _curAudio;
    [SerializeField] private AudioSource _critterAudio0;
    [SerializeField] private AudioSource _critterAudio1;
    [SerializeField] private AudioSource _critterAudio2;

    public void ClearSelected(CritterPod pod)
    {
        if(!curSelected)
            return;

        if(curSelected == pod.GetComponent<CritterCommandControl>())
            curSelected = null;
    }


    // Update is called once per frame
    void Update()
    {
        ListenForKeys();
        ListenForClick();

        if(curSelected)
            ForestManager.Instance.UpdateSelectedShader(curSelected.transform.position.x, curSelected.transform.position.z, curSelected.Pod.Radius);
        else
            ForestManager.Instance.HideSelectedShader();
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
            {
                ccc = tree.BurrowedCritters ? tree.BurrowedCritters : tree.OccupyingCritters;
                // Second click to select occupied
                if(curSelected == ccc && tree.OccupyingCritters)
                    ccc = tree.OccupyingCritters;
                else if(curSelected == ccc && tree.BurrowedCritters)
                    ccc = tree.BurrowedCritters;
            }
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
        PlayCritterAudio(ccc.Pod.CritterData.Sounds.SoundSelect);
    }

    private void HandleCommandClick()
    {
        if(curSelected == null)
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out var hitInfo))
        {
            PlayCritterAudio(curSelected.Pod.CritterData.Sounds.SoundCommand);
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
                            //Debug.Log("Clicked on:" + hitInfo.transform.gameObject.name);
                var cp = hitInfo.transform.GetComponent<CritterPod>();
                if( cp !=null && cp.CritterData.enemy)
                    curSelected.QueueAttackCommand(cp);
                else if(tree != null)
                    curSelected.QueueEnterCommand(tree);
                else
                    curSelected.QueueMoveCommand(hitInfo.point);
            }
            SendCommandToShader(hitInfo.point);
        }
    }

    private void SendCommandToShader(Vector3 pos)
    {
        ForestManager.Instance.NewCommandForShader(pos.x, pos.z);
    }

    public void PlayCritterAudio(AudioClip clip)
    {
        if(_curAudio == 0)
        {
            PlayAudioOnSource(_critterAudio0, clip);
            _curAudio++;
        }
        else if(_curAudio == 1)
        {
            PlayAudioOnSource(_critterAudio1, clip);
            _curAudio++;
        }
        else
        {
            PlayAudioOnSource(_critterAudio2, clip);
            _curAudio=0;
        }
    }

    private void PlayAudioOnSource(AudioSource src, AudioClip clip)
    {
        src.Stop();
        src.clip = clip;
        src.Play();
    }
}
