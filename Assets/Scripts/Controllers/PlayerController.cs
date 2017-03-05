using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    [Tooltip("The speed at which the player moves")]
    public float moveSpeed = 5;
    [Tooltip("The rate the character turns (does not effect direction of movement)")]
    public float turnSpeed = 15;
    [Tooltip("Turns on debugging visuals")]
    public bool debug = false;

    // The position the player is currently trying to move to
    private Vector3 targetPosition;
    private Rigidbody myRigidbody;


	// Use this for initialization
	void Start () {
        targetPosition = gameObject.transform.position;
        myRigidbody = gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
        UpdateMovement();
        HandleDebug();
	}

    void HandleInput()
    {
        // If right click down
        if (Input.GetMouseButton(1))
        {
            // Set the target position to mouse location projected onto game environment
            MoveTargetToMousePosition();
        }
    }

    void UpdateMovement()
    {
        // Find the direction we need to move to reach our goal
        Vector3 moveDirection = (targetPosition - gameObject.transform.position).normalized;
        // Set the x and z directions of velocity, but retain gravitation effects from y
        myRigidbody.velocity = new Vector3(moveDirection.x * moveSpeed, myRigidbody.velocity.y, moveDirection.z * moveSpeed);
    }

    void MoveTargetToMousePosition()
    {
        // Create a ray from camera to mouse position
        Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        // Check for intersection with anything tagged Terrain and if so, set that as the target
        RaycastHit hit;
        if (Physics.Raycast(mouseRay, out hit))
        {
            if (hit.collider.tag == "Terrain")
            {
                targetPosition = hit.point;
            }
        }
    }

    void HandleDebug()
    {
        if (debug)
        {
            Debug.DrawLine(transform.position, targetPosition, Color.red);
        }
    }
}
