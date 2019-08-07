using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisumotorExperiment : Experiment
{

    private Object[] images;
    private int chance;
    private List<Texture2D> usedImages = new List<Texture2D>();
    private List<int> directions = new List<int>();
    private List<int> numbers = new List<int>();


    public VisumotorExperiment() {

        // Currently there is not indication whether the image is new or has appeared before
        // correctAnswer - 0 left
        // correctAnswer - 1 right
        // correctAnswer - 2 top
        // correctAnswer - 3 bottom

        // Set up sending data about experiment to log file
        logData.Add("imageName", "");
        logData.Add("isGuide", "");

        chance = PlayerPrefs.GetInt("visumotor_chance");

        Start();

    }


    void Start()
    {

        // Loading all images from the "Images" folder
        images = Resources.LoadAll("Images");

        // Struggling with shuffling the images
        for (int i = 0; i < images.Length; i++) {
            numbers.Add(i);
        }
    }


    public override Texture2D GetNextTexture() {

        // Chance that the image has appeared alreay
        if (Random.Range(0, 100) >= chance) {
            // Only if there are images that have been shown already (when experiment has just started there are none)
            if (usedImages.Count > 0) {
                
                // Return random image that appeared already
                int randomIndex = Random.Range(0, usedImages.Count);
                Texture2D randomImage = usedImages[randomIndex];
                logData["imageName"] = randomImage.name;
                logData["isGuide"] = "false";
                correctAnswer = directions[randomIndex];

                specialButtonFeatureCode = 0;
                
                return randomImage;
            }
        }

        // Otherwise pick the next image (picking by random index)
        int randomImageIndex = numbers[Random.Range(0, numbers.Count)];
        numbers.Remove(randomImageIndex);
        Texture2D pickedImage = (Texture2D)images[randomImageIndex];

        // Pick a random direction
        int randomDirection = Random.Range(0, 4);

        // And add it to images that have already appeared and also save the direction chosen for it
        directions.Add(randomDirection);
        usedImages.Add(pickedImage);

        // Special feature - makes one button (direction selected) green and the others red 
        specialButtonFeatureCode = 100 + randomDirection;

        logData["imageName"] = pickedImage.name;
        logData["isGuide"] = "true";
        correctAnswer = randomDirection;

        return pickedImage;
    }


}
