using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script is used to send information to the server using the serverclient
public class SendToServerOnClick : MonoBehaviour {

    //Add the serverclient
    private ServerClientScript server;

    //The command that is going to be send to the server, this can be eddited in the unity editor.
    public string SendToServerString = null;

    //add timestaps to disable quick dubble taps that happen sometimes.
    private float lastClickTime = 0;
    private float debounceDelay = 0.005f;

    void OnSelect() {
        //Check if the tap was long enough away from the previous tap.
        if (Time.time - lastClickTime < debounceDelay)
        {
            return;
        }

        //Update the last clicked time
        lastClickTime = Time.time;

        //Send the string to the server
        server.writeString(SendToServerString);
    }

    // Use this for initialization
    void Start()
    {
        //Initialize it
        server = GameObject.Find("ServerClient").GetComponent<ServerClientScript>();
    }
}
