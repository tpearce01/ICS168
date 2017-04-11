using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : Singleton<Spawner> {

	//public static Spawner i;								    //Static reference

	public GameObject[] prefabs;								//List of all prefabs that may be instantiated
	GameObject activeObject;

	/*
	void Awake(){
		i = this;
	}
	*/

	//Instantiate an object at the specified location and add it to the list of active objects
	/// <summary>
	/// Instantiate an object at the specified location
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="index">Index.</param>
	/// <param name="location">Location.</param>
	public GameObject SpawnObject(int index, Vector3 location){
		activeObject = Instantiate (prefabs [index], location, Quaternion.identity) as GameObject;
		return activeObject;
	}
	/// <summary>
	/// Instantiate an object at the specified location
	/// </summary>
	/// <returns>The object.</returns>
	/// <param name="obj">Object.</param>
	/// <param name="location">Location.</param>
	public GameObject SpawnObject(Prefab obj, Vector3 location){
		return SpawnObject((int)obj, location);
	}

    //Instantiate an object at position with rotation
	/// <summary>
	/// Spawns the object with rotation.
	/// </summary>
	/// <returns>The object with rotation.</returns>
	/// <param name="index">Index.</param>
	/// <param name="location">Location.</param>
	/// <param name="rotation">Rotation.</param>
	public GameObject SpawnObjectWithRotation(int index, Vector3 location, Vector3 rotation)
    {
		activeObject = Instantiate(prefabs[index], location, Quaternion.identity) as GameObject;
        activeObject.transform.Rotate(rotation);
		return activeObject;
    }
	/// <summary>
	/// Spawns the object with rotation.
	/// </summary>
	/// <returns>The object with rotation.</returns>
	/// <param name="obj">Object.</param>
	/// <param name="location">Location.</param>
	/// <param name="rotation">Rotation.</param>
	public GameObject SpawnObjectWithRotation(Prefab obj, Vector3 location, Vector3 rotation)
    {
        return SpawnObjectWithRotation((int)obj, location, rotation);
    }
}
	
//Enum to easily convert prefab names to the appropriate index
/// <summary>
/// Enum to easily convert prefab names to the appropriate index
/// </summary>
[System.Serializable]
public enum Prefab{
	Explosion = 0,
	Bomb = 1
};
