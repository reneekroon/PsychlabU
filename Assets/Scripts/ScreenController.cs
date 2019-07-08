using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenController : MonoBehaviour
{

    public GameObject centerButton;
    public GameObject leftButton;
    public GameObject rightButton;
    public GameObject topButton;
    public GameObject bottomButton;
    Experiment experimentController;
    public GameObject uiTimer;
    public GameObject screenTimer;
    public bool useScreenTimer = false;
    TMPro.TextMeshProUGUI textTimer;
    public string experimentName;
    public int experimentMode = 0;
    GameObject[] circles;
    float trialStart = 0;
    float sequenceStart = 0;
    int sequenceLength = 30;
    bool movingExperiment = false;
    public bool useTimer = true;
    int trial = -1;
    public int maxTrials = 10;



    void Start()
    {

        if (useScreenTimer) {
            textTimer = screenTimer.GetComponent<TMPro.TextMeshProUGUI>();
        } else {
            textTimer = uiTimer.GetComponent<TMPro.TextMeshProUGUI>();
        }

        gameObject.GetComponent<Renderer>().material.color = Color.white;

        // To get all the circle that need to move in tracking experiment
        circles = GameObject.FindGameObjectsWithTag("screenobject_circle");

        // Disable the circles. Maybe some need to be reactivated if we are doing the tracking experiment
        foreach (GameObject circle in circles) {
            circle.SetActive(false);
        }

        // Set up the experiment (aka test sequence)
        // Also set the buttons to right places on the screen
        // TODO add some sort of system/menu to select the experiment

        if (experimentName == "recognition") {
            experimentController = new RecognitionExperiment();
            leftButton.GetComponent<ScreenButtonController>().PositionButtons("left_and_right");
            rightButton.GetComponent<ScreenButtonController>().PositionButtons("left_and_right");
        } else if (experimentName == "search") {
            experimentController = new SearchExperiment(experimentMode);
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
        }


    }

    void Update() 
    {
        // If the experiment length is limited by timer
        if (useTimer) {

            // If timer is running
            if (sequenceStart > 0) {

                // Update the timer showing on UI (for FPS controller)
                textTimer.text = (Time.time - sequenceStart).ToString("F0");

                // If time is over, end the test sequence
                if (Time.time - sequenceStart > sequenceLength) {
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
            float animationLength = 3f;
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

        // The center button should be makde visble now
        centerButton.SetActive(true);

        // Trial finished
        trial++;

        // If timer is not used, check for experiment over
        if (!useTimer && trial >= maxTrials) {
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
            string info = "Trial: " + (trial + 1) + ", Duration: " + trialDuration + ", Answer: " + button + ", Correct: " + experimentController.correctAnswer;

            // TODO do something with the info
            Debug.Log(info);
        }
    }


    void EndTestSequence() {

        // End the timer calculations
        Debug.Log("Test over!");
        // Set this to negative so that the experiment doesn't accidentally start again
        sequenceStart = -1;

        // Do this or the buttons will come back with "tracking" experiment
        CancelInvoke();

        // Disable all the buttons
        centerButton.SetActive(false);
        leftButton.SetActive(false);
        topButton.SetActive(false);
        bottomButton.SetActive(false);
        rightButton.SetActive(false);

        if(!movingExperiment) {
            // Make the screen blank
            gameObject.GetComponent<Renderer>().material.mainTexture = null;
        } else {
            // Because the "tracking" experiment uses moving gameobjects instead of a texture, reset these gameobjects instead
            foreach (GameObject circle in circles) {
                circle.GetComponent<MovingCircle>().Reset();
            }
        }

        return;
    }



}
