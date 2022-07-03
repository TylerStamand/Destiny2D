using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = System.Random;

public class Generator {

    
    public static bool[,] GenerateMap(GenerationOptions options) {
        Generator generator = new Generator(options);
        generator.InitializeMap();
        
        for (int i = 0; i < options.NumberOfSteps; i++) {
            generator.SimulationStep();
        }
        return generator.cellMap;
    }

    int width, height, deathLimit, birthLimit;
    
    float initialChance; 

    bool[,] cellMap;

    Random random;

    Generator(GenerationOptions options) {
        random = new Random(options.Seed);
        this.width = options.Width;
        this.height = options.Height;
        this.deathLimit = options.DeathLimit;   
        this.birthLimit = options.BirthLimit;
        this.initialChance = options.InitialChance01;
        cellMap = new bool[width, height];

    
    }


    void InitializeMap() {
        for(int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(random.NextDouble() < initialChance) {
                    cellMap[x,y] = true;
                }
                else {
                    cellMap[x, y] = false;
                }
            }
        }
    
    }


    void SimulationStep() {
        bool[,] newMap = new bool[width, height];
        Array.Copy(cellMap, newMap, width * height );


        for (int x = 0; x < cellMap.GetLength(0); x++) {
            for (int y = 0; y < cellMap.GetLength(1); y++) {
                int neighbors = CountAliveNeighbors(cellMap, x, y);

                if(cellMap[x,y]) {
                    if(neighbors < deathLimit) {
                        newMap[x,y] = false;
                    }
                    else {
                        newMap[x,y] = true;
                    }
                }
                else {
                    if(neighbors > birthLimit) {
                        newMap[x,y] = true;
                    }
                    else {
                        newMap[x, y] = false;
                    }
                }
            }
        }

        Array.Copy(newMap, cellMap, width * height);

    }


    int CountAliveNeighbors(bool[,] map,  int x, int y) {
        int count = 0;
        for(int i = -1; i < 2; i++) {
            for(int j = -1; j < 2; j++) {
                int neighborX = x+i;
                int neighborY = y+j;

                //If looking at the middle point
                if(i == 0 && j == 0) {
                   continue;
                }
                else if(neighborX < 0 || neighborY < 0 || neighborX >= width || neighborY >= height) {
                    count += 1;
                }
                else if(map[neighborX,neighborY]) {
                    count += 1;
                }
            } 
        }

        return count;
    }
 
}


public struct GenerationOptions {
    public int Seed;
    public int Width;
    public int Height;
    public int DeathLimit;
    public int BirthLimit; 
    public float InitialChance01;
    public int NumberOfSteps;
}