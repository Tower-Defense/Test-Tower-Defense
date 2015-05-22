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
    public bool gameOver = false;
    public bool buildPanelOpen = false;

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
    public Color onColor;
    public Color offColor;
    /// <summary>
    /// All structures that we can build.
    /// </summary>
    public Turret_Base[] allStructures;
    public GameObject[] buildButtons;
    private UISprite[] buildBtnGraphics;
    private UILabel[] costTexts;
    private int structureIndex = 0;
    //


    //--- GUI Items

    //---Game Info
    public UILabel waveText;
    public UILabel livesText;
    public UILabel scoreText;
    public UILabel cashText;
    //---

    //---TweenPositions  
    public TweenPosition buildPanelTweener;
    public TweenRotation buildPanelArrowTweener;
    public TweenPosition settingsPanelTweener;
    public TweenPosition upgradePanelTweener;
    public TweenPosition gameOverPanelTweener;
    //---

    //---Placement Plane items
    public GameObject placementPlanesRoot;
    public Material hoverMat;
    public LayerMask placementLayerMask;
    private Material originalMat;
    private GameObject lastHitObj;
    //---

    //---Upgrade
    private PlacementPlane focusedPlane;
    private Turret_Base structureToUpgrade;
    private Turret_Base upgradeStructure;
    public UILabel upgradeText;
    public GameObject upgradeBtn;
    public UILabel sellBtn;
    //---

    //--- Speed buttons
    public GameObject BtnPlay;
    public GameObject BtnPause;
    public UILabel BtnX2;
    //---

    //--- Game Over
    public UILabel GameOverText;
    //---

    // ThreadSafe singletone
    private LevelMaster() { }

    public static LevelMaster Instance { get; private set; }

    public void Awake()
    {

        Instance = this;
        // Get difficulty multiplier
        difficultyMultiplier *= PlayerPrefs.GetFloat("DifficultyMultiplier", 1.0f);
    }

    // Use this for initialization
    void Start()
    {
        // --- Build Panel
        var buildButtonsCount = buildButtons.Length;
        buildBtnGraphics = new UISprite[buildButtonsCount];
        costTexts = new UILabel[buildButtonsCount];
        for (int j = 0; j < buildButtonsCount; j++)
        {
            buildBtnGraphics[j] = buildButtons[j].GetComponentInChildren<UISprite>();
            costTexts[j] = buildButtons[j].GetComponentInChildren<UILabel>();
            //upgrade costs
            costTexts[j].text = "$" + allStructures[j].myCost.ToString();
        }
        //----

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
        if (livesCount <= 0)
        {
            GameOverPanel();
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettingsPanel();
        }
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
               && !upgradePanelOpen && !GameController.IsPause)
            {
                focusedPlane = lastHitObj.GetComponent<PlacementPlane>();
                int turretCost = allStructures[structureIndex].myCost;
                if (focusedPlane.isOpen && turretCost <= cashCount)
                {

                    GameObject newStructure = Instantiate(allStructures[structureIndex].gameObject,
                                                          lastHitObj.transform.position,
                                                          Quaternion.identity) as GameObject;

                    newStructure.transform.localEulerAngles = new Vector3(
                        newStructure.transform.localEulerAngles.x,
                        Random.Range(0, 360),
                        newStructure.transform.localEulerAngles.z);

                    focusedPlane.myStructure = newStructure;
                    focusedPlane.isOpen = false;

                    UpgradeCash(-turretCost);

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


    #region Moving Panels

    #region Upgrade Panel
    // Upgrade turret
    private void ShowUpgradeGUI()
    {
        //get the plane's structure, and that structure's upgrade options
        structureToUpgrade = focusedPlane.myStructure.GetComponent<Turret_Base>();
        int sellCost = (int)(structureToUpgrade.myCost * structureToUpgrade.mySellMultiplier);
        upgradeStructure = structureToUpgrade.myUpgrade;

        if (upgradeStructure != null)
        {
            var upgradeCost = upgradeStructure.myCost;
            string upgradeName = upgradeStructure.myName;
            sellBtn.GetComponentInChildren<UILabel>().text = "Delete +$" + sellCost.ToString();
            upgradeText.text = "Upgrade to " + upgradeName + " for $" + upgradeCost + "?";
            CostCheckButton(upgradeBtn, upgradeCost);

        }
        else
        {

            sellBtn.GetComponentInChildren<UILabel>().text = "Delete +$" + sellCost.ToString();
            upgradeText.text = "This is last version of turret";
            SetCollider(upgradeBtn, false);
            upgradeBtn.GetComponentInChildren<UILabel>().color = Color.grey;
        }
        SetUpgradePanelState(true);
    }

    public void ConfirmUpgrade()
    {
        var spawnPos = structureToUpgrade.gameObject.transform.position;
        var spawnRot = structureToUpgrade.gameObject.transform.rotation;
        Destroy(structureToUpgrade.gameObject);
        GameObject newStructure = Instantiate(upgradeStructure.gameObject, spawnPos, spawnRot) as GameObject;
        focusedPlane.myStructure = newStructure;

        UpgradeCash(-upgradeStructure.myCost);
        SetUpgradePanelState();
    }

    public void SellTurret()
    {
        int sellCost = (int)(structureToUpgrade.myCost * structureToUpgrade.mySellMultiplier);
        focusedPlane.isOpen = true;
        focusedPlane.myStructure = null;
        Destroy(structureToUpgrade.gameObject);
        // cost * sellMultiplier
        UpgradeCash(sellCost);
        SetUpgradePanelState();
    }

    public void CloseUpgradePanel()
    {
        SetUpgradePanelState();
    }

    public void SetUpgradePanelState(bool open = false)
    {
        upgradePanelTweener.Play(open);
        upgradePanelOpen = open;
    }

    #endregion


    #region Settings Panel
    // Upgrade Structures
    public void ToggleSettingsPanel()
    {
        if (!settingsPanelOpen)
        {

            settingsPanelOpen = true;
            settingsPanelTweener.Play(true);
            SetPause();
            Debug.Log("Open settings panel");
        }
        else if (settingsPanelOpen && GameController.IsPause)
        {
            settingsPanelOpen = false;
            settingsPanelTweener.Play(false);
            SetDefaultSpeedOfGame();
            Debug.Log("Close settings panel");
        }
    }
    #endregion

    #region GameOver
    public void GameOverPanel()
    {
        if (!settingsPanelOpen && !gameOver)
        {
            gameOver = true;
            gameOverPanelTweener.Play(true);

            // Counter of Score
            int scoreCounter = PlayerPrefs.GetInt("ScoreCounter", -1);
            scoreCounter++;
            PlayerPrefs.SetInt("ScoreCounter", scoreCounter);

            // Save Date of Game
            var time = System.DateTime.Now;

            PlayerPrefs.SetString("ScoreDate" + scoreCounter.ToString(),
                System.DateTime.SpecifyKind(time, System.DateTimeKind.Local).ToString());
            // Save score count
            PlayerPrefs.SetInt("Score" + scoreCounter.ToString(), scoreCount);

            // if it is a high score
            var highScore = PlayerPrefs.GetInt("HighScore", 0);
            if (scoreCount > highScore)
            {
                PlayerPrefs.SetInt("HighScore", scoreCount);
                GameOverText.color = Color.green;
                GameOverText.text = "New record!\n Score: " + scoreCount.ToString();
            }
            else
            {
                GameOverText.color = new Color32(8, 214, 255, 255); // light blue(standart)
                GameOverText.text = "Game over!\n You score:\n" + scoreCount.ToString();

            }

            SetPause();

            Debug.Log("Game over");
        }

    }

    #endregion

    #region Build Panel
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

    public void SetBuildChoice(GameObject btnObj)
    {
        string btnName = btnObj.name;
        for (int i = 0; i < buildButtons.Length; i++)
        {
            if (btnObj == buildButtons[i])
            {
                structureIndex = i;
                break;
            }
        }

        UpdateGUI();
    }
    #endregion

    public void GoToMainMenu()
    {
        GameController.GoToMenu();
    }

    public void RestartGame()
    {
        GameController.ReplayGame();
    }

    #region setTimeScale



    public void SetPause()
    {
        GameController.PauseGame();
        Debug.Log("Pause");
        BtnPause.SetActive(false);
        BtnPlay.SetActive(true);
        foreach (var btn in buildButtons)
        {
            SetCollider(btn, false);
        }

    }


    public void SetDefaultSpeedOfGame()
    {
        if (!settingsPanelOpen && !gameOver)
        {
            Debug.Log("x1");
            GameController.ResumeGame();
            BtnPause.SetActive(true);
            BtnPlay.SetActive(false);
            foreach (var btn in buildButtons)
            {
                SetCollider(btn, true);
            }
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
        if (!GameController.IsPause)
        {
            if (BtnX2.text == "X2")
            {
                BtnPause.SetActive(true);
                BtnPlay.SetActive(false);
                Debug.Log("x2");
                BtnX2.text = "X1";
                Time.timeScale = 2;
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



    #endregion


    private void SetNextWave()
    {
        waveLevel++;
        //Up difficulty exponentially
        difficultyMultiplier *= ((Mathf.Pow(waveLevel, 2)) * 0.005f) + 1;
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
            if (allStructures[i].myCost > cashCount)
            {
                costTexts[i].color = Color.red;
                buildBtnGraphics[i].color = new Color(0.5f, 0.5f, 0.5f, 0.5f);

                SetCollider(buildBtnGraphics[i].transform.parent
                    .gameObject, false);
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

                SetCollider(buildBtnGraphics[i].transform.parent
                    .gameObject, true);
            }
        }
    }

    private void UpgradeCash(int cash = 0)
    {
        cashCount += cash;
        UpdateGUI();
    }





    //---GUI methods   
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







    private void CostCheckButton(GameObject theBtn, int itemCost)
    {
        if (cashCount < itemCost)
        {
            theBtn.GetComponentInChildren<UILabel>()
                    .color = Color.red;
            SetCollider(theBtn, false);
        }
        else
        {
            theBtn.GetComponentInChildren<UILabel>()
                    .color = Color.green;
            SetCollider(theBtn, true);
        }
    }



    //--- End GUI methods

    private void SetCollider(GameObject obj, bool enabled_)
    {
        obj.GetComponent<Collider>().enabled = enabled_;
    }


}
