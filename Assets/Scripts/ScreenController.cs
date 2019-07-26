using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class ScreenController : MonoBehaviour
{

    public GameObject centerButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject topButton;
    public GameObject bottomButton;
    public GameObject uiTimer;
    public GameObject screenTimer;
    public GameObject experimentOverText;
    public Material transparentMaterial;

    private Experiment experimentController;
    private bool useScreenTimer;
    private TMPro.TextMeshProUGUI textTimer;
    private string experimentName;
    private GameObject[] circles;
    private int animationLength;
    private float trialStart = 0;
    private float sequenceStart = 0;
    private int trialsLength;
    private bool movingExperiment = false;
    private bool useTimer;
    private int trial = -1;
    private List<string> log = new List<string>(); 
    private bool experimentOver;


    void Start()
    {

        
        //gameObject.GetComponent<Renderer>().material = transparentMaterial;

        // Enable or disable VR here. It needs to be done separate from loading the device (done in the menu), otherwise VR doesn't seem to work
        if (SceneManager.GetActiveScene().name != "VRRoom") {
            XRSettings.enabled = false;
        } else {
            XRSettings.enabled = true;
        }

        // Select between the two different timers based on user selection in the menu
        // Also, uiTimer doesn't work in VR so select the other one for VR
        useScreenTimer = PlayerPrefs.GetInt("counter") == 1;
        if (useScreenTimer || PlayerPrefs.GetInt("vr") == 1) {
            textTimer = screenTimer.GetComponent<TMPro.TextMeshProUGUI>();
        } else {
            textTimer = uiTimer.GetComponent<TMPro.TextMeshProUGUI>();
        }

        gameObject.GetComponent<Renderer>().material.color = Color.white;
        experimentOver = false;

        // To get all the circle that need to move in tracking experiment
        circles = GameObject.FindGameObjectsWithTag("screenobject_circle");

        // Disable the circles. Maybe some need to be reactivated if we are doing the tracking experiment
        foreach (GameObject circle in circles) {
            circle.SetActive(false);
        }

        // Set experiment length limits as selected in the menu
        trialsLength = PlayerPrefs.GetInt("length");

        // Check if "experiment length in seconds" is selected
        useTimer = PlayerPrefs.GetInt("limit") == 0;

        // Get the animation length for the animated experiment
        animationLength = PlayerPrefs.GetInt("tracking_length");

        // Decode the dropdown selection number to experiment name
        string[] experimentNames = {"search", "recognition", "visumotor", "tracking"};
        experimentName = experimentNames[PlayerPrefs.GetInt("experiment")];

        // The selection from menu can be overridden here, useful if no menu button is added for the experiment 
        //experimentName = "example";

        // Set up the experiment (aka test sequence)
        // Also set the buttons to right places on the screen
        if (experimentName == "recognition") {
            experimentController = new RecognitionExperiment();
            leftButton.GetComponent<ScreenButtonController>().PositionButtons("left_and_right");
            rightButton.GetComponent<ScreenButtonController>().PositionButtons("left_and_right");
        } else if (experimentName == "search") {
            experimentController = new SearchExperiment();
            // This is default button configuration, so this does nothing (at the moment)
            leftButton.GetComponent<ScreenButtonController>().PositionButtons("bottom");
            rightButton.GetComponent<ScreenButtonController>().PositionButtons("bottom");
        } else if (experimentName == "visumotor") {
            experimentController = new VisumotorExperiment();
            leftButton.GetComponent<ScreenButtonController>().PositionButtons("all_sides");
            rightButton.GetComponent<ScreenButtonController>().PositionButtons("all_sides");
            topButton.GetComponent<ScreenButtonController>().PositionButtons("all_sides");
            bottomButton.GetComponent<ScreenButtonController>().PositionButtons("all_sides");
        } else if (experimentName == "tracking") {
            experimentController = new ObjectTrackingExperiment(circles);
            leftButton.GetComponent<ScreenButtonController>().PositionButtons("bottom");
            rightButton.GetComponent<ScreenButtonController>().PositionButtons("bottom");
            movingExperiment = true;
        } else if (experimentName == "example") {
            experimentController = new ExampleExperiment();
            // Configure buttons if needed
        }


    }

    void Update() 
    {

        // Pressing "Escape" now returns to main menu
        if (Input.GetAxis("Cancel") > 0) {
            SceneManager.LoadScene("Menu");
        }

        // Used to detect key press to return to menu after the experiment is over
        if (experimentOver) {
            if (Input.GetAxis("Submit") > 0) {
                SceneManager.LoadScene("Menu");
            }
        }

        // If the experiment length is limited by timer
        if (useTimer) {

            // If timer is running
            if (sequenceStart > 0) {

                // Update the timer showing on UI (for FPS controller)
                textTimer.text = (Time.time - sequenceStart).ToString("F0");

                // If time is over, end the test sequence
                if (Time.time - sequenceStart > trialsLength) {
                    EndTestSequence();
                }
            }
        } else {

            // If the experiment length is limited by the number of trials
            // Since the first "trial" is empty it starts at -1, but showing that would be bad UX
            if (trial >= 0) {
                textTimer.text = trial.ToString();
            }

        }
    }

    // Displays the next texture in the textures list on the screen
    public void CenterButtonActivated(){

        // The timer should start when the test subject looks at the center button for the first time
        if (sequenceStart == 0 && useTimer) {
            sequenceStart = Time.time;
        }

        // Use the experiment controller to get the next image to display
        gameObject.GetComponent<Renderer>().material.mainTexture = experimentController.GetNextTexture();

        trialStart = Time.time;

        if (!movingExperiment) {
            // Buttons are activated immediately
            ActivateButtons();
        } else {
            // If must wait for experiment to finish animation before enabling the buttons, it can be done here
            Invoke("ActivateButtons", animationLength);
        }
        
        
    }

    // Separated form CenterButtonActivated method, so that buttons can be activated after a delay
    void ActivateButtons() {

       // Make the selecton/control buttons visible
        leftButton.SetActive(true);
        rightButton.SetActive(true);
        // But don't activate the buttons that are supposed to be inactive, only activate when required
        if (experimentName == "visumotor") {
            topButton.SetActive(true);
            bottomButton.SetActive(true);
        }


        // Making the buttons default color again because button API is bad
        leftButton.gameObject.GetComponent<Renderer>().material.color = Color.black;
        rightButton.gameObject.GetComponent<Renderer>().material.color = Color.black;
        topButton.gameObject.GetComponent<Renderer>().material.color = Color.black;
        bottomButton.gameObject.GetComponent<Renderer>().material.color = Color.black;
        

        // If the experiment requires buttons to do something special it is checked and done now
        if (experimentController.specialButtonFeatureCode != 0) {
            leftButton.GetComponent<ScreenButtonController>().SpecialButtonFeature(experimentController.specialButtonFeatureCode);
            rightButton.GetComponent<ScreenButtonController>().SpecialButtonFeature(experimentController.specialButtonFeatureCode);
            topButton.GetComponent<ScreenButtonController>().SpecialButtonFeature(experimentController.specialButtonFeatureCode);
            bottomButton.GetComponent<ScreenButtonController>().SpecialButtonFeature(experimentController.specialButtonFeatureCode);
        }

    }

    // Makes the screen white
    // Also records which contorl button was pressed (left or right)
    public void SelectButtonActivated(){
        
        if(!movingExperiment) {
            // Make the screen blank
            gameObject.GetComponent<Renderer>().material.mainTexture = null;
        } else {
            // Because the "tracking" experiment uses moving gameobjects instead of a texture, reset these gameobjects instead
            foreach (GameObject circle in circles) {
                circle.GetComponent<MovingCircle>().Reset();
            }
        }

        // Deactivate the selection buttons. Done here because they all need to be deactivated 
        leftButton.SetActive(false);
        rightButton.SetActive(false);
        topButton.SetActive(false);
        bottomButton.SetActive(false);

        // The center button should be made visble now
        centerButton.SetActive(true);

        // Trial finished
        trial++;

        // If timer is not used, check for experiment over
        if (!useTimer && trial >= trialsLength) {
            EndTestSequence();
        }
        
    }

    // When the test subject looks at a select button the answer is locked in
    // Therefore record data: test time, button selected and correct answer
    public void SelectButtonHovered(int button) {

        float trialDuration = Time.time - trialStart;

        // Only gather and save the information when the test sequence is running (button is pressed once befor it starts) or trial-based limit is used and already doing trials
        if (sequenceStart != 0 || (!useTimer && trial >= 0)) {

            // Gather info that is necessary
            //string info = "Trial: " + (trial + 1) + ", Duration: " + trialDuration + ", Answer: " + button + ", Correct: " + experimentController.correctAnswer;
            string info = (trial + 1) + "," + trialDuration + "," + button + "," + experimentController.correctAnswer;
            log.Add(info);

        }
    }


    void EndTestSequence() {

        experimentOver = true;
        // Set this to negative so that the experiment doesn't accidentally start again
        sequenceStart = -1;

        // Do this or the buttons will come back with "tracking" experiment
        // Not really needed since buttons are destroyed now
        CancelInvoke();

        // Disable all the buttons
        // There was a bug that still caused the buttons to come back (when looking at the center button the same frame when experiment ends i think)
        // So this makes sure that they wont be coming back
        Destroy(centerButton);
        Destroy(topButton);
        Destroy(bottomButton);
        Destroy(leftButton);
        Destroy(rightButton);

        // Make the screen invisible (so that "experiment over" text wouldn't glitch behind it sometimes)
        gameObject.GetComponent<Renderer>().forceRenderingOff = true;  

        if (movingExperiment) {
            // Because the "tracking" experiment uses moving gameobjects instead of a texture, reset these gameobjects also
            foreach (GameObject circle in circles) {
                circle.GetComponent<MovingCircle>().Reset();
            }
        }

        // Show the "experiment over!" text
        experimentOverText.SetActive(true);


        // Check if logging is enabled
        if (PlayerPrefs.GetInt("logging") == 1) {

            // Save the experiment gathered information to a CSV file
            string fileName = Application.dataPath + "/Logs/" + experimentName + "-" + DateTime.Now.ToString("HH-mm-ss_dd-MM-yyyy") + ".csv";
            //string fileName = "Assets/Logs/" + experimentName + "-" + DateTime.Now.ToString("HH-mm-ss_dd-MM-yyyy") + ".csv";
            
            StreamWriter writer = new StreamWriter(fileName);
            writer.WriteLine("trial,duration,answer,correct");
            foreach (string line in log) {
                writer.WriteLine(line);
            }
            writer.Close();

        }
    }



}
