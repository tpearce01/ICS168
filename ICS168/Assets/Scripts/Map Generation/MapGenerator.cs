using System;
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
        //Uncomment to randomize walls
        //RandomizeWalls("Assets/Resources/" + mapToLoad.ToString() + ".txt");
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

            for (int x = 0; x < dataYLength; x++)
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

    void RandomizeWalls(string filePath)
    {
        System.Random random = new System.Random();
        int randNum = 0;
        string mapTxt = System.IO.File.ReadAllText(filePath);
        string newMap = "";

        for(int i=0; i<mapTxt.Length; ++i)
        {
            if(mapTxt[i] == '1')
            {
                randNum = random.Next(1, 4);
                if (randNum == 3) randNum = 4;
                newMap += randNum.ToString()[0];
            }else
            {
                newMap += mapTxt[i];
            }
        }
        System.IO.File.WriteAllText(filePath, newMap);
    }
}

[System.Serializable]
public enum Map{
	TestMap,
    TestRandomWalls
}
