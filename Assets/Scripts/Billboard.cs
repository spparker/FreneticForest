using UnityEngine;
using UnityEngine.AI;

public class Billboard : MonoBehaviour {
    private Transform _mainCamTransform;
    private NavMeshAgent _agent;
    private SpriteRenderer _sr;
    private void Start() {
        _mainCamTransform = Camera.main.transform;
        _agent = GetComponentInParent<NavMeshAgent>();
        _sr = GetComponent<SpriteRenderer>();
    }
    void LateUpdate() {
        transform.LookAt(transform.position + _mainCamTransform.forward);
        
        if(_agent == null)
            return;
        
        FlipToMoveDirection();
    }

    private void FlipToMoveDirection()
    {
        if(_agent.velocity.magnitude < 0.5f)
            return;

        Vector3 flat_cam = Vector3.ProjectOnPlane(_mainCamTransform.forward, Vector3.up).normalized;
        var angle = Vector3.SignedAngle(flat_cam, _agent.velocity.normalized, Vector3.up);

        if(angle > 0)
            _sr.flipX = false;
        else
            _sr.flipX = true;
    }
}