using UnityEngine;
using System.Collections;

public class rogGridSpaceProperties : MonoBehaviour {
	
	// Define which directions are open for this tile in the inspector using openDirNESW.
	public bool[] openDirNESW = new bool[4];
	public bool twoWayCorner;
	public bool fourWay = false;
	
	void Start(){
		// Randomly picks which fourway tile will be displayed.
		if (fourWay == true){
			Renderer[] rends = GetComponentsInChildren<Renderer>();
			int n = Random.Range (0, 4);
			rends[n].enabled = true;
		}
	}
	
}
