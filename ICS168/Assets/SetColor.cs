using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetColor : MonoBehaviour {
    void Start()
    {
        gameObject.GetComponent<Image>().color = Color.yellow;
    }
}
