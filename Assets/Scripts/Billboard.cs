using UnityEngine;
using UnityEngine.AI;

public class Billboard : MonoBehaviour {

    public const float MAX_WALK_ROTATION = 12f;
    public const float TILT_WALK_RATE = 100f;

    public bool LeftFacing;

    private float _curTilt;
    private int _tiltingDir = 1;
    private Transform _mainCamTransform;
    private NavMeshAgent _agent;
    private SpriteRenderer _sr;

    [SerializeField] private Sprite Forward;
    [SerializeField] private Sprite ForwardSideways;
    [SerializeField] private Sprite Sideways;
    [SerializeField] private Sprite Backward;
    [SerializeField] private Sprite BackwardSideways;
    private void Start()
    {
        _mainCamTransform = Camera.main.transform;
        _agent = GetComponentInParent<NavMeshAgent>();
        _sr = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + _mainCamTransform.forward);
        
        if(_agent == null)
            return;

        if(_agent.velocity.magnitude < 0.5f)
        {
            _curTilt = 0;
            return;
        }
        
        if(Forward == null)
            FlipToMoveDirection();
        else
            FlipToMultidirection();

        AnimateWalk();
    }

    private void FlipToMoveDirection()
    {
        Vector3 flat_cam = Vector3.ProjectOnPlane(_mainCamTransform.forward, Vector3.up).normalized;
        var angle = Vector3.SignedAngle(flat_cam, _agent.velocity.normalized, Vector3.up);

        if(LeftFacing)
            angle = -angle;

        if(angle > 0)
            _sr.flipX = false;
        else
            _sr.flipX = true;
    }

    private void FlipToMultidirection()
    {
        Vector3 flat_cam = Vector3.ProjectOnPlane(_mainCamTransform.forward, Vector3.up).normalized;
        var angle = Vector3.SignedAngle(flat_cam, _agent.velocity.normalized, Vector3.up);
        //Debug.Log("Angle: " + angle);
        if(LeftFacing)
            angle = -angle;

        if(angle > 0)
            _sr.flipX = false;
        else
            _sr.flipX = true;

        var absAngle = Mathf.Abs(angle);

        if(absAngle < 23)
            _sr.sprite = Forward;
        else if(absAngle < 68)
            _sr.sprite = ForwardSideways;
        else if(absAngle < 113)
            _sr.sprite = Sideways;
        else if(absAngle < 158)
            _sr.sprite = BackwardSideways;
        else
            _sr.sprite = Backward;
    }

    private void AnimateWalk()
    {
        _curTilt += _tiltingDir * Time.deltaTime * TILT_WALK_RATE;

        // Z rotation
        transform.Rotate(transform.forward, _curTilt);
        if(Mathf.Abs(_curTilt) >= MAX_WALK_ROTATION)
            _tiltingDir = -1 * _tiltingDir;
    }
}