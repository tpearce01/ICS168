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

    public List<int> playerNumArray = new List<int>();

	void Start ()
	{
        playerNumArray.Add(5);
        playerNumArray.Add(6);
        EditGameMap();
        
        //if (canGenerateMap) {
        //    GenerateMap();
        //    _isGenerated = true;
        //}
	}
    
    //Creates tiles and sets them to the appropriate locations
    public void GenerateMap(/*string fileName*/)
    {
        //if (!_isGenerated) {
           // _isGenerated = true;
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
                if (Int32.Parse(data[y][x].ToString()) >= 5) {
                    temp = (Instantiate(tileTypes[Int32.Parse(data[y][x].ToString())]) as GameObject).GetComponent<Tile>();
                    //Set tile location
                    temp.x = x;
                    temp.y = y;
                    temp.SetLocation();

                    //// Check if the created gameObject is a player object.
                    //if (temp.GetComponent<PlayerActions>() != null) {
                    //    _players.Add(temp.GetComponent<PlayerActions>());
                    //}

                    base_temp = (Instantiate(tileTypes[1]) as GameObject).GetComponent<Tile>(); ;
                    base_temp.x = x;
                    base_temp.y = y;
                    base_temp.SetLocation();

                    tileMap[x, y] = base_temp.GetComponent<Tile>();
                } else {
                    temp = (Instantiate(tileTypes[Int32.Parse(data[y][x].ToString())]) as GameObject).GetComponent<Tile>();

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
    }

    // Gets called when players are logging into lobby
    void AddPlayersToMap(int playerNum) {
        playerNumArray.Add(playerNum + 5);
    }

    // Gets called when player leaves lobby
    void RemovePlayersFromMap(int playerNum) {
        playerNumArray.Remove(playerNum + 5);
    }

    // Only gets called when countdown to game is starting, when players can't leave lobby
    void EditGameMap() {
        String newMap = (Resources.Load("GameMap") as TextAsset).text;
        StringBuilder sb = new StringBuilder(newMap);

        //Reset players
        sb[16] = '1';
        sb[26] = '1';
        sb[106] = '1';
        sb[116] = '1';

        for (int i = 0; i < playerNumArray.Count; ++i) {
            if (playerNumArray[i] == 5) {
                sb[16] = '5';
            }
            if (playerNumArray[i] == 6) {
                sb[26] = '6';
            }
            if (playerNumArray[i] == 7) {
                sb[106] = '7';
            }
            if (playerNumArray[i] == 8) {
                sb[116] = '8';
            }
        }
        
        File.WriteAllText("Assets/Resources/GameMap.txt", sb.ToString());

        FileInfo mapFile = new FileInfo("Assets/Resources/GameMap.txt");
        Debug.Log("Last write time: " + mapFile.LastWriteTime);
        while (IsFileLocked(mapFile)) {
            Debug.Log("Is file locked? " + (IsFileLocked(mapFile) ? "yes" : "no"));
        }
        Debug.Log("I guess file isn't locked?");
        GenerateMap();
    }

    private bool IsFileLocked(FileInfo file) {
        FileStream stream = null;

        try {
            stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
        } catch (IOException) {
            //the file is unavailable because it is:
            //still being written to
            //or being processed by another thread
            //or does not exist (has already been processed)
            return true;
        } finally {
            if (stream != null)
                stream.Close();
        }

        //file is not locked
        return false;
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
    GameMap,
}
