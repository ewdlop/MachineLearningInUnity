﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    #region Singleton
    public static Grid instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    } 
    #endregion

    public GameObject floorCube;
    public int rows = 15;
    public int columns = 15;
    private GameObject[,] grid;

    //public List<GameObject> prevActions;
    //public List<GameObject> currActions;

    private void OnDrawGizmosSelected()
    {
        // Generate Grid
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < columns; ++c)
            {
                Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.5f); // Gray
                Vector3 position = new Vector3(c, transform.position.y, r);
                Gizmos.DrawCube(position, new Vector3(1, 1, 1));
            }          
        }
 
    }

    private void Start()
    {
        grid = new GameObject[rows, columns];

        // Generate Grid
        for (int c = 0; c < columns; ++c)
        {
            for (int r = 0; r < rows; ++r)
            {
                Vector3 position = new Vector3(r, transform.position.y, c);
                grid[r, c] = Instantiate(floorCube, position, Quaternion.identity) as GameObject;
            }          
        }
    }

    public void ClearColors()
    {
        for (int r = 0; r < grid.GetLength(0); ++r)
        {
            for (int c = 0; c < grid.GetLength(1); ++c)
            {
                grid[r, c].GetComponent<FloorCube>().SetFadeScale(0.0f);
            }
        }
    }

    public void UpdateColor(int x, int y)
    {
        // Set fading 
        grid[x, y].GetComponent<FloorCube>().SetFadeScale(2.0f);
    }
}
