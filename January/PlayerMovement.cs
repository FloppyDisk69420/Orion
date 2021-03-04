// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    //Assingables
    public Transform playerCam;
    public Transform orientation;
    
    //Other
    Rigidbody rb;
	InputManager input;
	public EntityStats stats;

    //Rotation and look
    float sensitivity = 100f;
    float sensMultiplier = 1f;
    
    //Movement
    public bool grounded;
    public LayerMask whatIsGround;
	float moveSpeed;
	float maxSpeed;
	    
    public float counterMovement = 0.175f;
    float threshold = 0.01f;

    //Crouch & Slide
    Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    Vector3 playerScale;
    public float slideCounterMovement = 0.2f;

    //Jumping
    bool readyToJump = true;
    
    //Input
	Vector2 move;
	public Vector3 speed;
    public bool jumping, sprinting, crouching;
    
    //Sliding
    Vector3 normalVector = Vector3.up;
    Vector3 wallNormalVector;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
		input = InputController.Instance.input;
		stats = GetComponent<EntityStats>();
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		
		//Crouching
        input.Player.Crouch.performed += _ => StartCrouch();
        input.Player.Crouch.canceled += _ => StopCrouch();
    }

    void FixedUpdate() {
        Movement();
    }

    void Update() {
        MyInput();
		Look();
    }
    void MyInput() {
        move = input.Player.Movement.ReadValue<Vector2>();
        jumping = input.Player.Jump.ReadValue<float>() > 0;
        crouching = input.Player.Crouch.ReadValue<float>() > 0;
        sprinting = input.Player.Sprint.ReadValue<float>() > 0;

		// Set acceleration
		if (sprinting) moveSpeed = stats.sprintSpeed * stats.agility;
		else moveSpeed = stats.walkSpeed + stats.agility;
    }

    void StartCrouch() {
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f) {
            if (grounded) {
                rb.AddForce(orientation.transform.forward * stats.slideSpeed);
            }
        }
    }

    void StopCrouch() {
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    void Movement() {
		//Extra gravity
        rb.AddForce(Vector3.down * Time.deltaTime * 10);
        
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y;

        //Counteract sliding and sloppy movement
        CounterMovement(move.x, move.y, mag);
        
        //If holding jump && ready to jump, then jump
        if (readyToJump && jumping) Jump();

        //Set max speed
		if (sprinting) maxSpeed = stats.maxSprintSpeed * stats.agility;
		else maxSpeed = stats.maxWalkSpeed * stats.agility;
        
        //If sliding down a ramp, add force down so player stays grounded and also builds speed
        if (crouching && grounded && readyToJump) {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }
        
        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (move.x > 0 && xMag > maxSpeed) move.x = 0;
        if (move.x < 0 && xMag < -maxSpeed) move.x = 0;
        if (move.y > 0 && yMag > maxSpeed) move.y = 0;
        if (move.y < 0 && yMag < -maxSpeed) move.y = 0;

        //Some multipliers
        float multiplier = 1f, multiplierV = 1f;
        
        // Movement in air
        if (!grounded) {
            multiplier = 0.5f;
            multiplierV = 0.5f;
        }
        
        // Movement while sliding
        if (grounded && crouching) multiplierV = 0f;

        //Apply forces to move player
		Vector3 speedY = orientation.forward * move.y * moveSpeed * Time.deltaTime * multiplier * multiplierV;
		Vector3 speedX = orientation.right * move.x * moveSpeed * Time.deltaTime * multiplier;
		speed = speedX + speedY;
        rb.AddForce(speedY);
        rb.AddForce(speedX);
    }

    void Jump() {
        if (grounded && readyToJump) {
            readyToJump = false;

            //Add jump forces
            rb.AddForce(Vector2.up * stats.jumpForce * 1.5f);
            rb.AddForce(normalVector * stats.jumpForce * 0.5f);
            
            //If jumping while falling, reset y velocity.
            Vector3 vel = rb.velocity;
            if (rb.velocity.y < 0.5f)
                rb.velocity = new Vector3(vel.x, 0, vel.z);
            else if (rb.velocity.y > 0) 
                rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);
            
            Invoke(nameof(ResetJump), stats.jumpCooldown);
        }
    }
    
    void ResetJump() {
        readyToJump = true;
    }
    
	float xRot;
    void Look() {
		Vector2 mouse = input.Player.Look.ReadValue<Vector2>() * sensitivity * Time.fixedDeltaTime * sensMultiplier;

		xRot -= mouse.y;
		
		playerCam.localRotation = Quaternion.Euler(xRot, 0f, 0f);

		orientation.Rotate(Vector3.up * mouse.x);
    }

    void CounterMovement(float x, float y, Vector2 mag) {
        if (!grounded || jumping) return;

        //Slow down sliding
        if (crouching) {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }

        //Counter movement
        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0)) {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }
        
        //Limit diagonal running. This will also cause a full stop if sliding fast and un-crouching, so not optimal.
        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed) {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }

    bool IsFloor(Vector3 v) {
        float angle = Vector3.Angle(Vector3.up, v);
        return angle < stats.maxClimbAngle;
    }

    bool cancellingGrounded;
    
    void OnCollisionStay(Collision other) {
        //Make sure we are only checking for walkable layers
        int layer = other.gameObject.layer;
        if (whatIsGround != (whatIsGround | (1 << layer))) return;

        //Iterate through every collision in a physics update
        for (int i = 0; i < other.contactCount; i++) {
            Vector3 normal = other.contacts[i].normal;
            //FLOOR
            if (IsFloor(normal)) {
                grounded = true;
                cancellingGrounded = false;
                normalVector = normal;
                CancelInvoke(nameof(StopGrounded));
            }
        }

        //Invoke ground/wall cancel, since we can't check normals with CollisionExit
        float delay = 3f;
        if (!cancellingGrounded) {
            cancellingGrounded = true;
            Invoke(nameof(StopGrounded), Time.deltaTime * delay);
        }
    }

    void StopGrounded() {
        grounded = false;
    }
    
}
