using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenButtonController : MonoBehaviour
{
    public GameObject screenObject;
    public string type;
    ScreenController screen;

    void Start()
    {
        screen = screenObject.GetComponent<ScreenController>();

        if (type == "center") {
            gameObject.GetComponent<Renderer>().material.color = Color.red;
            gameObject.SetActive(false);
        // Some experiments need more than 2 buttons, extra buttons are made inactive at the start
        } else if (transform.name == "TopButton") {
            gameObject.SetActive(false);
        } else if (transform.name == "BottomButton") {
            gameObject.SetActive(false);
        }

    }

        // Should be called when player looks at the button
        public void LookEnter()
    {
        if (type == "select") {
            gameObject.GetComponent<Renderer>().material.color = Color.green;
            if (transform.name == "LeftButton") {
                screen.SelectButtonHovered(0);
            } else if (transform.name == "RightButton") {
                screen.SelectButtonHovered(1);
            } else if (transform.name == "TopButton") {
                screen.SelectButtonHovered(2);
            } else if (transform.name == "BottomButton") {
                screen.SelectButtonHovered(3);
            }
        } else if (type == "center") {
            screen.CenterButtonActivated();
            // The button becomes hidden 
            gameObject.SetActive(false);
        }
    }

    // Should be called when player looks away from the button
    public void LookExit()
    {
        if (type == "select") {
            gameObject.GetComponent<Renderer>().material.color = Color.black;
            screen.SelectButtonActivated();
        }        
    }


    public void SpecialButtonFeature(int specialButtonFeatureCode) {

        Color paleGreen = new Color(0.4f, 0.6f, 0.4f, 1);
        Color paleRed = new Color(0.6f, 0.4f, 0.4f, 1);

        // 100 - 103 makes one button green and other buttons red

        if (specialButtonFeatureCode == 100) {
            if (transform.name == "LeftButton") {
                gameObject.GetComponent<Renderer>().material.color = paleGreen;
            } else {
                gameObject.GetComponent<Renderer>().material.color = paleRed;
            }
        } else if (specialButtonFeatureCode == 101) {
            if (transform.name == "RightButton") {
                gameObject.GetComponent<Renderer>().material.color = paleGreen;
            } else {
                gameObject.GetComponent<Renderer>().material.color = paleRed;
            }
        } else if (specialButtonFeatureCode == 102) {
            if (transform.name == "TopButton") {
                gameObject.GetComponent<Renderer>().material.color = paleGreen;
            } else {
                gameObject.GetComponent<Renderer>().material.color = paleRed;
            }
        } else if (specialButtonFeatureCode == 103) {
            if (transform.name == "BottomButton") {
                gameObject.GetComponent<Renderer>().material.color = paleGreen;
            } else {
                gameObject.GetComponent<Renderer>().material.color = paleRed;
            }
        }

    }


    // Used for positioning buttons in differnet places for different experiments as needed
    // Called when experiment is initiated
    public void PositionButtons(string position) {

        switch (position) {

            // Set "left" and "right" buttons to sides 
            case "left_and_right":
                switch (transform.name) {
                    case "LeftButton":
                        transform.Translate(-0.9f, 1.3f, 0);
                        break;
                    case "RightButton":
                        transform.Translate(0.9f, 1.3f, 0);
                        break;
                }
                break;
            
            // Set "left" and "right" buttons to sides and activate "top" and "bottom" buttons
            case "all_sides":
                switch (transform.name) {
                    // This also has to take into account the scale of the screen
                    case "LeftButton":
                        transform.Translate(-0.9f * transform.parent.localScale.x, 1.3f * transform.parent.localScale.y, 0);
                        break;
                    case "RightButton":
                        transform.Translate(0.9f * transform.parent.localScale.x, 1.3f * transform.parent.localScale.y, 0);
                        break;
                    case "TopButton":
                        gameObject.SetActive(true);
                        break;
                    case "BottomButton":
                        gameObject.SetActive(true);
                        break;
                }
                break; 
        }
    }
}