using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvaderAI : MonoBehaviour
{
    private CritterCommandControl _ccc;
    private CritterCommandControl _target;

    // Start is called before the first frame update
    void Start()
    {
        _ccc = GetComponent<CritterCommandControl>();
        _target = CritterManager.Instance.GetNearestOfRandom(transform.position);
        if(_target)
            _ccc.QueueAttackCommand(_target.Pod);
    }

    // Update is called once per frame
    void Update()
    {
        if(_target == null)
        {
            _target = CritterManager.Instance.GetNearestOfRandom(transform.position);
            if(_target)
                _ccc.QueueAttackCommand(_target.Pod);
        }
    }
}
