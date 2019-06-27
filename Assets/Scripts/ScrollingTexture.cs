using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTexture : MonoBehaviour
{

    public float scrollSpeed;

    void Update()
    {
        
        // Adds horizontal movement to the applied texture
        GetComponent<Renderer>().material.mainTextureOffset = new Vector2(Time.time * scrollSpeed, 0);

    }
}
