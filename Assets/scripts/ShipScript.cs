using UnityEngine;
using System.Collections;

public class ShipScript : MonoBehaviour {

	private Rigidbody myBody;
	private float cooldown;

	// Use this for initialization
	void Start () {

		myBody = GetComponent<Rigidbody>();
		cooldown = 0;
	}
	
	// Update is called once per frame
	void Update () {
		cooldown -= Time.deltaTime;

		if (Input.GetAxis ("Vertical") != 0) {
			myBody.AddForce(new Vector3(0,0,20f*Time.deltaTime*Input.GetAxis("Vertical")), ForceMode.Impulse);
		}
		if (Input.GetAxis ("Horizontal") != 0) {
			myBody.AddForce(new Vector3(20f*Time.deltaTime*Input.GetAxis("Horizontal"), 0, 0), ForceMode.Impulse);
		}
		if (Input.GetAxis ("Jump") != 0 && cooldown <= 0) {
			myBody.AddForce(new Vector3(0, 5f, 0), ForceMode.Impulse);
			cooldown = 1f;
		}
	}
}
