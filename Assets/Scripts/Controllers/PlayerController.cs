using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
    // INSPECTOR VARIABLES
    [Tooltip("The speed at which the player moves")]
    public float moveSpeed = 5;

    [Tooltip("The rate the character turns (does not effect direction of movement)")]
    public float turnSpeed = 15;

    [Tooltip("Turns on debugging visuals")]
    public bool debug = false;

    [Tooltip("Clip the character to the floor if he's this far from the floor and not jumping. This helps with going down slopes")]
    public float clipThreshold;

    //PRIVATE VARIABLES
    // The position the player is currently trying to move to
    private Vector3 targetPosition;
    // The rigidbody for the current player
    private Rigidbody myRigidbody;
    // The collider for the current player
    private CapsuleCollider myCollider;
    // The animator attached to the player
    private Animator myAnimator;
    // Whether the character is intentionally off the ground
    private bool isJumping = false;


	// Use this for initialization
	void Start () {
        targetPosition = gameObject.transform.position;
        myRigidbody = gameObject.GetComponent<Rigidbody>();
        myCollider = gameObject.GetComponent<CapsuleCollider>();
        myAnimator = gameObject.GetComponent<Animator>();

        myAnimator.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        HandleInput();
        UpdateMovement();
        HandleDebug();
	}

    // Physics update
    private void FixedUpdate()
    {
        slopeClip();
    }

    void HandleInput()
    {
        // If right click down
        if (Input.GetMouseButton(1))
        {
            // Set the target position to mouse location projected onto game environment
            MoveTargetToMousePosition();
        }

        // If space bar is pressed
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BeginSpinAttack();
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            StopSpinAttack();
        }
    }

    void UpdateMovement()
    {
        // Find the direction we need to move to reach our goal
        // NOTE: despite only moving x and z, we get the direction in 3 dimensions for a damping effect
        // as you approach the point, the vector points downwards, decreasing the x and z components and easing to a halt
        Vector3 moveDirection = (targetPosition - gameObject.transform.position).normalized;
        // Set the x and z directions of velocity, but retain gravitation effects from y
        myRigidbody.velocity = new Vector3(moveDirection.x * moveSpeed, myRigidbody.velocity.y, moveDirection.z * moveSpeed);

        // Rotate towards the point. Make sure the point is at the same height so we are only rotating around the y
        Vector3 lookPoint = targetPosition;
        lookPoint.y = gameObject.transform.position.y;
        Quaternion lookAtPoint = Quaternion.LookRotation(lookPoint - gameObject.transform.position, Vector3.up);
        Quaternion newRotation = Quaternion.Slerp(gameObject.transform.rotation, lookAtPoint, turnSpeed * Time.deltaTime);
        gameObject.transform.rotation = newRotation;
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

    // If the character is not jumping snap him to the ground within a threshold (for going down ramps)
    void slopeClip()
    {
        // Only clip if we're not intentionally jumping
        if(!isJumping)
        {
            // Get the bottom position of our collider from which to cast the ray
            Vector3 bottom = gameObject.transform.position - new Vector3(0, (myCollider.height / 2), 0);
            // Raycast to check if the player is within the threshold to the nearest ground
            RaycastHit hit;
            Ray clippingRay = new Ray(bottom, Vector3.down);
            if (Physics.Raycast(clippingRay, out hit, clipThreshold))
            {
                // Only consider navigable terrain
                if (hit.collider.tag == "Terrain")
                {
                    gameObject.transform.position -= new Vector3(0, hit.distance, 0);
                }
            }
        }
    }

    void BeginSpinAttack() {
        myAnimator.enabled = true;
        myAnimator.SetBool("Attacking", true);
    }
    void StopSpinAttack() {
        myAnimator.SetBool("Attacking", false);
        myAnimator.enabled = false;
    }
}
