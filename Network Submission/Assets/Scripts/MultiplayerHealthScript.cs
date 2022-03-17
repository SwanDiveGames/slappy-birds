using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;

public class MultiplayerHealthScript : NetworkBehaviour
{
    //NEWTORK VARIABLES
    private NetworkVariableInt networkclientInt = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableULong networkclientIDUlong = new NetworkVariableULong(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableBool networkclientIDChangedBool = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    public Dictionary<ulong, int> LOCALplayersHealthDictionary = new Dictionary<ulong, int>();
    public NetworkDictionary<ulong, int> playersHealthDictionary = new NetworkDictionary<ulong, int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    [ServerRpc(RequireOwnership = false)]
    public void syncHealthToAllClientServerRpc(ulong clientId, int clientHealth)
    {
        if (networkclientIDUlong.Value != clientId)
        {
            networkclientIDUlong.Value = clientId;
            networkclientIDChangedBool.Value = true;
        }
        else
        {
            networkclientIDChangedBool.Value = false;
        }

        networkclientInt.Value = clientHealth;
        Debug.Log("net health = " + networkclientInt.ToString());
    }

    private void OnEnable()
    {
        networkclientInt.OnValueChanged += updatetheHealthClientRpc;
    }

    private void OnDisable()
    {
        networkclientInt.OnValueChanged -= updatetheHealthClientRpc;
    }

    [ClientRpc]
    private void updatetheHealthClientRpc(int oldScaleHealth, int newScaleHealth)
    {
        //setup if not available yet
        if (!playersHealthDictionary.ContainsKey(networkclientIDUlong.Value))
        {
            playersHealthDictionary[networkclientIDUlong.Value] = 0;
        }
        if (!LOCALplayersHealthDictionary.ContainsKey(networkclientIDUlong.Value))
        {
            LOCALplayersHealthDictionary[networkclientIDUlong.Value] = 0;
        }

        int lowestHealth = 0;
        if (newScaleHealth > oldScaleHealth)
        {
            lowestHealth = newScaleHealth;
        }
        else
        {
            lowestHealth = oldScaleHealth;
        }

        Debug.Log("Lowest Health Value" + lowestHealth.ToString());

        if (IsOwner)
        {
            //if owner then check if the value that already stored in the dictionary is less than the new value
            if (networkclientIDChangedBool.Value == false)
            {
                LOCALplayersHealthDictionary[networkclientIDUlong.Value] = lowestHealth;
            }
            else
            {
                LOCALplayersHealthDictionary[networkclientIDUlong.Value] = newScaleHealth;
            }

            if (LOCALplayersHealthDictionary[networkclientIDUlong.Value] != playersHealthDictionary[networkclientIDUlong.Value])
            {
                playersHealthDictionary[networkclientIDUlong.Value] = LOCALplayersHealthDictionary[networkclientIDUlong.Value];
            }

        }
    }

    void OnGUI()
    {
        int x = 0;
        foreach (KeyValuePair<ulong, int> entry in playersHealthDictionary)
        {
            GUI.Label(new Rect(500, 60 + (15 * x), 300, 20), "Player " + entry.Key + " HP = " + (2 - entry.Value));
            x++;
        }
    }
}
