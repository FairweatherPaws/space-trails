using UnityEngine;
using System.Collections;
using System.IO;

public class GCScript : MonoBehaviour {

	public static float xShift = 1.618f, yShift = 0.4f, zShift = 2.618f;
	public GameObject player, slab, playerPrefab, slabParent, bewm;
	public Material plainWhite, plainLightGrey;
	public Camera mainCamera;
	public Light ambience, solar;
	private int playerStartLocation = 0;
	private bool won = false;

    int playerDir = 0;
    
    // Init debug / basic touch buttons
    public bool debugControls = true;

	// Use this for initialization
	void Start () {
        
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

		string[,] grid = FileReader.readFile("./assets/levels/level01.lvl");
		int i = 0;

		StartCoroutine(CreateDelay(i, grid));

		// mainCamera.transform.position = new Vector3(grid.GetLength (1)/2 * 1.618f, mainCamera.transform.position.y, mainCamera.transform.position.z);
	}
	
	// Update is called once per frame
	void Update () {
	
		if (won) {
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

	void generateRow(int i, string[,] grid) {
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

					} else if (i == 0 && verticalInfo[k].Equals('p')) {
						playerStartLocation = j;
						break;
					}
				}
			}
		}
		if (i < grid.GetLength(0) - 1) {
			i++;
			StartCoroutine(CreateDelay(i, grid));
		}
		if (i == 1) {
			StartCoroutine(CreatePlayer());
		}
	}

	void instantiatePlayer() {
		player = Instantiate(playerPrefab, new Vector3(playerStartLocation*xShift, 4, 0), Quaternion.identity) as GameObject;

		player.GetComponent<Player>().getGC (this.gameObject);

		mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 2.5f, player.transform.position.z - 4f);

		mainCamera.transform.parent = player.transform;

	}

	public void win() {
		if (!won) {
			won = true;

			GameObject boom = Instantiate(bewm, player.transform.position, Quaternion.identity) as GameObject;
			mainCamera.transform.parent = boom.transform;
		}
	}
}
