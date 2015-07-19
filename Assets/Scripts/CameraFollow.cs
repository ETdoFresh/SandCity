using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform targetFollowTransform;
    public float targetMinDistance = 1;

    Vector3 _relativeCameraPosition;
    Quaternion _relativeCameraRotation;
    Vector3 _target;


    // Use this for initialization
    void Start()
    {
        _target = targetFollowTransform.position;
        _relativeCameraPosition = transform.position - _target;
        _relativeCameraRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetFollowTransform == null)
            return;

        if (Vector3.Distance(_target, targetFollowTransform.position) > targetMinDistance)
        {
            _target = (_target - targetFollowTransform.position).normalized;
            _target *= targetMinDistance;
            _target += targetFollowTransform.position;
            transform.position = _target + _relativeCameraPosition;
        }
    }
}
