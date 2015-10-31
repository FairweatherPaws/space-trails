using UnityEngine;
using System.Collections;

public class SlabScript : MonoBehaviour {

	private bool initialBounce = true;
	private float bounceTime;
	private float myX, myY, myZ;
	private bool winningTile;

	// Use this for initialization
	void Start () {
		myX = this.transform.position.x;
		myY = this.transform.position.y;
		myZ = this.transform.position.z;
		bounceTime = 2f;
		winningTile = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (initialBounce) {
			if (bounceTime > 1.4f) {
				this.transform.Translate(new Vector3(0,Time.deltaTime*2,0));
			} else if (bounceTime > 0.4f) { 
				this.transform.Translate(new Vector3(0,-Time.deltaTime*2,0));
			} else if (bounceTime > 0) {
				this.transform.Translate(new Vector3(0,Time.deltaTime*2,0));
			}
			bounceTime -= Time.deltaTime*2;
			if (bounceTime < 0) {
				this.transform.position = new Vector3(myX,myY,myZ);
				initialBounce = false;
			}
		}
	}

	public void makeSpecial(char s) {
		if (s.Equals('F')) {
			this.gameObject.tag = "Finish";
		}
	}

}
