using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTest : MonoBehaviour
{
    private int mod = 10;

    void Update()
    {
        gameObject.transform.position -= Vector3.one * Time.deltaTime*mod;
        if (gameObject.transform.position.y < -3)
        {
            mod = -10;
        }
        else if (gameObject.transform.position.y > 3)
        {
            mod = 10;
        }
    }
}
