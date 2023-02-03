using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DugHole : MonoBehaviour
{
    public GameObject HoleMask;
    public const float DEPTH_PER_PASS = 1;
    public const float WIDTH_PER_DEPTH = 0.2f;
    public const float MAX_DEPTH = 10;
    private float _depth = 1;

    // Returns true to continue
    public bool Deepen()
    {
        _depth = Mathf.Clamp(_depth + DEPTH_PER_PASS,1, MAX_DEPTH);

        transform.localScale = new Vector3(1 + _depth * WIDTH_PER_DEPTH, _depth, transform.localScale.z);

        if(_depth >= MAX_DEPTH)
        {
            _depth = MAX_DEPTH;
            return false;
        }
        else
            transform.Translate(0,-0.25f,0);

        HoleMask.transform.position = new Vector3(HoleMask.transform.position.x, 0, HoleMask.transform.position.z);

        return true;
    }
}
