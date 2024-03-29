﻿using UnityEngine;

public class FloorCube : MonoBehaviour
{
    private Color originalColor = Color.gray;
    public Color currActionColor = Color.red;
    public WeightDisplay weightDisplay;
    public (int, int) position;
    private float fadeScale;
    private bool IsMouseOver;

    private void Update()
    {
        if (!IsMouseOver)
        {
            fadeScale = Mathf.Clamp(fadeScale -= Time.deltaTime, 0.0f, 1.0f);
            GetComponent<Renderer>().material.color = Color.Lerp(originalColor, currActionColor, fadeScale);
        }
    }

    public void SetFadeScale(float time)
    {
        fadeScale = time;
    }

    private void OnMouseOver()
    {
        IsMouseOver = true;
        GetComponent<Renderer>().material.color = Color.blue;
        weightDisplay?.DisplaySelectedCube(this.gameObject);
    }

    private void OnMouseExit()
    {
        IsMouseOver = false;
        GetComponent<Renderer>().material.color = originalColor;
    }
}
