using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtRaycast : MonoBehaviour
{
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject topButton;
    public GameObject bottomButton;
    public GameObject centerButton;
    ScreenButtonController left;
    ScreenButtonController right;
    ScreenButtonController top;
    ScreenButtonController bottom;
    ScreenButtonController center;

    string last = "";
    

    void Start()
    {
        left = leftButton.GetComponent<ScreenButtonController>();
        right = rightButton.GetComponent<ScreenButtonController>();
        top = topButton.GetComponent<ScreenButtonController>();
        bottom = bottomButton.GetComponent<ScreenButtonController>();
        center = centerButton.GetComponent<ScreenButtonController>();
    }

    void Update()
    {
        RaycastHit hit;


        // Raycast to determine what the player is looking at (transform name)
        if (Physics.Raycast (transform.position, transform.forward, out hit)) {

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
                default:

                    if (last != ""){

                        switch (last) {
                            case "LeftButton":
                                left.LookExit();
                                break;
                            case "RightButton":
                                right.LookExit();
                                break;
                            case "TopButton":
                                top.LookExit();
                                break;
                            case "BottomButton":
                                bottom.LookExit();
                                break;
                            case "CenterButtonCollider":
                                center.LookExit();
                                break;
                        }
                        last = "";
                    }
                    break;
            }

        }


    }
}
