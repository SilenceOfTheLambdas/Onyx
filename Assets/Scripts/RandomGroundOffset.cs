using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;
using URandom = UnityEngine.Random;

[ExecuteInEditMode]
public class RandomGroundOffset : MonoBehaviour
{
    [SerializeField] private List<GameObject> groundTiles = new List<GameObject>();
    [SerializeField] private int              numberOfTilesToAffect;
    [SerializeField] private float            minYValue, maxYValue;

    private int _oldNumberOfTilesValue;
    private void Awake()
    {
        var random = new Random();
        for (var i = numberOfTilesToAffect; i <= groundTiles.Count && i <= numberOfTilesToAffect; i++)
        {
            var groundTile = groundTiles[random.Next(0, groundTiles.Count)];
            groundTile.transform.position =
                new Vector3(groundTile.transform.position.x, URandom.Range(minYValue, maxYValue), groundTile.transform.position.z);
        }
        _oldNumberOfTilesValue = numberOfTilesToAffect;
    }

    private void Update()
    {
        if (numberOfTilesToAffect != _oldNumberOfTilesValue)
            GenerateNewRandomOffsets();
        
    }
    
    private void GenerateNewRandomOffsets()
    {
        var random = new Random();
        for (var i = numberOfTilesToAffect; i <= groundTiles.Count && i <= numberOfTilesToAffect; i++)
        {
            var groundTile = groundTiles[random.Next(0, groundTiles.Count)];
            groundTile.transform.position =
                new Vector3(groundTile.transform.position.x, URandom.Range(minYValue, maxYValue), groundTile.transform.position.z);
        }
    }
}
