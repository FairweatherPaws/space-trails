using UnityEngine;
using System.Collections;
using System.IO;

public class GCScript : MonoBehaviour {

	// FUTURE ME: check what the hell is wrong with the post-last level spawning, kthx
	// PlayerControl spawn

	public static float xShift = 1.618f, yShift = 0.4f, zShift = 2.618f;
	public GameObject player, slab, playerPrefab, slabParent, bewm, tunnel;
	public Material plainWhite, plainLightGrey;
	public Camera mainCamera;
	public Light ambience, solar;
	private int playerStartLocation = 0;
	private bool playerDied, won;
	private int currentLevel, maxLevel, levelCount;
	private Vector2 touchStart, touchEnd;
	private string levelName, path;

    int playerDir = 0;
    
    // Init debug / basic touch buttons
    public bool debugControls = true;

	// Use this for initialization
	void Start () {

		playerDied = false;
		won = false;

		if (PlayerPrefs.HasKey("currentLevel")) {
			currentLevel = PlayerPrefs.GetInt ("currentLevel"); // saves the current level to long-term cache, could also implement e.g. upgrades or max level reached with this
		} else {
			currentLevel = 1;
		}
        
		levelCount = FileReader.getRowCount(Application.persistentDataPath + "/StreamingAssets/levels/index.idx");

        // Enable debug controls
        if (debugControls)
        {
            try
            {
                foreach (Transform child in GameObject.Find("PlayerControls").transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            catch
            { }

        }
		
		path = "";
		levelName = "";
		
		levelName = FileReader.getLine(Application.persistentDataPath + "/StreamingAssets/levels/index.idx", currentLevel); // this gets the next level from the index; add appropriate address for other platforms

		path = Application.persistentDataPath + "/StreamingAssets/levels/" + levelName + ".lvl";

		if (Application.platform == RuntimePlatform.Android) {

			levelName = FileReader.getLine(Application.streamingAssetsPath + "/levels/" + "index.idx", currentLevel);
			path = Application.streamingAssetsPath + "/levels/" + levelName + ".lvl";

		}

		string[,] grid = FileReader.readFile(path);

		int i = 0;

		StartCoroutine(CreateDelay(i, grid));

		// mainCamera.transform.position = new Vector3(grid.GetLength (1)/2 * 1.618f, mainCamera.transform.position.y, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {

		if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
		

	
		if (playerDied) {
			solar.color = new Color(solar.color.r - Time.deltaTime/2, solar.color.g - Time.deltaTime/2, solar.color.b - Time.deltaTime/2, 1);
		}


        // Debug/touch controls
        if(playerDir > 0 && playerDir < 5)
        {
            
            if (playerDir == 1)
            {
                player.GetComponent<Player>().Accelerate(true);
            }
            else if (playerDir == 2)
            {
                player.GetComponent<Player>().LateralMove(-1);
            }
            else if (playerDir == 3)
            {
                player.GetComponent<Player>().LateralMove(1);
            }
            else if (playerDir == 4)
            {
                player.GetComponent<Player>().Accelerate(false);
            }
        }

		if (Input.touchCount > 0) { 
			if (Input.GetTouch (0).phase == TouchPhase.Began) {
				touchStart = Input.GetTouch(0).position;
			}
			Vector2 currentLocation = Input.GetTouch(0).position;

			float distanceToDot = Mathf.Sqrt(Mathf.Pow(currentLocation.x-touchStart.x,2)+Mathf.Pow(currentLocation.y-touchStart.y,2));

			if (distanceToDot > Screen.width / 20) {
				if (currentLocation.y > touchStart.y) { // up
					if (Mathf.Abs (currentLocation.x - touchStart.x) < Mathf.Abs (currentLocation.y - touchStart.y)) {
						player.GetComponent<Player>().Accelerate(true);
					} else {
						if (currentLocation.x > touchStart.x) {
							player.GetComponent<Player>().LateralMove(1);
						} else {
							player.GetComponent<Player>().LateralMove(-1);
						}
					}
				} else if (currentLocation.y <= touchStart.y) { // lower
					if (Mathf.Abs (currentLocation.x - touchStart.x) < Mathf.Abs (currentLocation.y - touchStart.y)) {
						player.GetComponent<Player>().Accelerate(false);
					} else {
						if (currentLocation.x > touchStart.x) {
							player.GetComponent<Player>().LateralMove(1);
						} else {
							player.GetComponent<Player>().LateralMove(-1);
						}
					}
				} 
			}
 
			
			if (Input.GetTouch(0).phase == TouchPhase.Ended) {
				touchEnd = Input.GetTouch(0).position;
				if (Mathf.Sqrt (Mathf.Pow (touchEnd.x-touchStart.x,2)+Mathf.Pow (touchEnd.y-touchStart.y,2)) < Screen.width / 10) {
					player.GetComponent<Player>().Jump();
				}
			}
		}

	}

    public void HardReset()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    // button controls for movement, reference player object
    public void ButtonControls(int i)
    {
        try
        {
            if (i > 0 && i < 5)
            {
                playerDir = i;
            }
            else
            {
                playerDir = -1;
            }

            if (i == 5)
            {
                player.GetComponent<Player>().Jump();
            }
        }
        catch
        {

        }
    }

	IEnumerator CreateDelay(int i, string[,] grid) {
		yield return new WaitForSeconds(0.02f);
		generateRow(i, grid);
	}

	IEnumerator CreatePlayer() {
		yield return new WaitForSeconds(0.02f);
		instantiatePlayer();
	}

	IEnumerator AdvanceLevel() {

		yield return new WaitForSeconds(2f);
		destroyAllTheThings();
		yield return new WaitForSeconds(1f);
		gotoNextLevel();
	}

	void generateRow(int i, string[,] grid) {

		bool playerPresent = false;
		int[] deployable = new int[grid.GetLength (1)];

		for (int j = 0; j < grid.GetLength (1); j++) {
			
			if (grid[i,j].Length > 0) {

				string verticalInfo = grid[i,j];

				for (int k = 0; k < grid[i,j].Length; k++) {

					if (verticalInfo[k].Equals('1') || verticalInfo[k].Equals('F')) {
						GameObject newSlab = Instantiate (slab, new Vector3(j*xShift, k*yShift, i*zShift), Quaternion.identity) as GameObject;
						
						if ((i+j) % 2 == 0) {
							newSlab.gameObject.GetComponent<Renderer>().material = plainWhite;
						} else {
							newSlab.gameObject.GetComponent<Renderer>().material = plainLightGrey;
						}

						newSlab.transform.parent = slabParent.transform;

						newSlab.GetComponent<SlabScript>().makeSpecial(verticalInfo[k]);

						deployable[j]++;

					} else if (i == 0 && verticalInfo[k].Equals('p')) {
						playerStartLocation = j;
						playerPresent = true;
						break;
					} else if (verticalInfo[k].Equals ('T')) {
						GameObject newTunnel = Instantiate(tunnel, new Vector3(j*xShift, k*yShift-0.2f, i*zShift-1), Quaternion.identity) as GameObject;

						newTunnel.gameObject.GetComponent<Renderer>().material = plainWhite;

						newTunnel.transform.parent = slabParent.transform;

						deployable[j] += 2;

						k++;

						continue;
					}
				}
			}
		}

		if (!playerPresent && i == 0) {

			for (int j = 0; j < deployable.GetLength(0); j++) {

				if (deployable[j] > 0) {

					playerStartLocation = j;
					playerPresent = true;
					break;

				}

			}
		}

		if (i < grid.GetLength(0) - 1) {
			i++;
			StartCoroutine(CreateDelay(i, grid));
		}
		if (playerPresent) {
			StartCoroutine(CreatePlayer());
		} else {
			// Handle fucked up level design here
		}
	}

	void instantiatePlayer() {
		player = Instantiate(playerPrefab, new Vector3(playerStartLocation*xShift, 4, 0), Quaternion.identity) as GameObject;

		player.GetComponent<Player>().getGC (this.gameObject);

		mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2.5f, player.transform.position.z - 4f);

		mainCamera.transform.parent = player.transform;

	}

	void destroyAllTheThings() {

		mainCamera.transform.parent = null;

		GameObject[] components = GameObject.FindGameObjectsWithTag("LevelComponent");
		GameObject[] sfx = GameObject.FindGameObjectsWithTag("SFX");

		foreach(GameObject unit in components) {

			DestroyImmediate(unit);

		}

		foreach(GameObject effect in sfx) {

			DestroyImmediate (effect);

		}
		if (player != null) {
			DestroyImmediate (player);
		}

		playerDied = false;
		won = false;
	}

	void gotoNextLevel() {

		currentLevel++;

		if (currentLevel > levelCount) {

			currentLevel = 1;

		}

		if (maxLevel < currentLevel) {
			maxLevel = currentLevel;
		}

		string path = "";
		string levelName = "";
		
		levelName = FileReader.getLine(Application.dataPath + "/StreamingAssets/levels/index.idx", currentLevel); // this gets the next level from the index; add appropriate address for other platforms
		
		path = Application.dataPath + "/StreamingAssets/levels/" + levelName + ".lvl";
		
		if (Application.platform == RuntimePlatform.Android) {
			
			levelName = FileReader.getLine(Application.dataPath + "!/assets/StreamingAssets/levels/index.idx", currentLevel); // this gets the next level from the index; add appropriate address for other platforms
			path = "jar:file://" + Application.persistentDataPath + "!/assets/StreamingAssets/levels/" + levelName + ".lvl";
			
		}

		
		string[,] grid = FileReader.readFile(path);
		
		int i = 0;

		solar.color = new Color(1f, 0.8f, 0.5f, 1);
		
		StartCoroutine(CreateDelay(i, grid));

	}

	public void win() {

		if (!won) {
			won = true;

			Debug.Log("Level won");

			StartCoroutine(AdvanceLevel());
		}
	}

	public void playerCrash() {
		if (!playerDied) {
			Debug.Log("Respawn called");

			playerDied = true;

			GameObject boom = Instantiate(bewm, player.transform.position, Quaternion.identity) as GameObject;
			mainCamera.transform.parent = boom.transform;

			DestroyImmediate (player);

			currentLevel--;
			StartCoroutine(AdvanceLevel()); // hacky as fuck, beware.
		}
	}
}
