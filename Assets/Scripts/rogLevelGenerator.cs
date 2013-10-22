using UnityEngine;
using System.Collections;
using System.Linq;

public class rogLevelGenerator : MonoBehaviour {
	
	public int gridUnit = 5;
	public int[] gridMaxXZ = new int[2];
	public bool[,] vGrid = new bool[50, 50];
	public int[] curTileXZ = new int[2];
	public int[] undoMove = new int[2];
	public int roomsCount;
	public int maxRooms;
	public int lineStopNum;
	public int[] startXZ;
	int pickDirNESW;
	int changeDir;
	bool gridGenDone = false;
	
	public Transform[] tiles1Way = new Transform[4];
	public Transform[] tiles2Way = new Transform[6];
	public Transform[] tiles3Way = new Transform[4];
	public Transform[] tiles4Way = new Transform[1];
	static public bool generating;
	Transform spawnTile;
	public int[] checkSpaceXZ = new int[2];
	public bool[] dirCheckResultNESW = new bool[4];
	public rogGridSpaceProperties gridSpaceProps;
	public int numDirs = 0;
	public int testNum = 0;
	
	
	void Start () {
		roomsCount = 0;
		//int x = Random.Range (0, gridMaxXZ[0]);
		//int y = Random.Range (0, gridMaxXZ[1]);
		startXZ = new int[2]{gridMaxXZ[0]/2, gridMaxXZ[1]/2};
		vGrid[startXZ[0], startXZ[1]] = true;
		curTileXZ = new int[2]{startXZ[0], startXZ[1]};
		pickDirNESW = Random.Range (0,4);
		GenerateGrid();
		
		
		generating = true;
		checkSpaceXZ = new int[2]{0,0};
		FillGrid();
	}
	
	void GenerateGrid() {
		while (roomsCount < maxRooms){
			testNum++;
			if (testNum > 600)
				roomsCount = maxRooms;
			Debug.Log (testNum);
			if (curTileXZ.SequenceEqual(startXZ))
				lineStopNum = Random.Range (2, maxRooms + 1);
			undoMove = new int[2]{curTileXZ[0], curTileXZ[1]};
			if (pickDirNESW == 0)
				curTileXZ[1]++;
			if (pickDirNESW == 1)
				curTileXZ[0]++;
			if (pickDirNESW == 2)
				curTileXZ[1]--;
			if (pickDirNESW == 3)
				curTileXZ[0]--;
			if (curTileXZ[0] > gridMaxXZ[0] || curTileXZ[0] < 0 ||
				curTileXZ[1] > gridMaxXZ[1] || curTileXZ[1] < 0){
				curTileXZ = new int[2]{undoMove[0], undoMove[1]};
			} else {
				if (vGrid[curTileXZ[0], curTileXZ[1]] == false){
					vGrid[curTileXZ[0], curTileXZ[1]] = true;
					roomsCount++;
					
					// The line stop makes the 
					if (roomsCount % lineStopNum == 0){
						curTileXZ = new int[2]{startXZ[0], startXZ[1]};
					}
				}
			}
			//pickDirNESW = Random.Range (0, 4);  //Simple direction picker. This one makes the generator tend to cluster the rooms.
			
		////////  This way makes the generator spread out a little more.  ////////////////
			int n = Random.Range (0, 101);
			if (n < 10)
				changeDir = 2;
			else if (n < 20)
				changeDir = 0;
			else
				changeDir = Random.Range (-1, 2);
			pickDirNESW += changeDir;
			if (pickDirNESW > 3)
				pickDirNESW = 0;
			if (pickDirNESW < 0)
				pickDirNESW = 3;
		//////////////////////////////////////////////////////////////////////////////////
			
			if (roomsCount == maxRooms)
				gridGenDone = true;
		}
	}
	
	void FillGrid(){
		while (generating == true){
			//if the space checker gets to the end of a line, return. If the return brings you beyond the Y max, end 
			if (checkSpaceXZ[0] > gridMaxXZ[0]){
				checkSpaceXZ[1]++;
				if (checkSpaceXZ[1] > gridMaxXZ[1]){
					generating = false;
				}
				checkSpaceXZ[0] = 0;
			} else {
				if (vGrid[checkSpaceXZ[0], checkSpaceXZ[1]] == true){
					CheckDirs (checkSpaceXZ[0], checkSpaceXZ[1]);
					if (numDirs == 1){
						//You have the number of open directions and which directions those are. Now cycle through the different spawners and see which one fits
						for (int i = 0; i < tiles1Way.Length; i++){
							testNum = i;
							gridSpaceProps = tiles1Way[i].GetComponent<rogGridSpaceProperties>();
							if (gridSpaceProps.openDirNESW.SequenceEqual (dirCheckResultNESW)){
								Instantiate (tiles1Way[i], new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tiles1Way[i].rotation);
								i = tiles1Way.Length;
							}
						}
					}
					if (numDirs == 2){
						for (int i = 0; i < tiles2Way.Length; i++){
							gridSpaceProps = tiles2Way[i].GetComponent<rogGridSpaceProperties>();
							if (gridSpaceProps.openDirNESW.SequenceEqual (dirCheckResultNESW)){
								Instantiate (tiles2Way[i], new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tiles2Way[i].rotation);
								i = tiles2Way.Length;
							}
						}
					}
					if (numDirs == 3){
						for (int i = 0; i < tiles3Way.Length; i++){
							gridSpaceProps = tiles3Way[i].GetComponent<rogGridSpaceProperties>();
							if (gridSpaceProps.openDirNESW.SequenceEqual (dirCheckResultNESW)){
								Instantiate (tiles3Way[i], new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tiles3Way[i].rotation);
								i = tiles3Way.Length;
							}
						}
					}
					if (numDirs == 4){
						Instantiate (tiles4Way[0], new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tiles4Way[0].rotation);
					}
				}
				checkSpaceXZ[0]++;
			}
		}
	}
	
	void CheckDirs(int x, int z){
		
		numDirs = 0;
		//Check North
		if (z < gridMaxXZ[1]){
			if (vGrid[x, z + 1] == true){
				dirCheckResultNESW[0] = true;
				numDirs++;
			} else
				dirCheckResultNESW[0] = false;
		} else
			dirCheckResultNESW[0] = false;
		
		//Check East
		if (x < gridMaxXZ[0]){
			if (vGrid[x + 1, z] == true){
				dirCheckResultNESW[1] = true;
				numDirs++;
			} else
				dirCheckResultNESW[1] = false;
		} else
				dirCheckResultNESW[1] = false;
		
		//Check South
		if (z > 0){
			if (vGrid[x, z - 1] == true){
				dirCheckResultNESW[2] = true;
				numDirs++;
			} else
				dirCheckResultNESW[2] = false;
		} else
				dirCheckResultNESW[2] = false;
		
		//Check West
		if (x > 0){
			if (vGrid[x - 1, z] == true){
				dirCheckResultNESW[3] = true;
				numDirs++;
			} else
				dirCheckResultNESW[3] = false;
		} else
				dirCheckResultNESW[3] = false;		
	}
}
