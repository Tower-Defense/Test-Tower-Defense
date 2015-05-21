using UnityEngine;
using System.Collections;

public class LoadingScreen : MonoBehaviour {

	public int level;
	public Texture2D[] texture;

	IEnumerator Start() {
        level = PlayerPrefs.GetInt("LoadLevel", 1);
        if (level >= Application.levelCount)
        {
            level = 1;
            PlayerPrefs.SetInt("LoadLevel", level);
        }
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
