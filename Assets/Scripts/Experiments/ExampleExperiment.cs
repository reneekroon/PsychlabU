using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleExperiment : Experiment
{
    public ExampleExperiment() {

        // Load settings from PlayerPrefs
        // Do special features if needed

    }

    public override Texture2D GetNextTexture() {
        
        // Make the texture or load an image
        Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);
        texture.SetPixel(0, 0, Color.red);

        // Also set the correct response button for this texture
        // correctAnswer - 0 left
        // correctAnswer - 1 right
        // correctAnswer - 2 top
        // correctAnswer - 3 bottom
        correctAnswer = 0;

        texture.Apply();
        return texture;

    }
}
