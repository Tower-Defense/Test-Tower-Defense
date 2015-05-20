using UnityEngine;
using System.Collections;

public class LevelMaster : MonoBehaviour
{

    // From pathfinding
    public AstarPath AstarController;

    //States
    public bool waveActive = false;
    public bool spawnEnemies = false;
    public bool upgradePanelOpen = false;
    public bool settingsPanelOpen = false;
    public bool pause = false;
    public AudioListener cameraListener;

    //Player variables
    public int livesCount = 10;
    public int scoreCount = 0;
    public int cashCount = 200;

    //Define Wave Specific Variables
    public int waveLevel;
    public float difficultyMultiplier = 1.0f;
    public float waveLength = 30.0f;
    public float intermissionTime = 5.0f;
    private float waveEndTime = 0f;

    // Enemy Variables
    public GameObject[] enemyPrefabs;
    public Transform flyerSpawns;
    public Transform groundSpawns;
    private Transform[] flyerSpawnPoints;
    private Transform[] groundSpawnPoints;
    public float respawnMinBase = 3.0f;
    public float respawnMaxBase = 10.0f;
    private float respawnMin = 3.0f; // min create next enemy
    private float respawnMax = 10.0f; // max create next enemy
    public float respawnInterval = 2.5f; // when create next enemy
    public int enemyCount = 0;
    private float lastSpawnTime = 0f;

    // Turrets
    public int[] turretCosts;
    public Color onColor;
    public Color offColor;
    /// <summary>
    /// All structures that we can build.
    /// </summary>
    public GameObject[] allStructures;
    public UISprite[] buildBtnGraphics;
    public UILabel[] costTexts;
    private int structureIndex = 0;
    //


    //--- GUI Items

    //GUI Variables
    public UILabel waveText;
    public UILabel livesText;
    public UILabel scoreText;
    public UILabel cashText;
    public UILabel upgradeText;
    public GameObject upgradeBtn;

    //NGUI items
    public bool buildPanelOpen = false;
    public TweenPosition buildPanelTweener;
    public TweenRotation buildPanelArrowTweener;
    public TweenPosition settingsPanelTweener;
    public TweenPosition upgradePanelTweener;
    //

    //Placement Plane items
    public GameObject placementPlanesRoot;
    public Material hoverMat;
    public LayerMask placementLayerMask;
    private Material originalMat;
    private GameObject lastHitObj;
    //

    //Upgrade vars
    private PlacementPlane focusedPlane;
    private Turret_Base structureToUpgrade;
    private GameObject upgradeStructure;
    private int upgradeCost;
    //


    //--- Speed buttons
    public GameObject BtnPlay;
    public GameObject BtnPause;
    public UILabel BtnX2;
    //---

    // ThreadSafe singletone
    	private LevelMaster(){}
    	private static LevelMaster instance;
    	public static LevelMaster Instance
    	{
    		get 
    		{
    			//He is already created, so we find him
    			return instance ?? 
    				(instance = GameObject.FindWithTag ("LevelMaster").GetComponent <LevelMaster>()); 
    		}
    	}


    // Use this for initialization
    void Start()
    {
        //reset the structure index, refresh the GUI
        structureIndex = 0;
        UpdateGUI();

        //gather all the FLYER spawn into an array, 
        // so we don't have to do it manually
        flyerSpawnPoints = new Transform[flyerSpawns.childCount];
        int i = 0;
        foreach (Transform theSpawnPoint in flyerSpawns)
        {
            flyerSpawnPoints[i] = theSpawnPoint;
            i++;
        }
        //

        //gather all the GROUND spawn into an array, 
        // so we don't have to do it manually
        groundSpawnPoints = new Transform[groundSpawns.childCount];
        i = 0;
        foreach (Transform theSpawnPoint in groundSpawns)
        {
            groundSpawnPoints[i] = theSpawnPoint;
            i++;
        }
        //

        // Start wave
        SetNextWave();
        StartNewWave();

    }

    // Update is called once per frame
    void Update()
    {
        //---GUI
        if (buildPanelOpen)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000, placementLayerMask))
            {
                if (lastHitObj)
                {
                    (lastHitObj.GetComponent<MeshRenderer>()).material = originalMat;
                }

                lastHitObj = hit.collider.gameObject;
                originalMat = (lastHitObj.GetComponent<MeshRenderer>()).material;
                (lastHitObj.GetComponent<MeshRenderer>()).material = hoverMat;
            }
            else
            {
                if (lastHitObj)
                {
                    (lastHitObj.GetComponent<MeshRenderer>()).material = originalMat;
                    lastHitObj = null;
                }
            }

            //drop turrets on click
            if (Input.GetMouseButtonDown(0) && lastHitObj // 0 - left button
               && !upgradePanelOpen && !settingsPanelOpen)
            {
                focusedPlane = lastHitObj.GetComponent<PlacementPlane>();
                if (focusedPlane.isOpen && turretCosts[structureIndex] <= cashCount)
                {

                    GameObject newStructure = Instantiate(allStructures[structureIndex],
                                                          lastHitObj.transform.position,
                                                          Quaternion.identity) as GameObject;

                    newStructure.transform.localEulerAngles = new Vector3(
                        newStructure.transform.localEulerAngles.x,
                        Random.Range(0, 360),
                        newStructure.transform.localEulerAngles.z);

                    focusedPlane.myStructure = newStructure;
                    focusedPlane.isOpen = false;

                    cashCount -= turretCosts[structureIndex];
                    UpdateGUI();

                    //update the Graph
                    // rebuild the Graph
                    AstarController.Scan();

                    foreach (GameObject theEnemy in
                            GameObject.FindGameObjectsWithTag("Ground Enemy"))
                    {
                        theEnemy.GetComponent<Enemy_GroundUnit>().GetNewPath();
                    }

                }
                else if (focusedPlane.myStructure != null)
                {
                    ShowUpgradeGUI();
                }
            }
            //	}
        }
        //---End GUI

        //wave
        if (waveActive)
        {
            if (Time.time >= waveEndTime)
            {
                //stop spawn enemies
                spawnEnemies = false;

                if (enemyCount == 0)
                {
                    // Корутин
                    StartCoroutine(FinishWave());
                }
            }
            if (spawnEnemies)
            {
                // if wave still going, spawn enemies
                if (Time.time > (lastSpawnTime + respawnInterval))
                {
                    SpawnNewEnemy();
                }
            }
        }
    }

    public void ConfirmUpgrade()
    {
        var spawnPos = structureToUpgrade.transform.position;
        var spawnRot = structureToUpgrade.transform.rotation;
        Destroy(structureToUpgrade.gameObject);
        GameObject newStructure = Instantiate(upgradeStructure, spawnPos, spawnRot) as GameObject;
        focusedPlane.myStructure = newStructure;

        cashCount -= upgradeCost;
        UpdateGUI();
        upgradePanelTweener.Play(false);
        upgradePanelOpen = false;
    }

    public void CancelUpgrade()
    {
        upgradePanelTweener.Play(false);
        upgradePanelOpen = false;
    }

    // Upgrade turret
    private void ShowUpgradeGUI()
    {
        //get the plane's structure, and that structure's upgrade options
        structureToUpgrade = focusedPlane.myStructure.GetComponent<Turret_Base>();
        upgradeStructure = structureToUpgrade.myUpgrade;

        if (upgradeStructure != null)
        {

            upgradePanelOpen = true;

            upgradeCost = structureToUpgrade.myUpgradeCost;
            string upgradeName = structureToUpgrade.myUpgradeName;

            upgradeText.text = "Upgrade to" + upgradeName + "for $" + upgradeCost + "?";
            CostCheckButton(upgradeBtn, upgradeCost);
            upgradePanelTweener.Play(true);
        }
    }

    // Upgrade Structures
    public void ToggleSettingsPanel()
    {
        if (!settingsPanelOpen && !pause)
        {
                        
            settingsPanelOpen = true;
            settingsPanelTweener.Play(true);
            SetPause();
            Debug.Log("Open settings panel");
        }
        else if (settingsPanelOpen && pause)
        {                       
            settingsPanelOpen = false;
            settingsPanelTweener.Play(false);
            SetDefaultSpeedOfGame();
            Debug.Log("Close settings panel"); 
        }
    }

    public void GoToMainMenu()
    {
        setTimeScale(1);
        PlayerPrefs.SetInt("LoadLevel", 1);
        Application.LoadLevel(0);
        Debug.Log("Go to Main Menu");
    }

    #region setTimeScale

    public static void setTimeScale(int timeScale)
    {
        Time.timeScale = timeScale;
    }

    public void SetPause()
    {
        Debug.Log("Pause");
        BtnPause.SetActive(false);
        BtnPlay.SetActive(true);
        pause = true;
        setTimeScale(0);
    }
    public void SetDefaultSpeedOfGame()
    {
        if (!settingsPanelOpen)
        {
            Debug.Log("x1");
            pause = false;
            setTimeScale(1);
            BtnPause.SetActive(true);
            BtnPlay.SetActive(false);
            if (BtnX2.text == "X1")
                BtnX2.text = "X2";
        }
        else
        {
            Debug.Log("can't x1");
        }
    }
    public void Set2xSpeedOfGame()
    {
        if (!settingsPanelOpen)
        {
            if (BtnX2.text == "X2")
            {
                BtnPause.SetActive(true);
                BtnPlay.SetActive(false);
                Debug.Log("x2");
                BtnX2.text = "X1";
                setTimeScale(2);
            }
            else
            {
                SetDefaultSpeedOfGame();
            }
        }
        else
        {
            Debug.Log("can't x2");
        }
        

    }
    #endregion


    private void SetNextWave()
    {
        waveLevel++;
        //Up difficulty exponentially
        difficultyMultiplier = ((Mathf.Pow(waveLevel, 2)) * 0.005f) + 1;
        respawnMin = respawnMinBase * (1 / difficultyMultiplier);
        respawnMax = respawnMaxBase * (1 / difficultyMultiplier);
    }

    private void StartNewWave()
    {
        //Set the GUI
        UpdateGUI();

        //spawn the first enemy
        SpawnNewEnemy();

        //set the wave end time
        waveEndTime = Time.time + waveLength;

        //activate the wave
        waveActive = true;
        spawnEnemies = true;
    }


    private IEnumerator FinishWave()
    {
        waveActive = false;

        //wait for it. We use corrutine
        yield return new WaitForSeconds(intermissionTime);
        //	Debug.Log ("Yes! I'm here!");
        SetNextWave();
        StartNewWave();

    }


    private void SpawnNewEnemy()
    {
        //get random index to choose an enemy prefab with
        int enemyChoice = Random.Range(0, enemyPrefabs.Length);

        int spawnChoice;

        switch (enemyPrefabs[enemyChoice].tag)
        {
            case "Air Enemy":
                spawnChoice = Random.Range(0, flyerSpawnPoints.Length);
                Instantiate(enemyPrefabs[enemyChoice],
                             flyerSpawnPoints[spawnChoice].position,
                             flyerSpawnPoints[spawnChoice].rotation);
                break;
            case "Ground Enemy":
                spawnChoice = Random.Range(0, groundSpawnPoints.Length);
                Instantiate(enemyPrefabs[enemyChoice],
                             groundSpawnPoints[spawnChoice].position,
                             groundSpawnPoints[spawnChoice].rotation);
                break;

            default:
                Debug.Log("Wtf! SpawnNewEnemy choose: " + enemyPrefabs[enemyChoice].tag +
                           "o_O");
                break;
        }

        enemyCount++;

        lastSpawnTime = Time.time;

        respawnInterval = Random.Range(respawnMin, respawnMax);
    }

    /// <summary>
    /// Check to make sure we can purchase
    /// </summary>
    private void CheckTurretCosts()
    {
        for (int i = 0; i < allStructures.Length; i++)
        {
            if (turretCosts[i] > cashCount)
            {
                costTexts[i].color = Color.red;
                buildBtnGraphics[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
                buildBtnGraphics[i].transform.parent
                        .gameObject
                        .GetComponent<Collider>()
                        .enabled = false;
            }
            else
            {
                costTexts[i].color = Color.green;

                if (structureIndex == i)
                {
                    buildBtnGraphics[i].color = onColor;
                }
                else
                {
                    buildBtnGraphics[i].color = offColor;
                }
                buildBtnGraphics[i].transform.parent
                    .gameObject
                        .GetComponent<Collider>()
                        .enabled = true;
            }
        }
    }

    //---GUI methods
    public void SetBuildChoice(GameObject btnObj)
    {
        string btnName = btnObj.name;

        switch (btnName)
        {
            case "Btn_Cannon":
                structureIndex = 0;
                break;
            case "Btn_Missile":
                structureIndex = 1;
                break;
        }

        UpdateGUI();
    }


    public void UpdateGUI()
    {
        foreach (var theBtnGraphic in buildBtnGraphics)
        {
            theBtnGraphic.color = offColor;
        }

        buildBtnGraphics[structureIndex].color = onColor;

        waveText.text = "Wave: " + waveLevel;
        scoreText.text = "Score: " + scoreCount;
        livesText.text = "Lives: " + livesCount;
        cashText.text = "Cash: " + cashCount + "$";

        CheckTurretCosts();
    }


    public void ToogleBuildPanel()
    {
        if (!upgradePanelOpen && !settingsPanelOpen)
        {
            if (buildPanelOpen)
            {
                placementPlanesRoot.SetActive(false);

                buildPanelTweener.Play(false);
                buildPanelArrowTweener.Play(false);
                buildPanelOpen = false;
            }
            else
            {
                placementPlanesRoot.SetActive(true);
                buildPanelTweener.Play(true);
                buildPanelArrowTweener.Play(true);
                buildPanelOpen = true;
            }
        }
    }




    private void CostCheckButton(GameObject theBtn, int itemCost)
    {
        if (cashCount < itemCost)
        {
            theBtn.transform.Find("Label").gameObject
                    .GetComponent<UILabel>()
                    .color = Color.red;
            theBtn.GetComponent<Collider>().enabled = false;
        }
        else
        {
            theBtn.transform.Find("Label").gameObject
                .GetComponent<UILabel>()
                    .color = Color.green;
            theBtn.GetComponent<Collider>().enabled = true;
        }
    }



    //--- End GUI methods
}
