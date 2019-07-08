using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCircle : MonoBehaviour
{

    bool tracked = false;
    bool target = false;
    bool moving = false;
    public float speed = 0.5f;
    Vector3 direction;
    List<GameObject> otherCircles;
    float maxDistanceFromCenter;

    // Start is called before the first frame update
    void Start()
    {
        // Max distance from center should be half the size of screen (area thet shows textures), that is 1 unit
        // Screen and the circles have same parent object that should be scaled, so the screen scale should not be accounted for here
        // However, the circles scale has to be considered, so calculate the max distance from center here:
        maxDistanceFromCenter = 1f / transform.parent.localScale.x;

    }

    // Update is called once per frame
    void Update()
    {
        // Moving logic is in update because it didn't seem to work properly in a coroutine
        if  (moving) {

            transform.Translate(direction * Time.deltaTime * speed);

            // if the circle moves out of the screen bounds assign new direction so that it wouldn't
            if (transform.localPosition.x > maxDistanceFromCenter) {
                direction = GetNewDirection("right");
            } else if (transform.localPosition.x < -maxDistanceFromCenter) {
                direction = GetNewDirection("left");
            } else if (transform.localPosition.y > maxDistanceFromCenter) {
                direction = GetNewDirection("down");
            } else if (transform.localPosition.y < -maxDistanceFromCenter) {
                direction = GetNewDirection("up");
            } 

            // Check overlap with all other circles that are active in the experiment
            foreach (GameObject otherCircle in otherCircles) {

                // Direction towards other circle
                Vector3 directionFromOther = otherCircle.transform.localPosition - transform.localPosition;

                // If the other circle is too close the direction should be changed so that they start moving away from each other
                // Distance of 2 works, regardless of scale
                if (directionFromOther.magnitude < 2) {

                    if (Mathf.Abs(directionFromOther.y) > Mathf.Abs(directionFromOther.x)) {
                        // Should bounce either up or down
                        if (directionFromOther.y > 0) {
                            direction = GetNewDirection("down");
                        } else {
                            direction = GetNewDirection("up");
                        }
                    } else {
                        // Should bounce either left or right
                        if (directionFromOther.x > 0) {
                            direction = GetNewDirection("right");
                        } else {
                            direction = GetNewDirection("left");
                        }
                    }

                    
                }
            }
        }
    }

    public void PrepareMoving() {

        // Before the circles start moving they need to show their true color for a second
        // So nothing has to be done, just wait for a second
        Invoke("StartMoving", 1);
    }

    void StartMoving() {

        // After waiting for a second all the circles become the same color
        gameObject.GetComponent<Renderer>().material.color = Color.black;

        // Then start moving
        moving = true;

        // Stop moving after a few seconds
        Invoke("StopMoving", 2);

    }


    void StopMoving() {
        
        moving = false;

        if (target) {
            // After moving the target circle should be revealed and test subject has to tell whether it was green or not at first
            gameObject.GetComponent<Renderer>().material.color = Color.blue;
        }

    }


    public void Prepare(List<GameObject> _otherCircles) {

        // References to all the other circles that are active in the experiment (to get their positions)
        otherCircles = _otherCircles;

        // The circle starts at a random position but should not overlap any other circles, so look for a vacant position
        bool lookingForPosition = true;
        while (lookingForPosition) {

            // Assign a random position to circle
            // Max deviation from center should be 1 unit so that the circles won't overlap the buttons (but no need to multiply by 1 of course)
            // Also take into account the scale of the screen, so when the screen is smaller the distance should be also smaller
            float maxDeviation = transform.parent.parent.localScale.x;

            transform.Translate(Random.Range(-maxDeviation, maxDeviation), Random.Range(-maxDeviation, maxDeviation), 0);
            lookingForPosition = false;

            foreach (GameObject otherCircle in otherCircles) {
                if (Vector3.Distance(otherCircle.transform.localPosition, transform.localPosition) < 2) {
                    // If another circle is too close reset position and keep searching
                    transform.localPosition = new Vector3(0, 0, 0);
                    lookingForPosition = true;
                    break;
                }
            }

        }

        // Assign a random direction the circle will start moving in once StartMoving is called
        direction = GetNewDirection("");
        

        // Activate the circles that partake in the experiment (that is decided controlled by the experiment contoller script)
        gameObject.SetActive(true);

    }

    public void MakeTracked() {

        gameObject.GetComponent<Renderer>().material.color = Color.green;
        tracked = true;

    }

    public int MakeTarget() {

        target = true;
        return tracked ? 1 : 0;

    }

    public void Reset() {

        // Just so they all dont have to change color, but the target has to be made black form blue again
        if(target) { gameObject.GetComponent<Renderer>().material.color = Color.black; }

        // Forget all special properties
        tracked = false;
        target = false;
        
        // Reset the position, this is local position
        transform.localPosition = new Vector3(0, 0, 0);

        // Deactivate so that only a specific amount could be selected again
        gameObject.SetActive(false);

        // Forget other circles just in case
        otherCircles = new List<GameObject>();

        // No point in resetting direction though, as prepare is called again anyway

    }

    Vector3 GetNewDirection(string side) {

        // "side" shows in which direction is the obstruction, moving towards that side should be avoided 
        switch (side) {
            case "up":
                return new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), 0).normalized;
                //break;
            case "down":
                return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 0f), 0).normalized;
                //break;
            case "left":
                return new Vector3(Random.Range(0f, 1f), Random.Range(-1f, 1f), 0).normalized;
                //break;
            case "right":
                return new Vector3(Random.Range(-1f, 0f), Random.Range(-1f, 1f), 0).normalized;
                //break;
        }

        // when no valid "side" is given returns a random direction
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized;
    }

}
