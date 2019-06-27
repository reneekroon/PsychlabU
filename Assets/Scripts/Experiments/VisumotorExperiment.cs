using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisumotorExperiment : Experiment
{

    List<Texture2D> images = new List<Texture2D>();
    int imagesIterator = 0;
    List<Texture2D> usedImages = new List<Texture2D>();
    List<int> directions = new List<int>();


    public VisumotorExperiment() {

        // Currently there is not indication whether the image is new or has appeared before
        // correctAnswer - 0 left
        // correctAnswer - 1 right
        // correctAnswer - 2 top
        // correctAnswer - 3 bottom

        Start();

    }


    void Start()
    {

        // Making a list on numbers from 0 to 49 because I don't know how to do it
        List<int> numbers = new List<int>();
        for (int i = 0; i < 50; i++) {
            numbers.Add(i);
        }

        // Importing (loading) all 50 images in a random order, so no need to shuffle them later
        for (int i = 0; i < 50; i++) {
            int importable = numbers[Random.Range(0, numbers.Count)];
            numbers.Remove(importable);
            images.Add((Texture2D)Resources.Load("Images/test-" + importable));
        }
    }


    public override Texture2D GetNextTexture() {

        // 50% chance that the image has appeared alreay
        if (Random.Range(0,2) == 0) {
            // Only if there are images that have been shown already (when experiment has just started there are none)
            if (usedImages.Count > 0) {
                
                // Return random image that appeared already
                int randomIndex = Random.Range(0, usedImages.Count);
                correctAnswer = directions[randomIndex];

                // No special feature this time
                specialButtonFeatureCode = 0;

                return usedImages[randomIndex];
            }
        }

        // Otherwise pick the next image
        Texture2D pickedImage = images[imagesIterator];
        imagesIterator++;

        // Pick a random direction
        int randomDirection = Random.Range(0, 4);

        // And add it to images that have already appeared and also save the direction chosen for it
        directions.Add(randomDirection);
        usedImages.Add(pickedImage);

        // Special feature - makes one button (direction selected) green and the others red 
        specialButtonFeatureCode = 100 + randomDirection;

        correctAnswer = randomDirection;

        return pickedImage;
    }


}
