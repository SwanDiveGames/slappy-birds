using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.Logging;
using UnityEngine.SceneManagement;
using System.IO;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class FoxActions : NetworkBehaviour
{
    //Variables

    //Patrol points
    public GameObject foxPatrolPointsObj;
    public List<Transform> patrolPoints;

    int currentTarget = 0;

    //Fox Move Speed
    public float speed = 2f;

    //NETWORK VARIABLES
    //create a networkvariable to store scale in
    private NetworkVariableVector3 networkclientScale = new NetworkVariableVector3();
    public Vector3 componentTransform;
    public ulong localClientID;

    void Start()
    {
        // get local client id
        localClientID = NetworkManager.Singleton.LocalClientId;
    }

    // Update is called once per frame
    private void Update()
    {

    }

    void FixedUpdate()
    {
        MoveToPoint();
    }

    //Move
    void MoveToPoint()
    {
        //Get the transform of the target point
        Transform targetPoint = patrolPoints[currentTarget];

        //Make fox look in the correct direction
        if (targetPoint.position.x < transform.position.x)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = -1;
            transform.localScale = theScale;
        }

        else if (targetPoint.position.x >= GetComponentInParent<Transform>().position.x)
        {
            Vector3 theScale = transform.localScale;
            theScale.x = 1;
            transform.localScale = theScale;
        }

        //Send direction to networked clients
        CallToFlipFox(transform.localScale);

        //Move the fox towards the next point
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime); //speed * Time.deltaTime ensures speed is constant over time

        //Check if fox has reached target (or close enough) and change direction
        if (Vector2.Distance(transform.position, targetPoint.position) < 0.8f)
        {
            //Iterate current target
            currentTarget++;

            //reset current target if out of bounds
            if (currentTarget > patrolPoints.Count - 1)
                currentTarget = 0;
        }

        
    }

    //NETWORKING THE SCALE

    //Calling the Rpc to flip
    void CallToFlipFox(Vector3 scale)
    {
        // try to get the local client object..return when unsuccessful
        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(localClientID, out var networkedFox))
        {
            return;
        }
        // get the component we want to check
        if (!networkedFox.PlayerObject.TryGetComponent<Transform>(out var transformNetworkedFox))
        {
            return;
        }

        componentTransform = transformNetworkedFox.localScale;
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
