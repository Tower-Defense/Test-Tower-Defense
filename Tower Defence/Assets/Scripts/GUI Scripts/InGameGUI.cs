using UnityEngine;
using System.Collections;

public class InGameGUI : MonoBehaviour {

	//NGUI items
	public bool buildPanelOpen = false;
	public TweenPosition buildPanelTweener;
	public TweenRotation buildPanelArrowTweener;
	//
	
	//Placement Plane items
	public Transform placementPlanesRoot;
	public Material hoverMat;
	public LayerMask placementLayerMask;
	private Material originalMat;
	private GameObject lastHitObj;
	//
	
	//build selection items
	public Color onColor;
	public Color offColor;
	/// <summary>
	/// All structures that we can build.
	/// </summary>
	public GameObject[] allStructures;
	public UISprite[] buildBtnGraphics;
	private int structureIndex = 0;
	//
	
	// Use this for initialization
	void Start () {
		structureIndex = 0;
		UpdateGUI ();
	}
	
	// Update is called once per frame
	void Update () {
		if (buildPanelOpen) {
			var ray  = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 1000, placementLayerMask)) {
				if(lastHitObj) {
					(lastHitObj.GetComponent<MeshRenderer>()).material = originalMat;
				}

				lastHitObj = hit.collider.gameObject;
				originalMat = (lastHitObj.GetComponent<MeshRenderer>()).material;
				(lastHitObj.GetComponent<MeshRenderer>()).material = hoverMat;
			}
			else {
				if(lastHitObj) {
					(lastHitObj.GetComponent<MeshRenderer>()).material = originalMat;
					lastHitObj = null;
				}
			}

			if(Input.GetMouseButtonDown(0) && lastHitObj) { // 0 - left button
				if(lastHitObj.tag == "PlacementPlane_Open") {
					GameObject newStructure = Instantiate(allStructures[structureIndex], lastHitObj.transform.position, Quaternion.identity) as GameObject;
					newStructure.transform.localEulerAngles = new Vector3(
								newStructure.transform.localEulerAngles.x,
								Random.Range(0,360),
								newStructure.transform.localEulerAngles.z);
					lastHitObj.tag = "PlacementPlane_Taken";
				}
			}
		}
	}

	public void SetBuildChoice(GameObject btnObj) {
		string btnName = btnObj.name;

		switch (btnName) {
		case "Btn_Cannon":
			structureIndex = 0;
			break;
		case "Btn_Missile":
			structureIndex = 1;
			break;
		}

		UpdateGUI ();
	}

	private void UpdateGUI () {
		foreach(var theBtnGraphic in buildBtnGraphics)
		{
			theBtnGraphic.color = offColor;
		}

		buildBtnGraphics [structureIndex].color = onColor;
	}

	public void ToogleBuildPanel() {
		if (buildPanelOpen) {
			foreach(Transform thePlane in placementPlanesRoot)
			{
				MeshRenderer t = 
					thePlane.gameObject.GetComponent<MeshRenderer>();
				t.enabled=false;
			}


			buildPanelTweener.Play (false);
			buildPanelArrowTweener.Play (false);
			buildPanelOpen = false;
		} else {
			foreach(Transform thePlane in placementPlanesRoot)
			{
				MeshRenderer t = 
					thePlane.gameObject.GetComponent<MeshRenderer>();
				t.enabled=true;
			}
			buildPanelTweener.Play (true);
			buildPanelArrowTweener.Play (true);
			buildPanelOpen = true;
		}
	}
}
