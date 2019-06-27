using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Experiment
{

    // This must be set after the GetNextTexture method is called and befor it returns the texture
    public int correctAnswer;

    public int specialButtonFeatureCode;

    public abstract Texture2D GetNextTexture();


}
