using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehaviour : MonoBehaviour
{
    private GameObject boss;
    private List<GameObject> players = new List<GameObject>();
    private List<Health> playerHealth = new List<Health>();

    //THE BOSS' HEALTH
    private Health bossHealth;


    private int totalPlayers = 0;

    private bool bossDead = false;
    [HideInInspector] public bool WaveCompleted = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Checks through all gameobjects with the gameobject tag PLAYER
        GameObject[] foundPlayers = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in foundPlayers)
        {
            GameObject rootPlayer = player.transform.root.gameObject;

            //Makes sure in case a child gameobject has the tag player, that it does not add again
            if (!players.Contains(rootPlayer))
            {
                players.Add(rootPlayer);
                Health health = rootPlayer.GetComponent<Health>();
                playerHealth.Add(health);
                health.OnDeath += DeadPlayerCount;
                totalPlayers++;
            }
        }

        //Checks through all gameobjects with the gameobject tag Boss
        GameObject bossObject = GameObject.FindGameObjectWithTag("Boss");

        if (bossObject != null)
        {
            boss = bossObject.transform.root.gameObject;
        }
        bossHealth = boss.GetComponent<Health>();
        bossHealth.OnDeath += BossDead;
    }

    // Update is called once per frame
    void Update()
    {
        if (WaveCompleted)
        {
            //Do stuff here when all the players are dead;
        }
        else if (bossDead)
        {
            //Do stuff when teh boss is dead
            //TODO: Make it so that it goes to the next boss
        }
    }
    private void OnDestroy()
    {
        bossHealth.OnDeath -= BossDead;

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
        Debug.Log("Boss is dead");
        bossDead = true;
    }
    /// <summary>
    /// RUN THIS FUNCTION WHEN A PLAYER IS DEAD
    /// </summary>
    private void DeadPlayerCount()
    {
        totalPlayers--;
        if(totalPlayers <= 0)
        {
            WaveCompleted = true;
        }
    }
  
}
