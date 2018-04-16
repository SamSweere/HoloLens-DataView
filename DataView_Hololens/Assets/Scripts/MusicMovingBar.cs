using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script handles the movement of the music bar.
public class MusicMovingBar : MonoBehaviour {

    //Add the serverclient
    private ServerClientScript server;

    //The lenght of the song
    private string length = null;

    //How much of the song is already done
    private string done = null;

    //Begin location of the moving bar
    private Vector3 begin;
    
    // Use this for initialization
    void Start () {

        //Initialize it
        server = GameObject.Find("ServerClient").GetComponent<ServerClientScript>();

        //Since the bar will be moving and the whole music object can be moved, we can not use that for the start location. 
        //Therefore the moving bar is located inside an empty object that stays on the start location at all times.
        begin = this.transform.parent.position;
    }
	
    //Convert a time string with the format (mm:ss) to the total seconds as an intereger.
    int TimeToInt(string time)
    {
        int totalTime = 0;
        string[] str = time.Split(':');
        totalTime += int.Parse(str[0]) * 60;
        totalTime += int.Parse(str[1]);
        return totalTime;
    }

    //Move the bar relative to the begin location, total time and time played.
    void MoveBar(int now, int total)
    {
        transform.position = begin + new Vector3(((float)now / ((float)total))*0.263f, 0, 0);
    }

	// Update is called once per frame
	void Update () {
        //Since the whole music player can be moved we have to update the begin location every frame
        begin = this.transform.parent.position;

        //Get the total lenght of the song from the serverclient.
        length = server.getString("musicLength");

        if (length == null)
        {
            //If the text == null than no data has been recieved
            length = "99:99";
        }

        //Get the time done from the serverclient.
        done = server.getString("musicTime");

        if (done == null)
        {
            //If the text == null than no data has been recieved
            done = "00:00";
        }

        //Convert the serverlcient information and move the bar.
        MoveBar(TimeToInt(done), TimeToInt(length));
    }
}
