using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.IO.Compression;

public class tImage {
	//datasize = 57600
	public byte[] data;

	public void Decompress(){

	}

    public void CompressEmpty()
    {
        Color[] pixels = new Color[tImageMetaData.Instance.imageHeight * tImageMetaData.Instance.imageWidth];
        byte[] imgData = new byte[tImageMetaData.Instance.imageHeight * tImageMetaData.Instance.imageWidth];
        for (int i = 0; i < imgData.Length; i++)
        {
            imgData[i] = (int) ColorEnum.white;
        }
    }

	public void Compress(Texture2D img){
		Color[] pixels = img.GetPixels ();
		byte[] imgData = new byte[pixels.Length];
		for (int i = 0; i < pixels.Length; i++) {
			if (pixels [i] == Color.black) {
				imgData [i] = (int)ColorEnum.black;
				//numBlack++;
			} else if (pixels [i] == Color.white) {
				imgData [i] = (int)ColorEnum.white;
				//numWhite++;
			} else if (pixels [i] == Color.red) {
				imgData [i] = (int)ColorEnum.red;
				//numRed++;
			} else if (pixels [i] == Color.blue) {
				imgData [i] = (int)ColorEnum.blue;
				//numBlue++;
			} else if (pixels [i] == Color.green) {
				imgData [i] = (int)ColorEnum.green;
				//numGreen++;
			} else if (pixels [i] == Color.yellow) {
				imgData [i] = (int)ColorEnum.yellow;
				//numYellow++;
			} else {
				/* Else: No valid color assigned. In this case we should interpolate 
				 * any irregular colors to the best fitting color, and render it as 
				 * such.
				*/
                //interpolate black/white
			    if (pixels[i].r < .5f && pixels[i].g < .5f && pixels[i].b < .5f)
			    {
			        imgData[i] = (int) ColorEnum.black;
			    }
			    else
			    {
			        imgData[i] = (int) ColorEnum.white;
			    }
			}
		}
	}

	public void CompressDelta(Texture2D previous, Texture2D current){
		Color[] prev = previous.GetPixels ();
		Color[] curr = current.GetPixels();
		byte[] delta = new byte[Mathf.Min (prev.Length, curr.Length)];

		int numBlack = 0;
		int numWhite = 0;
		int numRed = 0; 
		int numBlue = 0;
		int numGreen = 0;
		int numYellow = 0;

		//Debug.Log ("Delta size: " + delta.Length);
		for (int i = 0; i < delta.Length; i++) {
			if (prev [i] != curr [i]) {
				if (curr [i] == Color.black) {
					delta [i] = (int)ColorEnum.black;
					//numBlack++;
				} else if (curr [i] == Color.white) {
					delta [i] = (int)ColorEnum.white;
					//numWhite++;
				} else if (curr [i] == Color.red) {
					delta [i] = (int)ColorEnum.red;
					//numRed++;
				} else if (curr [i] == Color.blue) {
					delta [i] = (int)ColorEnum.blue;
					//numBlue++;
				} else if (curr [i] == Color.green) {
					delta [i] = (int)ColorEnum.green;
					//numGreen++;
				} else if (curr [i] == Color.yellow) {
					delta [i] = (int)ColorEnum.yellow;
					//numYellow++;
				} else {
                    /* Else: No valid color assigned. In this case we should interpolate 
					 * any irregular colors to the best fitting color, and render it as 
					 * such.
					*/
                    if (curr[i].r < .7f && curr[i].g < .7f && curr[i].b < .7f)
                    {
                        delta[i] = (int)ColorEnum.black;
                    }
                    else
                    {
                        delta[i] = (int)ColorEnum.white;
                    }
                }
			}
		}

		/*Debug.Log ("Black: " + numBlack);
		Debug.Log ("White: " + numWhite);
		Debug.Log ("Red: " + numRed);
		Debug.Log ("Blue: " + numBlue);
		Debug.Log ("Green: " + numGreen);
		Debug.Log ("Yellow: " + numYellow);*/

		MemoryStream ms = new MemoryStream ();
		DeflateStream ds = new DeflateStream (ms, CompressionMode.Compress);
		ds.Write (delta, 0, delta.Length);
		ds.Flush();
		ds.Close();
		data = ms.ToArray ();
		//Testing
		//Debug.Log ("Compressed delta size: " + data.Length);
		//DecompressDelta ();
		//End Testing
	}

	/*
	public void Compress(Texture2D toCompress){
		Compress (toCompress.GetPixels());
	}

	public void Compress(Color[] toCompress){
		int cmX = (int)(320f/tImageMetaData.Instance.imageWidth);
		int cmY = (int)(180f/tImageMetaData.Instance.imageHeight);

		data = new byte[cmX*cmY];

		for (int x = 0; x < cmX; x++) {
			for (int y = 0; y < cmY; y++) {
				
			}
		}
	}*/

	public byte[] DecompressDelta(){
		byte[] delta = new byte[57600];
		MemoryStream input = new MemoryStream (data);
		MemoryStream output = new MemoryStream ();
		DeflateStream ds = new DeflateStream (input, CompressionMode.Decompress);
		ds.Read (delta, 0, delta.Length);
		ds.Close ();

		/*Test if data was decompressed. Returns false on an empty byte array*/
		bool test = false;
		for (int i = 0; i < delta.Length; i++) {
			if (delta [i] != 0) {
				test = true;
			}
		}

		//Debug.Log ("Delta Data Found: " + test);

		return delta;
	}

}

public enum ColorEnum{
	black = 1,
	white = 2,
	red = 3,
	blue = 4,
	green = 5,
	yellow = 6
}
