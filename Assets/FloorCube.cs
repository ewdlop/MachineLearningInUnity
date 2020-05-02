using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorCube : MonoBehaviour
{
    private Color originalColor = Color.gray;
    public Color currActionColor = Color.red;

    private float fadeScale = 0.0f; 
    // Update is called once per frame
    void Update()
    {
        fadeScale = Mathf.Clamp(fadeScale -= Time.deltaTime, 0.0f, 1.0f);
        GetComponent<Renderer>().material.color = Color.Lerp (originalColor, currActionColor, fadeScale);
    }

    // Sets the time it takes to fade the floor cube back to its original color
    public void SetFadeScale(float time)
    {
        fadeScale = time;
    }
}
