using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public float timeUntilExplosion;
	public int range;
	public Sound explosionSound;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		timeUntilExplosion -= Time.deltaTime;
		if (timeUntilExplosion <= 0) {
			Explode ();
		}
	}

	public void Explode(){
		SpawnExplosion (range);
		SoundManager.Instance.GetComponent<SoundManager> ().PlaySound (explosionSound);
		Destroy (gameObject);
	}

    //Spawns the explosion effects. Stops if blocked by a wall.
	void SpawnExplosion(int r){
		Spawner.Instance.GetComponent<Spawner>().SpawnObject(Prefab.Explosion, gameObject.transform.position);

	    int x = (int) gameObject.transform.position.x;
	    int y = (int) gameObject.transform.position.y;
	    bool blocked = false;

	    for (int i = 0; i < r; i++)
	    {
	        if (!blocked)
	        {
				if (MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x, y + i + 1].GetComponent<Tile> ().type != TileType.Wall) {
					Spawner.Instance.GetComponent<Spawner> ().SpawnObject (Prefab.Explosion, (Vector2)gameObject.transform.position + Vector2.up * (i + 1));
				}
				if (MapGenerator.Instance.GetComponent<MapGenerator>().tileMap[x, y + i + 1].GetComponent<Tile>().type == TileType.Destructable
					|| MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x - (i + 1), y].GetComponent<Tile> ().type == TileType.Wall)
	            {
	                blocked = true;
	            }
	        }
	    }
		blocked = false;
	    for (int i = 0; i < r; i++)
	    {
	        if (!blocked)
	        {
				if(MapGenerator.Instance.GetComponent<MapGenerator>().tileMap[x + i + 1, y].GetComponent<Tile>().type != TileType.Wall){
					Spawner.Instance.GetComponent<Spawner>().SpawnObject(Prefab.Explosion, (Vector2) gameObject.transform.position + Vector2.right*(i + 1));
				}
				if (MapGenerator.Instance.GetComponent<MapGenerator>().tileMap[x + i + 1, y].GetComponent<Tile>().type == TileType.Destructable
					|| MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x - (i + 1), y].GetComponent<Tile> ().type == TileType.Wall)
                {
                    blocked = true;
                }
            }
	    }
		blocked = false;
	    for (int i = 0; i < r; i++)
	    {
	        if (!blocked)
	        {
				if (MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x, y - (i + 1)].GetComponent<Tile> ().type != TileType.Wall) {
					Spawner.Instance.GetComponent<Spawner> ().SpawnObject (Prefab.Explosion, (Vector2)gameObject.transform.position - Vector2.up * (i + 1));
				}
				if (MapGenerator.Instance.GetComponent<MapGenerator>().tileMap[x, y - (i + 1)].GetComponent<Tile>().type == TileType.Destructable
					|| MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x - (i + 1), y].GetComponent<Tile> ().type == TileType.Wall)
                {
                    blocked = true;
                }
            }
	    }
		blocked = false;
	    for (int i = 0; i < r; i++)
        {
            if (!blocked)
            {
				if (MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x - (i + 1), y].GetComponent<Tile> ().type != TileType.Wall) {
					Spawner.Instance.GetComponent<Spawner> ().SpawnObject (Prefab.Explosion, (Vector2)gameObject.transform.position - Vector2.right * (i + 1));
				}
				if (MapGenerator.Instance.GetComponent<MapGenerator>().tileMap[x - (i + 1), y].GetComponent<Tile>().type == TileType.Destructable
					|| MapGenerator.Instance.GetComponent<MapGenerator> ().tileMap [x - (i + 1), y].GetComponent<Tile> ().type == TileType.Wall)
                {
                    blocked = true;
                }
            }
        }
	}
}
