using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine.UI;

public class TimerScript : NetworkBehaviour
{
    ////Timer Variables
    //int maxTime = 60;
    //int currentTime;
    
    //// Start is called before the first frame update
    //void Start()
    //{
    //    GameObject.Find("TimerBG").SetActive(false);
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}

    //public void countDown()
    //{
    //    //-1 time
    //    currentTime--;

    //    //Send new value to server
    //    timerUpdateServerRpc(currentTime);

    //    //End game when time runs out
    //    if (currentTime <= 0)
    //    {
    //        //End game screen
    //    }
    //}

    //[ServerRpc]
    //public void timerUpdateServerRpc(int currentTime)
    //{
    //    timerUpdateClientRpc(currentTime);
    //}

    //[ClientRpc]
    //private void timerUpdateClientRpc(int currentTime)
    //{
    //    GetComponentInParent<Text>().text = currentTime.ToString();
    //}
}
