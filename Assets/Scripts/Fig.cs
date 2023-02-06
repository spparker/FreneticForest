using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fig : MonoBehaviour
{

    public const float DIRECTION_TIME = 0.5f;
    public const float LIFE_TIME = 30f;
    Rigidbody rb;
    float _moveTime;
    float _totalTime;
    Vector3 _curDir;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _moveTime = DIRECTION_TIME;
    }

    void Update()
    {
        _totalTime += Time.deltaTime;
        if(_totalTime > LIFE_TIME)
            Destroy(gameObject);

        _moveTime += Time.deltaTime;
        if(_moveTime >= DIRECTION_TIME)
            ChangeDirection();
    }

    void FixedUpdate()
    {
        rb.AddForce(_curDir * 3);
    }

    private void ChangeDirection()
    {
        _moveTime = 0;
        _curDir = new Vector3(Random.Range(-1.0f,1.0f), 0, Random.Range(-1.0f,1.0f));
    }


    void OnCollisionEnter(Collision collision)
    {
        var pod = collision.collider.transform.GetComponent<CritterPod>();
        if(pod && pod.NeedHealth)
            HealPod(pod);
    }

    public void HealPod(CritterPod pod)
    {
        pod.HealMe();
        Destroy(gameObject);
    }
}
