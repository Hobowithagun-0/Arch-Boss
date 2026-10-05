using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameManagerBehaviour : MonoBehaviour
{
    //Singleton shit
    public static GameManagerBehaviour Instance;

    private GameObject boss;
    private List<GameObject> players = new List<GameObject>();
    private List<Health> playerHealth = new List<Health>();

    //THE BOSS' HEALTH
    private Health bossHealth;


    private int totalPlayers = 0;

    private bool bossDead = false;
    private float countDown = 3f;

    private float counter = 0f;
    public bool WaveCompleted = false;

    public int WaveNumber = 0;

    public List<GameObject>PlayerTypes = new List<GameObject>();
    public GameObject SpawnPoint;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetupScene();
    }
    private void SetupScene()
    {
        // Clear old references
        players.Clear();
        playerHealth.Clear();

        totalPlayers = 0;
        bossDead = false;
        WaveCompleted = false;
        counter = 0f;
        GameObject spawnObject = GameObject.FindGameObjectWithTag("PlayerSpawn");

        if (spawnObject != null)
        {
            SpawnPoint = spawnObject;
        }


        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in foundPlayers)
        {
            GameObject rootPlayer = player.transform.root.gameObject;

            if (!players.Contains(rootPlayer))
            {
                Health health = rootPlayer.GetComponent<Health>();

                if (health != null)
                {
                    players.Add(rootPlayer);
                    playerHealth.Add(health);

                    health.OnDeath += DeadPlayerCount;

                    totalPlayers++;
                }
            }
        }
        // Find players
        SpawnPlayers();

        // Find boss
        GameObject bossObject =
            GameObject.FindGameObjectWithTag("Boss");

        if (bossObject != null)
        {
            boss = bossObject.transform.root.gameObject;
            bossHealth = boss.GetComponent<Health>();

            if (bossHealth != null)
            {
                bossHealth.OnDeath += BossDead;
            }
        }
    }
    private void SpawnPlayers()
    {
        for (int i = 0; i < WaveNumber; i++)
        {
            // Pick a random player type
            int randomIndex = Random.Range(0, PlayerTypes.Count);
            GameObject playerPrefab = PlayerTypes[randomIndex];

            // Spawn it
            GameObject newPlayer = Instantiate(
                playerPrefab,
                SpawnPoint.transform.position,
                SpawnPoint.transform.rotation
            );

            // Add it to our lists
            GameObject rootPlayer = newPlayer.transform.root.gameObject;

            if (!players.Contains(rootPlayer))
            {
                Health health = rootPlayer.GetComponent<Health>();

                if (health != null)
                {
                    players.Add(rootPlayer);
                    playerHealth.Add(health);

                    health.OnDeath += DeadPlayerCount;

                    totalPlayers++;
                }
            }
        }
    }
    void Start()
    {
        SetupScene();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (WaveCompleted)
        {

            WaveNumber++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else if (bossDead)
        {
            //Do stuff when teh boss is dead
            //TODO: Make it so that it goes to the next boss
            counter += Time.deltaTime;
            if (counter >= countDown)
            {
                bossDead = false;
                counter = 0f;
                WaveNumber = 0;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
     
            }
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (bossHealth != null)
        {
            bossHealth.OnDeath -= BossDead;
        }

        foreach (Health health in playerHealth)
        {
            if (health != null)
            {
                health.OnDeath -= DeadPlayerCount;
            }
        }
    }
    /// <summary>
    /// RUN THIS FUNCTION WHEN THE BOSS IS DEAD
    /// </summary>
    private void BossDead()
    {
        bossDead = true;
        Destroy(boss);
    }
    /// <summary>
    /// RUN THIS FUNCTION WHEN A PLAYER IS DEAD
    /// </summary>
    private void DeadPlayerCount()
    {
        Debug.Log("PlayerDEAD");
        totalPlayers--;
        if(totalPlayers <= 0)
        {
            WaveCompleted = true;
        }
    }
  
}
