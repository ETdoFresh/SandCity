using UnityEngine;
using System.Collections;

public class CameraMovement : MonoBehaviour
{
    public Vector3 lowerLimits = new Vector3(-250, 5, -250);
    public Vector3 upperLimits = new Vector3(250, 40, 250);
    
    Vector3 _fire2Down;
    Vector3 _startingPosition;

    // Use this for initialization
    void Start()
    {
        _startingPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position = transform.position;
        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            position.x += Input.GetAxis("Horizontal");
            position.z += Input.GetAxis("Vertical");
        }
        else if (Input.GetButton("Fire2"))
        {
            if (Input.GetButtonDown("Fire2"))
                _fire2Down = Input.mousePosition;
            Vector3 deltaPosition = Input.mousePosition - _fire2Down;
            deltaPosition *= Time.deltaTime;
            position.x += deltaPosition.x;
            position.z += deltaPosition.y;
        }
        position.y += -Input.GetAxis("Mouse ScrollWheel") * 20;
        position.x = Mathf.Clamp(position.x, lowerLimits.x, upperLimits.x);
        position.y = Mathf.Clamp(position.y, lowerLimits.y, upperLimits.y);
        position.z = Mathf.Clamp(position.z, lowerLimits.z, upperLimits.z);
        transform.position = position;
    }

    public void ResetPosition()
    {
        transform.position = _startingPosition;
    }
}