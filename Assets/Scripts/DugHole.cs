using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DugHole : MonoBehaviour
{
    public GameObject HoleMask;
    public const float DEPTH_PER_PASS = 1f;
    public const float WIDTH_PER_DEPTH = 0.12f;
    public const float MAX_DEPTH = 10;
    private float _depth = 1;

    // Returns true to continue
    public bool Deepen()
    {
        _depth = Mathf.Clamp(_depth + DEPTH_PER_PASS,1, MAX_DEPTH);

        transform.localScale = new Vector3(1 + _depth * WIDTH_PER_DEPTH, _depth, transform.localScale.z);
        transform.Translate(0, -DEPTH_PER_PASS * 0.25f ,0);

        HoleMask.transform.position = new Vector3(HoleMask.transform.position.x, 0, HoleMask.transform.position.z);

        if(_depth >= MAX_DEPTH - 0.001)
            return false;

        return true;
    }
}
