using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class CharacterHealthScoreCollisions : NetworkBehaviour
{
    //VARIABLES
    #region Variables
    //Health stats
    public int maxHealth = 0;
    public int currentHealth;

    //Collision Counter to ensure two collisions can't happen at the same time. Happened during testing.
    bool hasCollided = false;

    //Respawn timer
    bool waitForSpawn = false;
    float respawnTime = 50f;

    //Damage Indicators
    public GameObject HeartFull;
    public GameObject HeartDamaged;

    //Checking for dead player
    public bool isDead = false;

    //Scoring
    int score = 0;
    ulong localClientID;
    #endregion

    //START AND UPDATES
    #region Start and Updates
    // Start is called before the first frame update
    void Start()
    {
        //Set current health to max health when player created
        currentHealth = maxHealth;
        localClientID = NetworkManager.Singleton.LocalClientId;
    }


    // Update is called once per frame
    void Update()
    {
        //Lose a life if playe falls off screen
        if (GetComponent<Transform>().position.y <= -10)
        {
            LoseALife();
        }

        if (currentHealth >= 2)
            deadBird();

            hasCollided = false;
    }

    #endregion

    //METHODS

    #region Methods

    //Dead
    public void deadBird()
    {
        //Change animation and health display
        HeartDamaged.GetComponent<SpriteRenderer>().enabled = false;
        HeartFull.GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<Animator>().SetTrigger("Dead");

        //Disable Player Actions
        GetComponent<Animator>().SetBool("Jump", false);
        GetComponent<CharacterController2D>().m_Grounded = true;
        GetComponent<CharacterMovement>().enabled = false;
        GetComponent<CharacterController2D>().enabled = false;
        GetComponent<Attack>().enabled = false;

        //Set position
        GetComponent<Rigidbody2D>().mass = 0;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Transform>().position = new Vector3(1, 1, 1);
        GetComponent<Collider2D>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;

        isDead = true;
    }

    //Lose a Life
    public void LoseALife()
    {
        //Set respawn timer
        waitForSpawn = true;

        //Hide player
        transform.position = new Vector2(10000, 10000);

        //Start timer
        StartCoroutine(WaitForRespawn(respawnTime));

        //End timer
        waitForSpawn = false;

        //Set player to respawn point
        transform.position = new Vector3(2, 2, 1);

        //Subtract health on Network
        //currentHealth -= 1;
        ulong clientID = NetworkManager.Singleton.LocalClientId;
        LoseALifeServerRpc(clientID);
    }


    IEnumerator WaitForRespawn(float respawnTimer)
    {
        yield return new WaitForSeconds(respawnTimer);
    }
    #endregion

    //COLLISION LOGIC
    #region Collision Logic

    //Colliding with Enemies and Bullets
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Chopper")
        {
            if (!hasCollided)
            {
                if (collision.gameObject.tag == "Enemy")
                    collision.gameObject.GetComponent<Animator>().SetTrigger("Attack");

                LoseALife();
                hasCollided = true;
            }

            else
                hasCollided = false;
        }
        
        //if (collision.gameObject.tag == "Platform" && GetComponent<CharacterController2D>().m_Grounded)
        //{
        //    this.transform.parent = collision.transform;
        //}
    }

    //private void OnCollisionExit2D(Collision2D collision)
    //{
    //    if (collision.gameObject.tag == "Platform")
    //    {
    //        this.transform.parent = null;
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!hasCollided)
        {
            if (collision.gameObject.tag == "Bullet")
            {
                
                //Destroys bullet prefab
                Destroy(collision.GetComponent<GameObject>());

                //Lose a Life
                LoseALife();

            }

            else if (collision.gameObject.tag == "Egg")
            {
                //Gain a Point
                collision.GetComponent<eggScript>().selectSpawn();
                eggCollectedServerRpc(OwnerClientId);
            }

            hasCollided = true;
        }

        else
            hasCollided = false;
        
    }

    #endregion

    //NETWORKING FUNCTIONS
    #region Networking
    //Functions under Server RPC run exclusively on the server client -> server
    [ServerRpc]
    public void LoseALifeServerRpc(ulong clientID)
    {
        LoseALifeClientRpc(clientID);
    }

    //Functions under Client RPC run on all clients, sent by the server
    [ClientRpc]
    public void LoseALifeClientRpc(ulong clientID)
    {
        if (clientID == OwnerClientId)
        {
            GetComponent<CharacterHealthScoreCollisions>().currentHealth++;

            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerHealthScript>().syncHealthToAllClientServerRpc(clientID, currentHealth);
        }

        Debug.Log("Client " + clientID + " HP = " + currentHealth + "/" + maxHealth);
    }

    [ServerRpc]
    public void eggCollectedServerRpc(ulong clientID)
    {
        //request the players to send all their scores
        eggCollectedClientRpc(clientID);
    }

    [ClientRpc]
    private void eggCollectedClientRpc(ulong clientID)
    {
        //Get the target client ID, compare it to the owner ID, and if the same update the score
        if (clientID == OwnerClientId)
        {
            //theClientObject = this.gameObject
            GetComponent<CharacterHealthScoreCollisions>().score++;

            GameObject.Find("MultiplayerManager").GetComponent<MultiplayerScoreScript>().syncScoreToAllClientServerRpc(OwnerClientId, score);
        }

        Debug.Log("Player " + clientID + " score = " + score);
    }
    #endregion
}
