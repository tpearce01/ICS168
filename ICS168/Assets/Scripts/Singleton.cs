using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<Type> : MonoBehaviour where Type : MonoBehaviour {

    private static GameObject _instance;

    public static GameObject Instance {
        get {
            // If the instance is null, look to see if there is already an instance in the scene
            if (_instance == null) {
                _instance = GameObject.FindGameObjectWithTag(typeof(Type).Name);

                // If an instance could not be found, create one.
                if (_instance == null) {
                    _instance = Instantiate(Resources.Load("Managers/" + typeof(Type).Name, typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity) as GameObject;
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
            _instance = GameObject.FindGameObjectWithTag(typeof(Type).Name);
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }

	/*
	protected virtual void OnAwake(){
		//To be filled by child class
	}
	*/
}
