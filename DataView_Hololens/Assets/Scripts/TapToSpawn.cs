using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This function spawns prefabs
public class TapToSpawn : MonoBehaviour {

    //Set the prefab object that has to be spawned, this can be done in the unity editor
    public GameObject spawnObject = null;

    //Make the position variable
    private Vector3 newPosition;

    //The fucntion that spawnes the prefab object
    void Spawn()
    {
        // On each Select gesture spawn object, set the location randomly based on the location from wich the object is spawned
        newPosition = new Vector3(transform.position.x+ Random.Range(-0.5f, -0.3f), transform.position.y + Random.Range(0.5f, -0.5f), transform.position.z);
        //Instatiate the object
        Instantiate(spawnObject, newPosition, Quaternion.identity);
    }

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

        if (spawnObject != null)
        {
            //If no object is given, dont spawn
            Spawn();
        }
    }
}
