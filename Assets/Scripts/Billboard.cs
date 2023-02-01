using UnityEngine;

public class Billboard : MonoBehaviour {
    private Transform _mainCamTransform;
    private void Start() {
        _mainCamTransform = Camera.main.transform;
    }
    void LateUpdate() {
        transform.LookAt(transform.position + _mainCamTransform.forward);
    }
}