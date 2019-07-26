using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Attached to Crosshair GameObject
public class LookAtRaycast : MonoBehaviour
{
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject topButton;
    public GameObject bottomButton;
    public GameObject centerButton;
    public GameObject mainCamera;
    public LayerMask mask;

    private ScreenButtonController left;
    private ScreenButtonController right;
    private ScreenButtonController top;
    private ScreenButtonController bottom;
    private ScreenButtonController center;
    private int ignoreMask;
    private string last = "";
    

    void Start()
    {

        // Invert the mask so that the selected layer will be ignored
        ignoreMask =~ mask;

        left = leftButton.GetComponent<ScreenButtonController>();
        right = rightButton.GetComponent<ScreenButtonController>();
        top = topButton.GetComponent<ScreenButtonController>();
        bottom = bottomButton.GetComponent<ScreenButtonController>();
        center = centerButton.GetComponent<ScreenButtonController>();

    }

    void LateUpdate()
    {
        RaycastHit hit;


        // Raycast to determine what the player is looking at (transform name)
        if (Physics.Raycast (mainCamera.transform.position, mainCamera.transform.forward, out hit, 100000, ignoreMask)) {

            //Debug.DrawRay(mainCamera.transform.position, mainCamera.transform.forward * 100000, Color.red);

            // Vector from camera's position to raycast hit location
            Vector3 a = mainCamera.transform.rotation * Vector3.forward * hit.distance;
            // Vector form raycast hit location to a little in front of the raycast hit location
            // 0.006 should be enough to avoid clipping with buttons on screen object
            Vector3 b = hit.transform.rotation * Vector3.back * 0.006f;

            // Make the crosshair appear at the position the player is looking at and rotaed same way as the surface
            transform.position = mainCamera.transform.position + a + b;
            //transform.position = Vector3.Lerp(transform.position, mainCamera.transform.position + a + b, Time.deltaTime * 40);
            transform.rotation = hit.transform.rotation;

            string name = hit.transform.name;            

            // If looking at button but didnt look at it last frame, call the lookEnter method
            switch (name) {
                case "LeftButton":
                    if (last != "LeftButton") {
                        left.LookEnter();
                        last = name;
                    }
                    break;

                case "RightButton":      
                    if (last != "RightButton") {
                        right.LookEnter();
                        last = name;
                    }
                    break;

                case "TopButton":
                    if (last != "TopButton") {
                        top.LookEnter();
                        last = name;
                    }
                    break;

                case "BottomButton":
                    if (last != "BottomButton") {
                        bottom.LookEnter();
                        last = name;
                    }
                    break;

                case "CenterButtonCollider":
                    if (last != "CenterButtonCollider") {
                        center.LookEnter();
                        last = name;
                    }
                    break;

                // If looked at button last frame and not anymore, call the lookExit method
                // Also checks if these button still exist (because they get destroyed after the experiment ends)
                default:

                    if (last != ""){

                        switch (last) {
                            case "LeftButton":
                                if (leftButton) left.LookExit();
                                break;
                            case "RightButton":
                                if (rightButton) right.LookExit();
                                break;
                            case "TopButton":
                                if (topButton) top.LookExit();
                                break;
                            case "BottomButton":
                                if (bottomButton) bottom.LookExit();
                                break;
                            case "CenterButtonCollider":
                                if (centerButton) center.LookExit();
                                break;
                        }
                        last = "";
                    }
                    break;
            }

        }


    }
}
