using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class eggScript : NetworkBehaviour
{
    //Spawn Points for eggs
    public Transform lastSpawn;
    public List<Transform> allSpawns;
    public int spawnPoint = 0;

    //Box Collider Disabler
    public NetworkObject eggNet;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    //UPDATED SPAWNING
    public void selectSpawn()
    {
        //increment spawn counter
        spawnPoint++;
        //Ensure counter not outside range
        if (spawnPoint >= allSpawns.Count)
            spawnPoint = 0;

        Debug.Log("Egg Spawn Point Updated to " + spawnPoint.ToString());

        //Send spawnPoint to all clients and update locally
        eggLocationChangeServerRpc(spawnPoint);
        
        transform.position = allSpawns[spawnPoint].position;
    }

    [ServerRpc]
    public void eggLocationChangeServerRpc(int spawnNo)
    {
        eggLocationChangeClientRpc(spawnNo);
    }

    [ClientRpc]
    public void eggLocationChangeClientRpc(int spawnNo)
    {
        spawnPoint = spawnNo;
    }
}
