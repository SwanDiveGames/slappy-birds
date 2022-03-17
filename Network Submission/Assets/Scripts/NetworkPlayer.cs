using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class NetworkPlayer : NetworkBehaviour
{
    public Vector3 componentTransform;
    public ulong localClientID;

    //create a networkvariable
    private NetworkVariableVector3 networkclientScale = new NetworkVariableVector3();


    // Start is called before the first frame update
    void Start()
    {
        // get local client id
        localClientID = NetworkManager.Singleton.LocalClientId;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NetworkStart()
    {
        // get local client id
        localClientID = NetworkManager.Singleton.LocalClientId;
    }

    public void callTheRPCToFlip()
    {
        // try to get the local client object..return when unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientID, out var networkedClient))
        {
            return;
        }
        // get the component we want to check
        if (!networkedClient.PlayerObject.TryGetComponent<Transform>(out var transformNetworkedClient))
        {
            return;
        }
        componentTransform = transformNetworkedClient.localScale;
        // send a message to the server via RPC
        AdjustClientScaleWhenFlippingServerRpc(componentTransform);
    }

    [ServerRpc] 
    public void AdjustClientScaleWhenFlippingServerRpc(Vector3 localScaleVector)
    {
        networkclientScale.Value = localScaleVector;
    }

    private void OnEnable()
    {
        networkclientScale.OnValueChanged += OnClientScaleChange;
    }

    private void OnDisable()
    {
        networkclientScale.OnValueChanged -= OnClientScaleChange;
    }

    private void OnClientScaleChange(Vector3 oldScaleVector3, Vector3 newScaleVector3)
    {
        if (newScaleVector3 == new Vector3(0.0f, 0.0f, 0.0f))
        {
            newScaleVector3 = new Vector3(1.0f, 1.0f, 1.0f);
        }
        transform.localScale = newScaleVector3;
    }

}
