using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<GameObject> tileTypes;  //List of tile prefabs

	void Start () {
		GenerateMap("TestMap");     //Generates the map. "TestMap" should be a variable based on map selection
        Destroy(gameObject);        //Destroy this game object once it's functions are complete
	}

    //Creates tiles and sets them to the appropriate locations
    void GenerateMap(string fileName)
    {
        TextAsset txt = Resources.Load(fileName) as TextAsset;  //Load text file

        string[] data = txt.text.Split('\n');   //Split text file by line
        Tile temp;                              //Used to store tile temporarily

        //Generate each tile specified by the text file
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length - 1; x++)
            {
                //Create the tile
                temp = (Instantiate(tileTypes[Int32.Parse(data[y][x].ToString())]) as GameObject).GetComponent<Tile>();

                //Set tile location
                temp.x = x;
                temp.y = y;
                temp.SetLocation();
            }
        }

        //Move camera to middle position
        GameObject.Find("Main Camera").transform.position = new Vector3((data[0].Length - 1) / 2f,(data.Length-1) / 2f,-10);
        //Need to change camera size based on tile dimensions
    }


}
