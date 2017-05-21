using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;

public class MapGenerator : Singleton<MapGenerator>
{
    //public static MapGenerator i;
    public List<GameObject> tileTypes;  //List of tile prefabs
    public Tile[,] tileMap;             //Stores all tiles
	public Map mapToLoad;
    //private bool _isGenerated = false;
    
    //Dictionary of playerNum and username, gets updated whenever a player enters or leaves the lobby
    public Dictionary<int, string> playerArray = new Dictionary<int, string>();

	void Start ()
	{        
        //if (canGenerateMap) {
        //    GenerateMap();
        //    _isGenerated = true;
        //}
	}
    
    //Creates tiles and sets them to the appropriate locations
    //Gets called when game is about to start
    public void GenerateMap(/*string fileName*/)
    {
        TextAsset txt = Resources.Load(mapToLoad.ToString()) as TextAsset;  //Load text file

        string[] data = txt.text.Split('\n');           //Split text file by line
        Tile temp;                                      //Used to store tile temporarily
        Tile base_temp;

        tileMap = new Tile[data[0].Length, data.Length]; //Allocate tileMap

        //Generate each tile specified by the text file
        for (int y = 0; y < data.Length; y++) {
            int dataYLength = SystemInfo.operatingSystem.Substring(0, 3) == "Mac" ? data[y].Length : data[y].Length - 1;
            //int dataYLength = data[y].Length;

            for (int x = 0; x < dataYLength; x++) {
                int tileNum = Int32.Parse(data[y][x].ToString());

                Debug.Log("TileNum: " + tileNum);

                //Only instantiates a player if the txt file has 5-8 in it and that number exists in the playerArray
                if (tileNum >= 5 && playerArray.ContainsKey(tileNum)) {
                    Debug.Log("playerArray[tileNum] : " + playerArray[tileNum]);

                    temp = (Instantiate(tileTypes[tileNum]) as GameObject).GetComponent<Tile>();
                    Debug.Log("Instantiating a Player");
                    //Sets the player's playernumber and username
                    //% 5 is there because the playerNumber range is 0-3 and the player tile range is 5-8. Need to convert 5-8 to 0-3
                    temp.GetComponent<PlayerActions>().PlayerNumber = tileNum % 5;
                    temp.GetComponent<PlayerActions>().PlayerName = playerArray[tileNum];
                    
                    //Set tile location
                    temp.x = x;
                    temp.y = y;
                    temp.SetLocation();

                    base_temp = (Instantiate(tileTypes[1]) as GameObject).GetComponent<Tile>(); ;
                    base_temp.x = x;
                    base_temp.y = y;
                    base_temp.SetLocation();

                    tileMap[x, y] = base_temp.GetComponent<Tile>();
                }

                if (tileNum < 5) {
                    temp = (Instantiate(tileTypes[tileNum]) as GameObject).GetComponent<Tile>();

                    //Set tile location
                    temp.x = x;
                    temp.y = y;
                    temp.SetLocation();

                    tileMap[x, y] = temp.GetComponent<Tile>();
                }
            }
        }

        //Move camera to middle position
        GameObject.Find("Main Camera").transform.position = new Vector3((data[0].Length) / 2f, (data.Length) / 2f, -10);
        //Need to change camera size based on tile dimensions
        // }

        //GameManager.Instance.addPlayers();
        //GameManager.Instance.AssignPlayer(); //needs to take in playernumber
        GameManager.Instance.getPlayerReferences(); 
    }

    // Gets called when players are logging into lobby
    public void AddPlayerToMap(int playerNum, string username) {
        int newNum = playerNum + 5;
        Debug.Log("Adding player to map: " + newNum);
        playerArray.Add(newNum, username);
    }

    // Gets called when player leaves lobby
    public void RemovePlayerFromMap(int playerNum) {
        Debug.Log("Removing player from map");
        playerArray.Remove(playerNum+5);
    }
    
    void RandomizeWalls(string fileName) {
        System.Random random = new System.Random();
        int randNum = 0;
        String mapTxt = (Resources.Load(fileName) as TextAsset).text;

        // Randomizes the walls
        StringBuilder sb = new StringBuilder(mapTxt);
        for (int i = 0; i < sb.Length; ++i) {
            if (mapTxt[i] == '1') {
                randNum = random.Next(1, 4);
                if (randNum == 3) randNum = 4;
                sb[i] = randNum.ToString()[0];
            } else if (mapTxt[i] == '2' || mapTxt[i] == '4') {
                sb[i] = '1';
            } else { // if wall or player, don't change
                sb[i] = mapTxt[i];
            }
        }

        // Clears the spaces next to the players, only works when players are on the corners
        for (int i = 0; i < sb.Length; ++i) {
            if (sb[i] == '5' || sb[i] == '7') sb[i + 1] = '1';
            if (sb[i] == '6' || sb[i] == '8') sb[i - 1] = '1';
            if (sb[i] == '5' || sb[i] == '6') sb[i + 15] = '1';
            if (sb[i] == '7' || sb[i] == '8') sb[i - 15] = '1';
        }

        //Debug.Log(sb.ToString());
        System.IO.File.WriteAllText("Assets/Resources/" + fileName.ToString() + ".txt", sb.ToString());
    }
}

[System.Serializable]
public enum Map{
	TestMap,
    TestRandomWalls,
}
