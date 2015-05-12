using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

	public int level;
	public Texture2D[] texture;

	IEnumerator Start() {
		AsyncOperation async = Application.LoadLevelAsync(level);
		LoadingImage ();
		yield return async;
	}

	private void LoadingImage()
	{
		int randImage = Random.Range (0, texture.Length);
		var guiTexture = GetComponent<GUITexture> ();
		guiTexture.texture = texture [randImage];

	}
}
