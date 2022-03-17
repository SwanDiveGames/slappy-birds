using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using UnityEngine.Networking;
using MLAPI.SceneManagement;


public class MultiplayerScoreScript : NetworkBehaviour
{
    //NEWTORK VARIABLES
    #region Network Variables
    private NetworkVariableInt networkclientInt = new NetworkVariableInt(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableULong networkclientIDUlong = new NetworkVariableULong(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    private NetworkVariableBool networkclientIDChangedBool = new NetworkVariableBool(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });

    public Dictionary<ulong, int> LOCALplayersScoresDictionary = new Dictionary<ulong, int>();
    public NetworkDictionary<ulong, int> playersScoresDictionary = new NetworkDictionary<ulong, int>(new NetworkVariableSettings { WritePermission = NetworkVariablePermission.Everyone });
    #endregion


    //START AND UPDATES
    private void Start()
    {

    }

    private void Update()
    {
        //End game when a score is reached
    }

    void sendDataToServerScript()
    {
        StartCoroutine(toDatabase());
    }

    IEnumerator toDatabase()
    {
        int xcount = 0;
        foreach (KeyValuePair<ulong, int> entry in playersScoresDictionary)
        {
            WWWForm form = new WWWForm();
            form.AddField("clientid", (int)entry.Key);
            form.AddField("score", entry.Value);
            UnityWebRequest www = UnityWebRequest.Post("http://localhost/unityMultiplayerLeaderboard/leaderboard.php", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                xcount++;

                Debug.Log(www.downloadHandler.text);
            }
            www.Dispose();
        }

        if (xcount == playersScoresDictionary.Count)
        {
            changeSceneServerRpc();
        }

    }

    [ServerRpc(RequireOwnership = false)]
    public void changeSceneServerRpc()
    {
        NetworkSceneManager.SwitchScene("EndScreen");
    }


    [ServerRpc(RequireOwnership = false)]
    public void syncScoreToAllClientServerRpc(ulong clientId, int clientScore)
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
        networkclientInt.Value = clientScore;

    }
    private void OnEnable()
    {
        networkclientInt.OnValueChanged += updatetheScoreClientRpc;
    }

    private void OnDisable()
    {
        networkclientInt.OnValueChanged -= updatetheScoreClientRpc;
    }

    [ClientRpc]
    private void updatetheScoreClientRpc(int oldScaleScore, int newScaleScore)
    {
        //setup if not available yet
        if (!playersScoresDictionary.ContainsKey(networkclientIDUlong.Value))
        {
            playersScoresDictionary[networkclientIDUlong.Value] = 0;
        }
        if (!LOCALplayersScoresDictionary.ContainsKey(networkclientIDUlong.Value))
        {
            LOCALplayersScoresDictionary[networkclientIDUlong.Value] = 0;
        }

        int highestscore = 0;
        if (newScaleScore > oldScaleScore)
        {
            highestscore = newScaleScore;
        }
        else
        {
            highestscore = oldScaleScore;
        }

        if (IsOwner)
        {
            //if owner then check if the value that already stored in the dictionary is less than the new value
            if (networkclientIDChangedBool.Value == false)
            {
                LOCALplayersScoresDictionary[networkclientIDUlong.Value] = highestscore;
            }
            else
            {
                LOCALplayersScoresDictionary[networkclientIDUlong.Value] = newScaleScore;
            }

            if (LOCALplayersScoresDictionary[networkclientIDUlong.Value] != playersScoresDictionary[networkclientIDUlong.Value])
            {
                playersScoresDictionary[networkclientIDUlong.Value] = LOCALplayersScoresDictionary[networkclientIDUlong.Value];
            }

        }
    }

    void OnGUI()
    {
        int x = 0;
        foreach (KeyValuePair<ulong, int> entry in playersScoresDictionary)
        {
            GUI.Label(new Rect(10, 60 + (15 * x), 300, 20), "Player " + entry.Key + " Score = " + entry.Value);
            x++;
        }
    }



}
