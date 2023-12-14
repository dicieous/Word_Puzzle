using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private int gridHeight = 5;
    [SerializeField] private int gridWeight = 5;
    [SerializeField] private float gridSpacing = 2f;

    [SerializeField] private Transform stickingCubeObj;

    private void Start()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        for (int height = 0; height < gridHeight; height++)
        {
            for (int width = 0; width < gridWeight; width++)
            {
                var position = new Vector3(height * gridSpacing, width * gridSpacing, 0f);
                Instantiate(stickingCubeObj, position, Quaternion.identity, transform);
            }
        }
    }
}
