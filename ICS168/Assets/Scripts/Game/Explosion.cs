using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

	public float timeToExist;

	void Update(){
		timeToExist -= Time.deltaTime;
		if (timeToExist <= 0) {
			Destroy (gameObject);
		}
	}

	void OnTriggerEnter2D(Collider2D other){
		if (other.CompareTag ("Indestructable")) {
			Destroy (gameObject);
		}
		else if(other.CompareTag ("Destructable")){
			Destroy (other);
		}
	}
}
