using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {


	public Texture2D[] texture;

	IEnumerator Start() {
		AsyncOperation async = Application.LoadLevelAsync(GameController.currentLevel);
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
