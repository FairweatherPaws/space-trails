using UnityEngine;
using System.Collections;


public class GCScript : MonoBehaviour {

	public static float xShift = 1.618f, yShift = 0.4f, zShift = 2.618f;
	public GameObject slab;
	public Material plainWhite, plainLightGrey;
	public Camera mainCamera;

	// Use this for initialization
	void Start () {

		int[,] grid = FileReader.readFile("./assets/levels/level01.lvl");
		int i = 0;
		StartCoroutine(CreateDelay(i, grid));

		mainCamera.transform.position = new Vector3(grid.GetLength (1)/2 * 1.618f, mainCamera.transform.position.y, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator CreateDelay(int i, int[,] grid) {
		yield return new WaitForSeconds(0.02f);
		generateRow(i, grid);
	}

	void generateRow(int i, int[,] grid) {
		for (int j = 0; j < grid.GetLength (1); j++) {
			
			if (grid[i,j] != 0) {
				
				GameObject newSlab = Instantiate (slab, new Vector3(j*xShift, 0, i*zShift), Quaternion.identity) as GameObject;
				
				if ((i+j) % 2 == 0) {
					newSlab.gameObject.GetComponent<Renderer>().material = plainWhite;
				} else {
					newSlab.gameObject.GetComponent<Renderer>().material = plainLightGrey;
				}
			}
		}
		if (i < grid.GetLength(0)) {
			i++;
			StartCoroutine(CreateDelay(i, grid));
		}
	}
}
