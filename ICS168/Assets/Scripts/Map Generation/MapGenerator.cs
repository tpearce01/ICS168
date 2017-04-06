using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public List<GameObject> tileTypes;

	void Start () {
		GenerateMap("TestMap");
        Destroy(gameObject);
	}

    //Creates tiles and sets them to the appropriate locations
    void GenerateMap(string fileName)
    {
        TextAsset txt = Resources.Load(fileName) as TextAsset;
        //Debug.Log(txt.text);

        string[] data = txt.text.Split('\n');
        Tile temp;
        for (int y = 0; y < data.Length; y++)
        {
            for (int x = 0; x < data[y].Length - 1; x++)
            {
                //Debug.Log(data[y][x]);
                temp = (Instantiate(tileTypes[Int32.Parse(data[y][x].ToString())]) as GameObject).GetComponent<Tile>();
                temp.x = x;
                temp.y = y;
                temp.SetLocation();
            }
        }

        //Move camera to middle position
        GameObject.Find("Main Camera").transform.position = new Vector3((data[0].Length - 1) / 2f,(data.Length-1) / 2f,-10);
        //Change size based on tile dimensions
    }


}
