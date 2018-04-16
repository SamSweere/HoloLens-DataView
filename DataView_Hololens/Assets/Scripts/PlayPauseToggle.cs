using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script toggles the play pause texture. It also checks with the server if it is still correct.
public class PlayPauseToggle : MonoBehaviour {
    //Include a meshRenderer, this makes it possible to change the texture
    private MeshRenderer meshRenderer;

    //Add the serverclient
    private ServerClientScript server;

    //The commands that have to be send to the server
    public string playStr = "music#play";

    public string pauseStr = "music#pause";

    //Boolean for is playing
    private bool isPlaying = false;

    //The play and pause material
    public Material playMaterial;
    public Material pauseMaterial;

    //add timestaps to disable quick dubble taps that happen sometimes.
    private float lastClickTime = 0;
    private float debounceDelay = 0.005f;

    void OnSelect()
    {
        //Check if the tap was long enough away from the previous tap.
        if (Time.time - lastClickTime < debounceDelay)
        {
            return;
        }
        //Update the last clicked time
        lastClickTime = Time.time;

        //Switch the play button material and send the command to the server
        if (isPlaying)
        {
            server.writeString(pauseStr);
            meshRenderer.material = playMaterial;
        }
        else
        {
            server.writeString(playStr);
            meshRenderer.material = pauseMaterial;
        }
        isPlaying = !isPlaying;
    }

    // Use this for initialization
    void Start()
    {
        //Initialise the mesh renderer with the play material
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.material = playMaterial;
        
        //Get the serverclient object
        server = GameObject.Find("ServerClient").GetComponent<ServerClientScript>();
    }

    // Update is called once per frame
    private void Update()
    {
        //Get the play status form the server
        string status = server.getString("musicPlaying");

        //Check if the local play status is different from the play status of the server, if so change the material and the local status.
        if(status == "True" && !isPlaying)
        {
            meshRenderer.material = pauseMaterial;
            isPlaying = true;
        }
        else if(status == "False" && isPlaying)
        {
            meshRenderer.material = playMaterial;
            isPlaying = false;
        }
    }
}
