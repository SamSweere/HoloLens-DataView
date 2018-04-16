using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This function makes an object invisible
public class TapToDestroyParent : MonoBehaviour
{
    void OnSelect()
    {
        //Make the object invisible, there was a bug with destroying, the whole program would crash.
        transform.parent.gameObject.SetActive(false);
    }
}


