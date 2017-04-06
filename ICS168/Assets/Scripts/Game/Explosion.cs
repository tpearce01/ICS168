using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float timeToExist;

    void Awake()
    {
        //Check if explosion is out of bounds
        int x = (int)gameObject.transform.position.x;
        int y = (int)gameObject.transform.position.y;

        if (x < 0 || x >= MapGenerator.i.tileMap.GetLength(0) || y < 0 || y >= MapGenerator.i.tileMap.GetLength(1))
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
		else if(other.CompareTag ("Destructable") || other.CompareTag("Player")){
			Destroy (other.gameObject);
            if (other.CompareTag("Destructable"))
            {
                Tile temp = Instantiate(MapGenerator.i.tileTypes[(int)TileType.Basic]).GetComponent<Tile>();
                temp.x = (int)gameObject.transform.position.x;
                temp.y = (int)gameObject.transform.position.y;
                temp.SetLocation();
            }
        }
	}
}
