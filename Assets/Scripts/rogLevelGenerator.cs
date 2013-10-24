using UnityEngine;
using System.Collections;
using System.Linq;

public class rogLevelGenerator : MonoBehaviour {
	
	
	// Variables for GenerateGrid()
	public int gridUnit = 25; // How big is a grid space?
	public int[] gridMaxXZ = new int[2]; // What are the maximum dimensions of the grid?
	public bool[,] vGrid = new bool[50, 50]; // The virtual grid that contains the basic level layout
	public int[] curTileXZ = new int[2]; // The current tile being examined
	public int[] undoMove = new int[2]; // Saves the tile location before curTileXZ moves on, in case it moves somewhere it shouldn't
	public int roomsCount; // How many spaces have been filled?
	public int maxRooms; // What's the max spaces to fill?
	public int currentLineNum; // How long is the current path being created?
	public int lineStopNum; // How long should a path be before curTileXZ is reset back to the startXZ?
	public int[] startXZ; // Where does the level generation start?
	int pickDirNESW; // What direction does curTileXZ go next?
	int changeDir; 
	
	
	// Variables for FillGrid()
	// Tile sets organized by how many open sides they have
	public Transform[] tiles1Way = new Transform[4];
	public Transform[] tiles2Way = new Transform[6];
	public Transform[] tiles3Way = new Transform[4];
	public Transform[] tiles4Way = new Transform[1];
	
	static public bool generating;
	public int[] checkSpaceXZ = new int[2];
	public bool[] dirCheckResultNESW = new bool[4]; // Used to determine where surrounding filled grid spaces are
	public rogGridSpaceProperties gridSpaceProps;
	public int numDirs = 0;
	
	
	void Start () {
		roomsCount = 0;
		startXZ = new int[2]{gridMaxXZ[0]/2, gridMaxXZ[1]/2};
		vGrid[startXZ[0], startXZ[1]] = true;
		curTileXZ = new int[2]{startXZ[0], startXZ[1]};
		pickDirNESW = Random.Range (0,4);
		GenerateGrid();
		
		
		generating = true;
		checkSpaceXZ = new int[2]{0,0};
		FillGrid();
	}
	// GenerateGrid() creates the virtual grid the contains the basic level layout.
	void GenerateGrid() {
		while (roomsCount < maxRooms){
			// If curTileXZ == startXZ, start a new path.
			if (curTileXZ.SequenceEqual(startXZ)){
				lineStopNum = Random.Range (2, maxRooms + 1);
				currentLineNum = 0;
			}
			undoMove = new int[2]{curTileXZ[0], curTileXZ[1]}; // Save the current tile before curTileXZ moves on
			// Move curTileXZ in the direction pickDirNESW points
			if (pickDirNESW == 0)
				curTileXZ[1]++;
			if (pickDirNESW == 1)
				curTileXZ[0]++;
			if (pickDirNESW == 2)
				curTileXZ[1]--;
			if (pickDirNESW == 3)
				curTileXZ[0]--;
			// If curTileXZ moves outside of the grid, undo that move.
			if (curTileXZ[0] > gridMaxXZ[0] || curTileXZ[0] < 0 ||
				curTileXZ[1] > gridMaxXZ[1] || curTileXZ[1] < 0){
				curTileXZ = new int[2]{undoMove[0], undoMove[1]};
			} else {
				// If the new space is empty, fill it, advance the room count and check if the path needs to be reset.
				if (vGrid[curTileXZ[0], curTileXZ[1]] == false){
					vGrid[curTileXZ[0], curTileXZ[1]] = true;
					roomsCount++;
					currentLineNum++;
					if (currentLineNum == lineStopNum){
						curTileXZ = new int[2]{startXZ[0], startXZ[1]};
					}
				}
			}
			//pickDirNESW = Random.Range (0, 4);  //Simple direction picker. This one makes the generator tend to cluster the rooms.
			
		////////  This way makes the generator spread out a little more by making it harder for a path to double-back  ////////////////
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
		///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

		}
	}
	
	// FillGrid() looks at the newly filled vGrid[] and plops in prefabs that form the overall level design.
	void FillGrid(){
		while (generating == true){
			//if the space checker gets to the end of a line, return. If the return brings you beyond the Z max, end
			// This makes it so we start at the lower left corner of the grid and check every space of the grid for fills until we get to the top right.
			if (checkSpaceXZ[0] > gridMaxXZ[0]){
				checkSpaceXZ[1]++;
				if (checkSpaceXZ[1] > gridMaxXZ[1]){
					generating = false;
				}
				checkSpaceXZ[0] = 0;
			} else {
				if (vGrid[checkSpaceXZ[0], checkSpaceXZ[1]] == true){
					// CheckDirs() looks at the surrounding spaces and determines how many connections this tile has and in what direction.
					CheckDirs (checkSpaceXZ[0], checkSpaceXZ[1]);
					if (numDirs == 1){
						//You have the number of open directions and which directions those are. Now cycle through the different spawners and see which one fits
						foreach (Transform tile in tiles1Way){
							gridSpaceProps = tile.GetComponent<rogGridSpaceProperties>();
							if (gridSpaceProps.openDirNESW.SequenceEqual (dirCheckResultNESW)){
								Instantiate (tile, new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tile.rotation);
							}
						}
					}
					if (numDirs == 2){
						foreach (Transform tile in tiles2Way){
							gridSpaceProps = tile.GetComponent<rogGridSpaceProperties>();
							if (gridSpaceProps.openDirNESW.SequenceEqual (dirCheckResultNESW)){
								Instantiate (tile, new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tile.rotation);
							}
						}
					}
					if (numDirs == 3){
						foreach (Transform tile in tiles3Way){
							gridSpaceProps = tile.GetComponent<rogGridSpaceProperties>();
							if (gridSpaceProps.openDirNESW.SequenceEqual (dirCheckResultNESW)){
								Instantiate (tile, new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tile.rotation);
							}
						}
					}
					if (numDirs == 4){
						Instantiate (tiles4Way[0], new Vector3(checkSpaceXZ[0] * gridUnit, 0, checkSpaceXZ[1] * gridUnit), tiles4Way[0].rotation);
					}
				}
				checkSpaceXZ[0]++; // Advance one space down the row
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