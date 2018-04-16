using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script gets infromation from the server client.
public class GetTextFromServer : MonoBehaviour
{
    //Define what information you want. The GetText variable can be eddited in the unity browser.
    public string GetText = null;
    
    //Add the serverclient
    private ServerClientScript server;

    //Add string the string that is going to be displayed
    private string text = null;

    // Use this for initialization
    void Start()
    {
        //Initialize it
        server = GameObject.Find("ServerClient").GetComponent<ServerClientScript>();
    }

    // Update is called once per frame
    void Update()
    {
        //Get text data from serverClient
        text = server.getString(GetText);

        if (text == null)
        {
            //If the text == null than no data has been recieved
            text = "Error no text recieved from server.";
        }

        //Update the text with the most recent update of the server time
        GetComponent<TextMesh>().text = text;
    }
}
