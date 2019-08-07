using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Experiment
{

    // This must be set after the GetNextTexture method is called and before it returns the texture
    public int correctAnswer;

    public int specialButtonFeatureCode = 0;

    public abstract Texture2D GetNextTexture();

    // Used to get data from experiment to log file 
    public Dictionary<string, string> logData = new Dictionary<string, string>();


}
