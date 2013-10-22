using UnityEngine;
using System.Collections;

public class rogGridSpaceProperties : MonoBehaviour {

	public bool[] openDirNESW = new bool[4];
	public bool twoWayCorner;
	public bool fourWay = false;
	
	void Start(){
		if (fourWay == true){
			Renderer[] rends = GetComponentsInChildren<Renderer>();
			int n = Random.Range (0, 4);
			rends[n].enabled = true;
		}
	}
	
}
