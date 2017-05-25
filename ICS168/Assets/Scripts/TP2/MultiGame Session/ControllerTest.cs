using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControllerTest : MonoBehaviour {
    public List<Image> images = new List<Image>();
    public TestMultiCamera tmc;

    public void AddRenderInstance()
    {
        Debug.Log("Adding Render Instance...");
        tmc.Generate();
        //images[index].mainTexture = ;
        //tmc.renderInstances[tmc.renderInstances.Count-1]
    }

    public void RemoveRenderInstance()
    {
        Debug.Log("Removing Render Instance...");
    }

    public void Update()
    {
        StartCoroutine(Render());
    }

    public IEnumerator Render()
    {
        yield return new WaitForEndOfFrame();
        int i = 0;
        foreach (TestMultiCamera.RenderInstance ri in tmc.renderInstances)
        {
            Texture2D tex = new Texture2D(ri.camera.targetTexture.width, ri.camera.targetTexture.height,
                TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, ri.camera.targetTexture.width, ri.camera.targetTexture.height), 0, 0);
            tex.Apply();

            images[i].GetComponent<CanvasRenderer>().SetTexture(tex);

            i++;
        }
    }
}
