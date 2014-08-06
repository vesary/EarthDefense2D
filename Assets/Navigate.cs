using UnityEngine;
using System.Collections;

public class Navigate : MonoBehaviour {

	float maxZoom = 20f;
	float minZoom = 5f;
	float cameraDistance = 10f;
	float scrollSpeed = 0.5f;
	public float dragSpeed = 0.5f;
	private Vector3 dragOrigin;
	Camera HudCam;


	// Use this for initialization
	void Start () {
		HudCam = transform.FindChild("HUD/HUDCam").GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Mouse ScrollWheel") > 0)
		{
			ZoomOrthoCamera(this.camera, Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
			//ZoomOrthoCamera(HudCam, Camera.main.ScreenToWorldPoint(Input.mousePosition), 1);
		}
		
		// Scoll back
		if (Input.GetAxis("Mouse ScrollWheel") < 0)
		{
			ZoomOrthoCamera(this.camera, Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
			//ZoomOrthoCamera(HudCam, Camera.main.ScreenToWorldPoint(Input.mousePosition), -1);
		}

		if (Input.GetMouseButtonDown(1))
		{
			dragOrigin = Input.mousePosition;
			return;
		}
		
		if (!Input.GetMouseButton(1)) return;
		
		Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragOrigin);
		Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
		
		transform.Translate(move, Space.World);  
	}

	void ZoomOrthoCamera(Camera cam, Vector3 zoomTowards, float amount)
	{
		// Calculate how much we will have to move towards the zoomTowards position
		float multiplier = (1.0f / cam.orthographicSize * amount);
		
		// Move camera
		transform.position += (zoomTowards - transform.position) * multiplier; 
		
		// Zoom camera
		cam.orthographicSize -= amount;
		
		// Limit zoom
		cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
	}
}
