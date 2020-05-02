using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public GameObject floorCube;
    public int rows = 10;
    public int columns = 10;
    private GameObject[,] grid;

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

    private void Awake()
    {
        grid = new GameObject[rows, columns];

        // Generate Grid
        for (int r = 0; r < rows; ++r)
        {
            for (int c = 0; c < columns; ++c)
            {
                Vector3 position = new Vector3(c, transform.position.y, r);
                grid[r, c] = Instantiate(floorCube, position, Quaternion.identity) as GameObject;
            }          
        }
    }

    private void Update()
    {

    }
}
