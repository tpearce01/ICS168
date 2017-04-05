using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public float timeUntilExplosion;
	public int range;

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
		Destroy (gameObject);
	}

	void SpawnExplosion(int r){
		Spawner.i.SpawnObject(Prefab.Explosion, gameObject.transform.position);
		for (int i = 0; i < r; i++) {
			Spawner.i.SpawnObject(Prefab.Explosion, (Vector2)gameObject.transform.position + Vector2.up * (i+1));
			Spawner.i.SpawnObject(Prefab.Explosion, (Vector2)gameObject.transform.position + Vector2.right * (i+1));
			Spawner.i.SpawnObject(Prefab.Explosion, (Vector2)gameObject.transform.position - Vector2.up * (i+1));
			Spawner.i.SpawnObject(Prefab.Explosion, (Vector2)gameObject.transform.position - Vector2.right * (i+1));

		}
	}
}
