﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : Singleton<MapGenerator>
{
    //public static MapGenerator i;
    public List<GameObject> tileTypes;  //List of tile prefabs
    public Tile[,] tileMap;             //Stores all tiles
	public Map mapToLoad;


	void Start ()
	{
		GenerateMap(mapToLoad.ToString());
	}

    //Creates tiles and sets them to the appropriate locations
    void GenerateMap(string fileName)
    {
        TextAsset txt = Resources.Load(fileName) as TextAsset;  //Load text file

        string[] data = txt.text.Split('\n');           //Split text file by line
        Tile temp;                                      //Used to store tile temporarily

        tileMap = new Tile[data[0].Length,data.Length]; //Allocate tileMap

        //Generate each tile specified by the text file
        for (int y = 0; y < data.Length; y++)
        {
            int dataYLength = SystemInfo.operatingSystem.Substring(0, 3) == "Mac" ? data[y].Length : data[y].Length - 1;

            for (int x = 0; x < data[y].Length - 1; x++)
            {
                //Create the tile
                temp = (Instantiate(tileTypes[Int32.Parse(data[y][x].ToString())]) as GameObject).GetComponent<Tile>();

                //Set tile location
                temp.x = x;
                temp.y = y;
                temp.SetLocation();

                tileMap[x, y] = temp.GetComponent<Tile>();
            }
        }

        //Move camera to middle position
        GameObject.Find("Main Camera").transform.position = new Vector3((data[0].Length) / 2f,(data.Length) / 2f,-10);
        //Need to change camera size based on tile dimensions
    }


}

[System.Serializable]
public enum Map{
	TestMap
}
