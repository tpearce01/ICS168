using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float timeToExist;
	public Sound destructableWallDestroyedSound;

    void Awake()
    {
        //Check if explosion is out of bounds
        int x = (int)gameObject.transform.position.x;
        int y = (int)gameObject.transform.position.y;
		if (x < 0 || x >= MapGenerator.Instance.tileMap.GetLength(0)
			|| y < 0 || y >= MapGenerator.Instance.tileMap.GetLength(1))
        {
            Destroy(gameObject);
        }
    }

	void Update(){
        //Decrement time to exist, and destroy if that time reaches 0
		timeToExist -= Time.deltaTime;
		if (timeToExist <= 0) {
			Destroy (gameObject);
            
		}
	}

    //Destroy explosion on contact with indestructable object
    //Destroy other object on contact with destructable object
	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Wall")) {
			Destroy (gameObject);
		}
		else if(other.CompareTag("Destructable"))
        {
			Destroy (other.gameObject);
			ReplaceWithBasicTile ();
			SoundManager.Instance.PlaySound (destructableWallDestroyedSound);
        }
		else if(other.CompareTag("Player")){
			other.GetComponent<PlayerScript>().Kill();
		}
        else if (other.CompareTag("WallPowerUp"))
        {
            Destroy(other.gameObject);
            ReplaceWithPowerUpTile();
        }
	}

	void ReplaceWithBasicTile(){
		Tile temp = Instantiate(MapGenerator.Instance.tileTypes[(int)TileType.Basic]).GetComponent<Tile>();
		temp.x = (int)gameObject.transform.position.x;
		temp.y = (int)gameObject.transform.position.y;
		temp.SetLocation();
		MapGenerator.Instance.tileMap [temp.x, temp.y] = temp;
	}
    void ReplaceWithPowerUpTile()
    {
        Tile temp = Instantiate(MapGenerator.Instance.tileTypes[(int)TileType.PowerUp]).GetComponent<Tile>();
        temp.x = (int)gameObject.transform.position.x;
        temp.y = (int)gameObject.transform.position.y;
        temp.SetLocation();
        MapGenerator.Instance.tileMap[temp.x, temp.y] = temp;
    }
}
