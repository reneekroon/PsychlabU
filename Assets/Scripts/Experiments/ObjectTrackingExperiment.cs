using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectTrackingExperiment : Experiment
{

    private GameObject[] circles;
    private int count;
    

    public ObjectTrackingExperiment(GameObject[] _circles) {

        // correctAnswer - 0 if generated image doesn't contain target shape (left button)
        // correctAnswer - 1 if generated image contains target shape with target color (right button)
        // target shape is the first element in shapes list (similarily for target color)

        // No special button feature
        specialButtonFeatureCode = 0;

        // Set up sending data about experiment to log file
        logData = new Dictionary<string, string>();
        logData.Add("circleCount", "");

        circles = _circles;
        
        Start();

    }

    void Start() {

    }


    public override Texture2D GetNextTexture() {

        // Setup the trial

        // Randomly Decide how many circles in experiment, minimum of 2, they come in pairs (half are green and half are not)
        count = 2 * Random.Range(1, circles.Length / 2 + 1);

        // Setup the circles
        for (int i = 0; i < count; i++) {

            List<GameObject> otherCircles = new List<GameObject>();
        
            // Make a list consisting of all the circles except this one
            for (int j = 0; j < count; j++) {
                if (i != j) {
                    otherCircles.Add(circles[j]);
                }
            }

            circles[i].GetComponent<MovingCircle>().Prepare(otherCircles);
            
        }

        // Make half of the circles green (rounded down)
        // These are the objects that you have to track with your eyes
        for (int i = 0; i < count / 2; i++) {
            circles[i].GetComponent<MovingCircle>().MakeTracked();
        }

        // Randomly select one circle that will be asked about after the movement
        // Also this returns whether that circle was marked (green) or not, so whether left button or right button is correct
        correctAnswer = circles[Random.Range(0, count)].GetComponent<MovingCircle>().MakeTarget();
        
        // Now that everything is ready the circles can start animation
        for (int i = 0; i < count; i++) {
            circles[i].GetComponent<MovingCircle>().PrepareMoving();
        }

        // Set data that can be accessed for putting it in the log file
        logData["circleCount"] = count.ToString();

        // Since this uses the circles no texture is returned, and the background remains same
        return null;
    }

}
