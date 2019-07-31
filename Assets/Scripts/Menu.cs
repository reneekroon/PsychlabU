using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class Menu : MonoBehaviour
{
    private GameObject searchOptions;
    private GameObject recognitionOptions;
    private GameObject visumotorOptions;
    private GameObject trackingOptions;


    public void StartGame() {

        // Get all inputs from LeftMenu
        int experiment = transform.Find("LeftMenu").Find("ExperimentDropdown").GetComponent<TMPro.TMP_Dropdown>().value;
        bool vr = transform.Find("LeftMenu").Find("VRToggle").GetComponent<Toggle>().isOn;
        bool counter = transform.Find("LeftMenu").Find("CounterToggle").GetComponent<Toggle>().isOn;
        bool logging = transform.Find("LeftMenu").Find("LoggingToggle").GetComponent<Toggle>().isOn;
        int limit = transform.Find("LeftMenu").Find("LimitTypeDropdown").GetComponent<TMPro.TMP_Dropdown>().value;

        // Set default length to 30 because 0 would end expreiments immediately
        int length = 0;
        int.TryParse(transform.Find("LeftMenu").Find("LengthInputField").GetComponent<TMPro.TMP_InputField>().text, out length);
        if (length == 0) length = 30;

        // Save the user inputs so they can be accessed later (in the next scene or after program is closed)
        PlayerPrefs.SetInt("experiment", experiment);
        PlayerPrefs.SetInt("vr", vr ? 1 : 0);
        PlayerPrefs.SetInt("counter", counter ? 1 : 0);
        PlayerPrefs.SetInt("logging", logging ? 1 : 0);
        PlayerPrefs.SetInt("limit", limit);
        PlayerPrefs.SetInt("length", length);

        // Finding the input parameters for specific experiments and saving them
        switch(experiment) {
            case 0:

                // Search Experiment
                int search_mode = searchOptions.transform.Find("SearchDropdown").GetComponent<TMPro.TMP_Dropdown>().value;
                int search_chance;
                if (!int.TryParse(searchOptions.transform.Find("SearchInputField").GetComponent<TMPro.TMP_InputField>().text, out search_chance)) {
                    // Use this value when parsing int failed 
                    search_chance = 65; 
                }
                bool search_random = searchOptions.transform.Find("SearchRandomToggle").GetComponent<Toggle>().isOn;

                PlayerPrefs.SetInt("search_mode", search_mode);
                PlayerPrefs.SetInt("search_chance", search_chance);
                PlayerPrefs.SetInt("search_random", search_random ? 1 : 0);

                break;
            case 1:
                
                // Recognition Experiment
                int recognition_chance;
                if (!int.TryParse(recognitionOptions.transform.Find("RecognitionInputField").GetComponent<TMPro.TMP_InputField>().text, out recognition_chance)) {
                    search_chance = 50; 
                }

                PlayerPrefs.SetInt("recognition_chance", recognition_chance);

                break;
            case 2:

                // Visumotor Experiment
                int visumotor_chance;
                if (!int.TryParse(visumotorOptions.transform.Find("VisumotorInputField").GetComponent<TMPro.TMP_InputField>().text, out visumotor_chance)) {
                    search_chance = 50; 
                }

                PlayerPrefs.SetInt("visumotor_chance", visumotor_chance);
            
                break;
            case 3:

                // Tracking Experiment 
                int tracking_speed;
                int tracking_length; 
                if (!int.TryParse(trackingOptions.transform.Find("TrackingSpeedInputField").GetComponent<TMPro.TMP_InputField>().text, out tracking_speed)) {
                    tracking_speed = 5; // This has to be divided by 10
                }
                if (!int.TryParse(trackingOptions.transform.Find("TrackingLengthInputField").GetComponent<TMPro.TMP_InputField>().text, out tracking_length)) {
                    tracking_length = 3; 
                }

                PlayerPrefs.SetInt("tracking_speed", tracking_speed);
                PlayerPrefs.SetInt("tracking_length", tracking_length);

                break;
        }

        PlayerPrefs.Save();

        // Load appropriate VR devicee and start the experiment 
        if (vr) {
            XRSettings.LoadDeviceByName("Oculus");
            SceneManager.LoadScene("VRRoom");
        } else {
            XRSettings.LoadDeviceByName("");
            SceneManager.LoadScene("FirstPersonRoom");
        }

    }

    public void Start() {

        // Find references to all the option menus for different experiments
        searchOptions = transform.Find("RightMenu").Find("SearchOptions").gameObject;
        recognitionOptions = transform.Find("RightMenu").Find("RecognitionOptions").gameObject;
        visumotorOptions = transform.Find("RightMenu").Find("VisumotorOptions").gameObject;
        trackingOptions = transform.Find("RightMenu").Find("TrackingOptions").gameObject;

        // When returning from FPSRoom the cursor has to be enabled again
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // Load setting from disk
        int experiment = PlayerPrefs.GetInt("experiment");
        bool vr = PlayerPrefs.GetInt("vr") == 1;
        bool counter = PlayerPrefs.GetInt("counter") == 1;
        bool logging = PlayerPrefs.GetInt("logging") == 1;
        int limit = PlayerPrefs.GetInt("limit");
        string length = PlayerPrefs.GetInt("length").ToString();

        // Apply setting from previous time they were set 
        transform.Find("LeftMenu").Find("ExperimentDropdown").GetComponent<TMPro.TMP_Dropdown>().value = experiment;
        transform.Find("LeftMenu").Find("VRToggle").GetComponent<Toggle>().isOn = vr;
        transform.Find("LeftMenu").Find("CounterToggle").GetComponent<Toggle>().isOn = counter;
        transform.Find("LeftMenu").Find("LoggingToggle").GetComponent<Toggle>().isOn = logging;
        transform.Find("LeftMenu").Find("LimitTypeDropdown").GetComponent<TMPro.TMP_Dropdown>().value = limit;
        transform.Find("LeftMenu").Find("LengthInputField").GetComponent<TMPro.TMP_InputField>().text = length;

        ShowExperimentOptions();

    }

    // This method is called when a new experiment is selected from the dropdown menu and should show the options specific for that experiment.
    public void ShowExperimentOptions() {

        // First disable all the options, so that if something was shown before it won't be shown any more
        searchOptions.SetActive(false);
        recognitionOptions.SetActive(false);
        visumotorOptions.SetActive(false);
        trackingOptions.SetActive(false);

        // Get what experiment was selected
        int experiment = transform.Find("LeftMenu").Find("ExperimentDropdown").GetComponent<TMPro.TMP_Dropdown>().value;

        // Set the options for that experiment to be active
        // Also load the previously used state from PlayerPrefs
        switch(experiment) {
            case 0:
                searchOptions.transform.Find("SearchDropdown").GetComponent<TMPro.TMP_Dropdown>().value = PlayerPrefs.GetInt("search_mode");
                searchOptions.transform.Find("SearchInputField").GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetInt("search_chance").ToString();
                searchOptions.transform.Find("SearchRandomToggle").GetComponent<Toggle>().isOn = PlayerPrefs.GetInt("search_random") == 1;
                searchOptions.SetActive(true);
                break;
            case 1:
                recognitionOptions.transform.Find("RecognitionInputField").GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetInt("recognition_chance").ToString();
                recognitionOptions.SetActive(true);
                break;
            case 2:
                visumotorOptions.transform.Find("VisumotorInputField").GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetInt("visumotor_chance").ToString();
                visumotorOptions.SetActive(true);
                break;
            case 3:
                trackingOptions.transform.Find("TrackingSpeedInputField").GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetInt("tracking_speed").ToString();
                trackingOptions.transform.Find("TrackingLengthInputField").GetComponent<TMPro.TMP_InputField>().text = PlayerPrefs.GetInt("tracking_length").ToString();
                trackingOptions.SetActive(true);
                break;
        }



    }

    public void Update() {

        // Because pressing "Start" button with VR headset on is too hard
        if (Input.GetAxis("Submit") > 0) {
            StartGame();
        // Pressing "Escape" in the menu quits
        } else if (Input.GetAxis("Cancel") > 0) {
            Application.Quit();
        }

    }

}
