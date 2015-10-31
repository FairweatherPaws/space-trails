using UnityEngine;
using System.Collections;
using System.IO;


public class GCScript : MonoBehaviour {

	public static float xShift = 1.618f, yShift = 0.4f, zShift = 2.618f;
	public GameObject player, slab, playerPrefab, slabParent;
	public Material plainWhite, plainLightGrey;
	public Camera mainCamera;
	private int playerStartLocation = 0;

	// Use this for initialization
	void Start () {

		string[,] grid = FileReader.readFile("./assets/levels/level01.lvl");
		int i = 0;
		StartCoroutine(CreateDelay(i, grid));

		mainCamera.transform.position = new Vector3(grid.GetLength (1)/2 * 1.618f, mainCamera.transform.position.y, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	IEnumerator CreateDelay(int i, string[,] grid) {
		yield return new WaitForSeconds(0.02f);
		generateRow(i, grid);
	}

	IEnumerator CreatePlayer() {
		yield return new WaitForSeconds(0.02f);
		instantiatePlayer();
	}

	void generateRow(int i, string[,] grid) {
		for (int j = 0; j < grid.GetLength (1); j++) {
			
			if (grid[i,j].Length > 0) {

				string verticalInfo = grid[i,j];

				for (int k = 0; k < grid[i,j].Length; k++) {

					if (verticalInfo[k].Equals('1')) {
						Debug.Log (verticalInfo[k]);
						GameObject newSlab = Instantiate (slab, new Vector3(j*xShift, k*yShift, i*zShift), Quaternion.identity) as GameObject;
						
						if ((i+j) % 2 == 0) {
							newSlab.gameObject.GetComponent<Renderer>().material = plainWhite;
						} else {
							newSlab.gameObject.GetComponent<Renderer>().material = plainLightGrey;
						}

						newSlab.transform.parent = slabParent.transform;
					} else if (i == 0 && verticalInfo[k].Equals('p')) {
						playerStartLocation = j;
						continue;
					}
				}
			}
		}
		if (i < grid.GetLength(0) - 1) {
			i++;
			StartCoroutine(CreateDelay(i, grid));
		} else {
			StartCoroutine(CreatePlayer());
		}
	}

	void instantiatePlayer() {
		player = Instantiate(playerPrefab, new Vector3(playerStartLocation*xShift, 15, 0), Quaternion.identity) as GameObject;

	}
}
