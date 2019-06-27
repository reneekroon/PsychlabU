using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecognitionExperiment : Experiment
{

    List<Texture2D> images = new List<Texture2D>();
    int imagesIterator = 0;
    List<Texture2D> usedImages = new List<Texture2D>();


    public RecognitionExperiment() {

        // correctAnswer - 0 if image is new (left button)
        // correctAnswer - 1 if image already appeared before (right button)

        // No special button feature
        specialButtonFeatureCode = 0;

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
                correctAnswer = 1;
                return usedImages[Random.Range(0, usedImages.Count)];
            }
        }

        // Otherwise pick the next image
        Texture2D pickedImage = images[imagesIterator];
        imagesIterator++;
        // And add it to images that have already appeared
        usedImages.Add(pickedImage);
        correctAnswer = 0;

        return pickedImage;
    }


}
