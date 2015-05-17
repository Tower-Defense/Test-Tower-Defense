using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectionGrid : MonoBehaviour {
	private CanvasGroup _currentPanel;
	private Button _currentBtn;
	

	/// <summary>
	/// Select panel
	/// </summary>
	/// <param name="button">selected button</param>
	public void Select(Button button) {
		if (_currentBtn != null) {
			_currentBtn.enabled= true;
		}
		if (_currentPanel != null ) {

			_currentPanel.alpha = 0;
			_currentPanel.interactable = false;
			_currentPanel.blocksRaycasts = false;
		}
		_currentBtn = button;
		_currentPanel = button.GetComponent<SelectionButton> ().panel;
		if (_currentBtn != null) {
			_currentBtn.enabled =false;
		} 
		if (_currentPanel != null ) {
			
			_currentPanel.alpha = 1;
			_currentPanel.interactable = true;
			_currentPanel.blocksRaycasts = true;
		}
	}


	
}
