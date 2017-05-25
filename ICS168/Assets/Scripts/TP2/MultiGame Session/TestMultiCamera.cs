using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMultiCamera : MonoBehaviour
{
    [SerializeField]
    public GameObject renderInstancePrefab;
    public List<RenderInstance> renderInstances = new List<RenderInstance>();

    public class RenderInstance
    {
        public RenderInstance()
        {
            rt = new RenderTexture(360, 180, 0);
        }
        public Camera camera;
        public RenderTexture rt;
    }

    public void Generate()
    {
        GameObject rip = Instantiate(renderInstancePrefab);
        RenderInstance ri = new RenderInstance();
        ri.camera = rip.GetComponentInChildren<Camera>();
        ri.camera.targetTexture = ri.rt;
        renderInstances.Add(ri);
        Debug.Log("Render Instance Generated.");
    }
}
