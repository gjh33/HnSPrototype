using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    [Tooltip("Angle the camera is facing relative to parent.transform.forward")]
    [Range(-360f, 360f)]
    public float facingAngle;

    [Tooltip("Distance from the character to the camera")]
    public float distanceToCamera;

    [Tooltip("Angle the camera is looking down on the player (bigger = closer to birds eye)")]
    [Range(0f, 90f)]
    public float viewingAngle;

    [Tooltip("The target for the camera to follow")]
    public GameObject target;
	
	// Update is called once per frame
	void Update () {
        // First set the position and rotation
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.transform.Rotate(viewingAngle, facingAngle, 0);
        Vector3 offset = new Vector3(0, 0, distanceToCamera);
        gameObject.transform.position = target.transform.position - (transform.rotation * offset);
    }
}
