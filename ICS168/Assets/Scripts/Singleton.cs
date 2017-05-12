using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<Type> : MonoBehaviour where Type : MonoBehaviour {

    //private static Type _instance;
    private static GameObject _GO;

    public static Type Instance {
        get {
            if (_GO == null) {
                _GO = GameObject.FindGameObjectWithTag(typeof(Type).Name);

                // If an instance could not be found, create one.
                if (_GO == null) {
                    _GO = Instantiate(Resources.Load("Managers/" + typeof(Type).Name, typeof(GameObject)) as GameObject, Vector3.zero, Quaternion.identity);
                }
            }

            return _GO.GetComponent<Type>();
        }
    }

    protected virtual void Awake() {

        if (_GO == null) {
            _GO = GameObject.FindGameObjectWithTag(typeof(Type).Name);
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
}
