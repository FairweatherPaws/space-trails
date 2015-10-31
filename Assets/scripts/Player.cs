using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

    private float speed = 0.0f;
    private float lateralSpeed = 0.0f;
    public float maxSpeed = 15.0f;
    public float acceleration = 5.0f;
    private float lateralAcceleration = 1.0f;
    public float jumpForce = 5.0f;

    private bool jumping = false;
    private Rigidbody playerRigidbody; 


	// Use this for initialization
	void Start () {
        playerRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        lateralSpeed = 0.0f;

	    // Keyboard player controls
        // Accelerate
        if (Input.GetKey(KeyCode.W))
        {
            Accelerate(true);
        }
        // Decelerate
        if (Input.GetKey(KeyCode.S))
        {
            Accelerate(false);
        }
        // Lateral controls
        if (Input.GetKey(KeyCode.A))
        {
            LateralMove(-1);
        } 
        if (Input.GetKey(KeyCode.D))
        {
            LateralMove(1);
        } 
        // Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
	}

    void FixedUpdate()
    {
        // Adjust velocity
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y, speed);

        // Adjust lateral velocity
        playerRigidbody.velocity = new Vector3(lateralSpeed, playerRigidbody.velocity.y, playerRigidbody.velocity.z);

        // Perform jump
    }

    // Movement methods
    void LateralMove(int dir)
    {
        if(dir == -1)
        {
            lateralSpeed = -5.0f;
        }
        else if (dir == 1)
        {
            lateralSpeed = 5.0f;
        }
    }

    void Accelerate(bool forward)
    {
        if (forward)
        {
            if (speed < maxSpeed)
            {
                speed += Time.deltaTime * acceleration;
            }
        } 
        else if (!forward)
        {
            if (speed > 0)
            {
                speed -= Time.deltaTime * acceleration;
            }
            else
            {
                speed = 0;
            }
        }
    }

    void Jump()
    {
        if(!jumping)
        {
            jumping = true;
            playerRigidbody.velocity = new Vector3(0, jumpForce, playerRigidbody.velocity.z);
        }
    }
    
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            jumping = false;
        }
    }

}
