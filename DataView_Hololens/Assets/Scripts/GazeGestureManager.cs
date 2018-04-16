using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }

    private bool placing = false;

    GestureRecognizer recognizer;

    //Add the serverclient
    private ServerClientScript server;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        //Initialize it
        server = GameObject.Find("ServerClient").GetComponent<ServerClientScript>();

        // Set up a GestureRecognizer to detect Select gestures.
        recognizer = new GestureRecognizer();
        recognizer.Tapped += (args) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            if (FocusedObject != null)
            {
                FocusedObject.SendMessageUpwards("OnSelect", SendMessageOptions.DontRequireReceiver);
                //Switch the placing bool
                placing = !placing;
                //Let the server know that an object has been tapped.
                server.writeString(FocusedObject.name + " tapped");
            }
        };
        recognizer.StartCapturingGestures();
    }

    // Update is called once per frame
    void Update()
    {

        if (placing)
        {
            //do nothing
        }
        else
        {
            // Figure out which hologram is focused this frame.
            GameObject oldFocusObject = FocusedObject;

            // Do a raycast into the world based on the user's
            // head position and orientation.
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hitInfo;
            if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
            {
                // If the raycast hit a hologram, use that as the focused object.
                FocusedObject = hitInfo.collider.gameObject;
            }
            else
            {
                // If the raycast did not hit a hologram, clear the focused object.
                FocusedObject = null;
            }

            // If the focused object changed this frame,
            // start detecting fresh gestures again.
            if (FocusedObject != oldFocusObject)
            {
                recognizer.CancelGestures();
                recognizer.StartCapturingGestures();
            }
        }


    }
}