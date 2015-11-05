using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Player : MonoBehaviour {

    private float speed = 0.0f;
    private float lateralSpeed = 0.0f;
    public float maxSpeed = 15.0f;
    public float acceleration = 5.0f;
    private float lateralAcceleration = 1.0f;
    public float jumpForce = 5.0f;

    private bool jumping = false;
    private Rigidbody playerRigidbody;
	private GameObject gc;

    private Vector2 touchOrigin = -Vector2.one; // Storing location of screen touch


	// Use this for initialization
	void Start () {
        playerRigidbody = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
      

        // Check if running in web player or standalone
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
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
/*
        try 
        {
            if(GameObject.Find("UpButton").GetComponent<Button>().OnPointerDown)
            {
                Debug.Log("UpButton");
                Accelerate(true);
            } else if (Input.GetButton("DownButton"))
            {
                Accelerate(false);
            } else if (Input.GetButton("LeftButton"))
            {
                LateralMove(-1);
            } else if (Input.GetButton("RightButton"))
            {
                LateralMove(1);
            }
        }
        catch
        {

        }
        */

#elif UNITY_IOS || UNITY_ANDROID || UNITY_WP8 || UNITY_IPHONE

    // Initial touch controls
    // Premise: arrows to control the ship
    // touch anywhere else to jump
    

    /* Touch controls to check differential between two touches to get the direction

        // Check if input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            // store first touch
            Touch firstTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (firstTouch.phase == TouchPhase.Began)
            {
                // set touchOrigin to the position of that touch
                touchOrigin = firstTouch.position;
                

            }

        // if touch phase is not Began and instead is equal to Ended and the x of touchOrigin
            else if (firstTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                // Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;
                
                // difference between the beginning and end of the touch on the x axis
                float x = touchEnd.x - touchOrigin.x;

                // diff between y axis points
                float y = touchEnd.y - touchOrigin.y;

                touchOrigin.x = -1;

                // check whether x or y axis has the greater difference

            }
        }
    */

    
    


#endif // end of mobile platform dependent section

	}

    void FixedUpdate()
    {
        // Adjust velocity
        playerRigidbody.velocity = new Vector3(playerRigidbody.velocity.x, playerRigidbody.velocity.y, speed);

        // Adjust lateral velocity
        playerRigidbody.velocity = new Vector3(lateralSpeed, playerRigidbody.velocity.y, playerRigidbody.velocity.z);
        lateralSpeed = 0.0f;
        // Perform jump
		Debug.Log (Mathf.Sqrt(Mathf.Pow(playerRigidbody.velocity.z,2f) + Mathf.Pow(playerRigidbody.velocity.x,2f) + Mathf.Pow(playerRigidbody.velocity.y,2f)));
    }

    // Movement methods
    public void LateralMove(int dir)
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

    public void Accelerate(bool forward)
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

    public void Jump()
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
		if (collision.gameObject.tag == "FinishLine" && gc != null) {
			gc.GetComponent<GCScript>().win ();
		}
		if (collision.gameObject.tag == "Boomer" && gc != null && (Mathf.Sqrt(Mathf.Pow(playerRigidbody.velocity.z,2f) + Mathf.Pow(playerRigidbody.velocity.x,2f) + Mathf.Pow(playerRigidbody.velocity.y,2f))) > 10) {
			gc.GetComponent<GCScript>().playerCrash();
		}
    }

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Boomer" && gc != null && (Mathf.Sqrt(Mathf.Pow(playerRigidbody.velocity.z,2f) + Mathf.Pow(playerRigidbody.velocity.x,2f) + Mathf.Pow(playerRigidbody.velocity.y,2f))) > 10) {
			gc.GetComponent<GCScript>().playerCrash();
		}
	}

	public void getGC(GameObject go) {
		gc = go;
	}
}
