using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Dungeon : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] Tile floorTile;
    [SerializeField] Tile wallTile;
    [SerializeField] int seed;
    [SerializeField] int width;
    [SerializeField] int height;
    [SerializeField] int deathLimit;
    [SerializeField] int birthLimit;
    [Range(0,1)]
    [SerializeField] float initialChance;
    [SerializeField] int NumberOfSteps;


    [ContextMenu("GenerateMap")]
    void GenerateMap() {
        tilemap.ClearAllTiles();

        GenerationOptions options = new GenerationOptions {
            Seed = seed,
            Width = width,
            Height = height,
            DeathLimit = deathLimit,
            BirthLimit = birthLimit,
            NumberOfSteps = NumberOfSteps,
            InitialChance01 = initialChance
        };

        bool[,] generatedMap = Generator.GenerateMap(options);


        for (int i = 0; i < generatedMap.GetLength(0); i++) {
            for (int j = 0; j < generatedMap.GetLength(1); j++) {
                if (generatedMap[i, j]) {
                    tilemap.SetTile(new Vector3Int(i, j), floorTile);
                }
                else {
                    tilemap.SetTile(new Vector3Int(i, j), wallTile);
                }
            }
        }
    }


}
