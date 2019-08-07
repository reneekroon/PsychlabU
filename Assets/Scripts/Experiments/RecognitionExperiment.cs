using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognitionExperiment : Experiment
{

    //private List<Texture2D> images = new List<Texture2D>();
    private Object[] images;
    private int chance;
    private List<Texture2D> usedImages = new List<Texture2D>();
    private List<int> numbers = new List<int>();


    public RecognitionExperiment() {

        // correctAnswer - 0 if image is new (left button)
        // correctAnswer - 1 if image already appeared before (right button)

        // Set up sending data about experiment to log file
        logData.Add("imageName", "");

        chance = PlayerPrefs.GetInt("recognition_chance");

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
                // Select and return random image that appeared already
                Texture2D usedImage = usedImages[Random.Range(0, usedImages.Count)];
                logData["imageName"] = usedImage.name;
                correctAnswer = 1;
                return usedImage;
            }
        }

        // Otherwise pick the next image (picking by random index)
        int randomImageIndex = numbers[Random.Range(0, numbers.Count)];
        numbers.Remove(randomImageIndex);
        Texture2D pickedImage = (Texture2D)images[randomImageIndex];
        
        // And add it to images that have already appeared
        usedImages.Add(pickedImage);

        logData["imageName"] = pickedImage.name;
        correctAnswer = 0;
        return pickedImage;
    }


}
