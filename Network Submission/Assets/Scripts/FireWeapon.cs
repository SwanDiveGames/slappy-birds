using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class FireWeapon : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform bulletPoint;

    public NetworkVariableVector3 bulletPointNet;
    public NetworkVariableQuaternion bulletRotNet;

    public NetworkVariableFloat bulletPointX;
    public NetworkVariableInt bulletPointY;
    public NetworkVariableInt bulletPointZ;

    int counter = 0;

    // Update is called once per frame
    void Update()
    {
        
    }

    void fireWeapon()
    {
        //Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
        ////Debug.Log("BANG");

        fireWeaponServerRpc();
    }

    private void FixedUpdate()
    {
        if (NetworkManager.IsHost)
        {
            if (counter <= 90)
                counter++;
            else
            {
                counter = 0;
                fireWeapon();

            }

        }
    }

    [ServerRpc]
    public void fireWeaponServerRpc()
    {
        fireWeaponClientRpc();
    }

    [ClientRpc]
    private void fireWeaponClientRpc()
    {
        Instantiate(bulletPrefab, bulletPoint.position, bulletPoint.rotation);
    }
}
