using UnityEngine;
using System.Collections;

public class Menu : MonoBehaviour {
	private Animator _animator;
	private CanvasGroup _canvasGroup;

	public bool isOpen {
		// "IsOpen" parametr  between states
		get {return _animator.GetBool("IsOpen");}
		set {_animator.SetBool("IsOpen", value);}
	}

	public void Awake() {
		_animator = GetComponent<Animator> ();
		_canvasGroup = GetComponent<CanvasGroup> ();

		// into center
		var rect = GetComponent<RectTransform> ();
		rect.offsetMax = rect.offsetMin = new Vector2 (0, 0);
	}

	public void Update() {
		// if animation controller in a state "Open"
		// "Open" -state(name of Animation in animator)
		// It make overlapping each other
		if (!_animator.GetCurrentAnimatorStateInfo (0).IsName ("Open")) { 
			//disable
			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = false;
		} else {

			// It make overlapping each other
			_canvasGroup.blocksRaycasts = _canvasGroup.interactable = true;
		}

	}

	public void QuitGame()
	{
		Application.Quit ();
	}
	
	public void StartLevel(int level)
	{
        GameController.GoNextLevel(level);
	}
	
	public void NewGame(int level)
	{
        StartLevel(level + GameController.MainMenuLevelId);
	}
}
