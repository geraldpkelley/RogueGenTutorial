using UnityEngine;
using System.Collections;

public class rogCameraScript : MonoBehaviour {
	
	public float spd;
	
	void Start () {
	
	}
	
	void Update () {
		if (Input.GetAxis ("Horizontal") != 0f){
			transform.Translate (Vector3.right * spd * Input.GetAxis ("Horizontal") * Time.deltaTime);
		}
		if (Input.GetAxis ("Vertical") != 0f){
			transform.Translate (Vector3.up * spd * Input.GetAxis ("Vertical") * Time.deltaTime);
		}
		if (Input.GetAxis ("Zoom") > 0f && transform.position.y > 10f){
			transform.Translate (Vector3.forward * spd * Input.GetAxis ("Zoom") * Time.deltaTime);
		}
		if (Input.GetAxis ("Zoom") < 0f && transform.position.y < 700f){
			transform.Translate (Vector3.forward * spd * Input.GetAxis ("Zoom") * Time.deltaTime);
		}
		if (Input.GetKeyDown (KeyCode.Escape)){
			Application.LoadLevel (Application.loadedLevelName);
		}
	}
}
