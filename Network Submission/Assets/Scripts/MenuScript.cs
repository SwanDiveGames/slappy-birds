using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UIElements;
using UnityEngine.UI;
using MLAPI.Transports.UNET;

public class MenuScript : MonoBehaviour
{
    public GameObject menuPanel;
    public string ipAddress = "127.0.0.1"; //Set initially to connect to local host if nothing input by user
    UNetTransport transport;

    public void Host()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        
        NetworkManager.Singleton.StartHost(GetRandomSpawn(), Quaternion.identity);

        menuPanel.SetActive(false);
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientID, NetworkManager.ConnectionApprovedDelegate callback)
    {
        //check the incoming data
        bool approve = System.Text.Encoding.ASCII.GetString(connectionData) == "SecurePassword";
        callback(true, null, approve, GetRandomSpawn(), Quaternion.identity);
    }

    public void JoinAsClient()
    {
        //Change Connect Address

        //Get the connection address variable for the NetworkManager
        transport = NetworkManager.Singleton.GetComponent<UNetTransport>();
        //Set the connection address to the newly input ip address
        if (ipAddress == null || ipAddress == "")
            ipAddress = "127.0.0.1";

        transport.ConnectAddress = ipAddress;

        //Connect to Host

        //This code feeds a network password to the Hos, encrypted by ASCII, in order to gain server admittance.
        //This is commonly used to compare version types, and in a larger project this string could be stored locally
        //and imported to check the user's game version and ensure no mismatches between client and host.
        NetworkManager.Singleton.NetworkConfig.ConnectionData = System.Text.Encoding.ASCII.GetBytes("SecurePassword");

        NetworkManager.Singleton.StartClient();

        menuPanel.SetActive(false);
    }

    public void SetIP(Text ipInput)
    {
        //Take ip from player input
        ipAddress = ipInput.text;
    }

    Vector3 GetRandomSpawn()
    {
        int x = Random.Range(2, 6);
        return new Vector3(x, 2, 0);
    }

    //Take user input in the UI entry field and store in ipAddress variable
    public void ipAddressChanged(string newAddress)
    {
        this.ipAddress = newAddress;
    }
}