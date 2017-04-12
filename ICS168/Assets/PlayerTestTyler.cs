using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTestTyler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Move ();
		Action ();
	}

	bool ValidPos(Vector3 pos){
		Tile[,] tileMap = MapGenerator.Instance.tileMap;

		//if(tileMap[]

		return true;
	}

	void Move(){
		Vector3 pos = gameObject.transform.position;
		if (Input.GetKeyDown (KeyCode.W)) {
			pos.y += 1;
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			pos.x -= 1;
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			pos.y -= 1;
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			pos.x += 1;
		}

		if(ValidPos(pos)){
			gameObject.transform.position = pos;
		}
	}

	void Action(){
		if (Input.GetKeyDown (KeyCode.Space)) {
			Spawner.Instance.GetComponent<Spawner> ().SpawnObject (Prefab.Bomb, new Vector3(Mathf.Round(gameObject.transform.position.x), Mathf.Round(gameObject.transform.position.y), 0));
		}
	}
}
