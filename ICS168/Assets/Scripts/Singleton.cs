using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<Type> : MonoBehaviour where Type : MonoBehaviour {

    private static Type _instance;

    public static Type Instance {
        get {
            // If the instance is null, look to see if there is already an instance in the scene
            if (_instance == null) {
                _instance = GameObject.FindGameObjectWithTag(typeof(Type).Name) as Type;

                // If an instance could not be found, create one.
                if (_instance == null) {
                    _instance = Instantiate(Resources.Load("Managers/" + typeof(Type).Name, typeof(Type)) as Type, Vector3.zero, Quaternion.identity) as Type;
                }
            }

            return _instance;
        }
    }

    protected virtual void Awake() {
        if (_instance == null) {
            _instance = GameObject.FindGameObjectWithTag(typeof(Type).Name) as Type;
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
