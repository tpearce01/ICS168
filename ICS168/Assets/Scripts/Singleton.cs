using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<Type> : MonoBehaviour where Type : MonoBehaviour {

    //private static Type _instance;
    private static GameObject _GO;

    public static Type Instance {
        get {
            // If the instance is null, look to see if there is already an instance in the scene
            if (_GO == null) {
                //_instance = GameObject.FindGameObjectWithTag(typeof(Type).Name) as Type;
                _GO = GameObject.FindGameObjectWithTag(typeof(Type).Name);

                // If an instance could not be found, create one.
                if (_GO == null) {
                    //_instance = Instantiate(Resources.Load("Managers/" + typeof(Type).Name, typeof(Type)) as Type, Vector3.zero, Quaternion.identity) as Type;
                    //_GO = _instance.gameObject;
                    _GO = Instantiate(Resources.Load("Managers/" + typeof(Type).Name, typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity);
                }
            }

            return _GO.GetComponent<Type>();
        }
    }

    //public static GameObject GO {
    //    get {
    //        // If the instance is null, look to see if there is already an instance in the scene
    //        if (_GO == null) {
    //            _instance = GameObject.FindGameObjectWithTag(typeof(Type).Name) as Type;
    //            _GO = GameObject.FindGameObjectWithTag(typeof(Type).Name);

    //            // If an instance could not be found, create one.
    //            if (_GO == null) {
    //                _instance = Instantiate(Resources.Load("Managers/" + typeof(Type).Name, typeof(Type)) as Type, Vector3.zero, Quaternion.identity) as Type;
    //                _GO = _instance.gameObject;
    //            }
    //        }

    //        return _GO;
    //    }
    //}

    protected virtual void Awake() {

        if (_GO == null) {
            //_instance = GameObject.FindGameObjectWithTag(typeof(Type).Name) as Type;
            _GO = GameObject.FindGameObjectWithTag(typeof(Type).Name);
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
