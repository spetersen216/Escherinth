using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;

[ExecuteInEditMode]
public class MazeTool:MonoBehaviour {
	// change this code to enable builds
	public GameObject activeGameObject {
		get { return Selection.activeGameObject; }
		set { Selection.activeGameObject = value; }
	}

	// maze variables
	public int width=10;
	public int height=10;
	public MazeToolWall[,] walls;
	public MazeToolCell[,] cells;
	public Transform wallContainer;
	public Transform cellContainer;
	public Transform border;

	// display variables
	public bool focusOnThis=true;
	public bool displayCells=true;
	public bool displayWalls=true;
	public bool displayBorders=true;

	// wall/material variables
	public bool visibleWalls=true;
	public bool unlitShader=false;
	public float wallHeight=0.5f;
	public Color wallColor=new Color(1.0f, 0.5f, 0.2f, 0f);
	private Material wallMaterial;
	private Material defaultDiffuse;

	// variables that trigger functions
	public bool resetMaze=false;
	public bool clearMaze=false;
	public bool generateMaze=false;
	public bool pathFind=false;
	public bool pathFindToPos=false;
	public bool resetPathFind=false;

	// string variables
	public static int version=0;
	public string toString="";
	private string _toString="";

	void Start() {
		Update();
	}

	/// <summary>
	/// Resizes cells and adds old cells into the new object.
	/// Old cells are deleted if there isn't space for them.
	/// New cells are created to fill up cells.
	/// </summary>
	private void AddCells() {
		MazeToolCell[,] old = cells;
		cells = new MazeToolCell[2*width-1, 2*height-1];

		// consolidate old cells (if any)
		for (int i=0; i<old.GetLength(0) && i<cells.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1) && j<cells.GetLength(1); ++j)
				cells[i, j] = old[i, j];

		// delete extra cells (if any)
		for (int i=0; i<old.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1); ++j)
				if ((i>=cells.GetLength(0) || j>=cells.GetLength(1)) && old[i, j]!=null)
					DestroyImmediate(old[i, j].gameObject);

		// add cells (if necessary)
		for (int i=0; i<cells.GetLength(0); ++i)
			for (int j=0; j<cells.GetLength(1); ++j)
				if (cells[i, j]==null)
					cells[i, j] = CreateCell(i, j);
	}

	/// <summary>
	/// Creates a new cell from the given indexes
	/// </summary>
	private MazeToolCell CreateCell(int i, int j) {
		if (i%2==1 && j%2==1)
			return null;
		bool hasWall = (i%2==0) != (j%2==0);
		GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);
		result.name = "cell "+i+" "+j+(hasWall?" (active)":"");
		result.transform.parent = cellContainer;
		result.transform.localEulerAngles = new Vector3(90, 0, 0);
		result.transform.localPosition = new Vector3(i+0.5f, 0, j+0.5f);
		if (hasWall)
			result.renderer.sharedMaterial = wallMaterial;
		else {
			defaultDiffuse = result.renderer.sharedMaterial;
			defaultDiffuse.color = Color.white;
		}
		return result.AddComponent<MazeToolCell>();
	}

	/// <summary>
	/// Resizes walls and adds old walls into the new object.
	/// Old walls are deleted if there isn't space for them.
	/// New walls are created to fill up walls.
	/// </summary>
	private void AddWalls() {
		MazeToolWall[,] old = walls;
		walls = new MazeToolWall[2*width-1, 2*height-1];

		// consolidate old walls (if any)
		for (int i=0; i<old.GetLength(0) && i<walls.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1) && j<walls.GetLength(1); ++j)
				walls[i, j] = old[i, j];

		// delete extra walls (if any)
		for (int i=0; i<old.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1); ++j)
				if ((i>=walls.GetLength(0) || j>=walls.GetLength(1)) && old[i, j]!=null)
					DestroyImmediate(old[i, j].gameObject);

		// create horizontal walls (if necessary)
		for (int i=1; i<walls.GetLength(0); i+=2)
			for (int j=0; j<walls.GetLength(1); j+=2)
				if (walls[i, j]==null)
					walls[i, j] = CreateWall(i, j);

		// create vertical walls (if necessary)
		for (int i=0; i<walls.GetLength(0); i+=2)
			for (int j=1; j<walls.GetLength(1); j+=2)
				if (walls[i, j]==null)
					walls[i, j] = CreateWall(i, j);
	}

	/// <summary>
	/// Creates a new wall from the given indexes
	/// </summary>
	private MazeToolWall CreateWall(int i, int j) {
		GameObject result = GameObject.CreatePrimitive(PrimitiveType.Cube);
		result.name = "wall "+i+" "+j+" (active)";
		result.transform.parent = wallContainer;
		result.transform.localPosition = new Vector3(i+0.5f, 0.25f, j+0.5f);
		result.transform.localScale = (i%2==0?new Vector3(2.1f, 0.5f, 0.1f):new Vector3(0.1f, 0.5f, 2.1f));
		result.renderer.sharedMaterial = wallMaterial;
		return result.AddComponent<MazeToolWall>();
	}

	void Update() {
		height = Mathf.Max(height, 2);
		width = Mathf.Max(width, 2);

		// handle trigger variables
		if (resetMaze)
			ResetMaze();
		if (clearMaze)
			ClearMaze();
		if (generateMaze)
			GenerateMaze();
		if (pathFind)
			PathFind();
		if (pathFindToPos)
			PathFindToPos();
		if (resetPathFind)
			ResetPathFind();

		// parse string
		if (toString!=_toString) {
			_toString = toString;

			// split string, handle first 3 splits, Update()
			string[] split = toString.Split(new char[] { ' ' });
			int version = int.Parse(split[0]);
			width = int.Parse(split[1]);
			height = int.Parse(split[2]);
			Update();

			// initialize vars
			int wallLength = walls[0, 1].ToString().Length;
			int cellLength = cells[0, 0].ToString().Length;
			int wallIndex=0;
			int cellIndex=0;

			// update all walls
			foreach (Point2 p in allWalls())
				walls[p.x, p.y].FromString(split[3].Substring(wallLength*(wallIndex++), wallLength), version);

			// update all cells
			for (int i=0; i<cells.GetLength(0); ++i) {
				for (int j=0; j<cells.GetLength(1); ++j) {
					if (walls[i, j]==null && cells[i, j]!=null)
						cells[i, j].FromString(split[4].Substring(cellLength*(cellIndex++), cellLength), version);
					else if (cells[i, j]!=null)
						cells[i, j].renderer.sharedMaterial = (walls[i, j].gameObject.activeSelf?wallMaterial:defaultDiffuse);
				}
			}
		}

		// handle wallMaterial, defaultDiffuse, visibleWalls, unlitShader
		if (wallMaterial==null)
			wallMaterial = new Material(Shader.Find("Transparent/Diffuse"));
		wallMaterial.shader = (visibleWalls?
		                       (unlitShader?Shader.Find("Sprites/Default"):Shader.Find("Diffuse")):
		                       Shader.Find("Transparent/Diffuse"));
		wallColor.a = (visibleWalls?1f:0f);
		wallMaterial.color = wallColor;
		if (!visibleWalls)
			unlitShader = false;
		GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
		defaultDiffuse = temp.renderer.sharedMaterial;
		DestroyImmediate(temp);

		// handle walls
		if (walls==null) {
			// handle wallContainer
			if (wallContainer==null)
				wallContainer = transform.Find("wall container");
			if (wallContainer==null) {
				wallContainer = new GameObject("wall container").transform;
				wallContainer.parent = transform;
			}

			// consolidate or destroy old walls
			walls = new MazeToolWall[2*width-1, 2*height-1];
			for (int i=0; i<wallContainer.childCount; ++i) {
				// parse the wall's name to determine if it's valid
				string[] split = wallContainer.GetChild(i).gameObject.name.Split(new char[] { ' ' });
				if (split[0]=="wall") {
					int p1, p2;
					if (int.TryParse(split[1], out p1) && int.TryParse(split[2], out p2) &&
					    p1<walls.GetLength(0) && p2<walls.GetLength(1)) {
						// the wall corresponds to a valid position
						walls[p1, p2] = FindComponent<MazeToolWall>(wallContainer.GetChild(i).gameObject);
						walls[p1, p2].renderer.sharedMaterial = wallMaterial;
					} else {
						// the wall is garbage
						DestroyImmediate(wallContainer.GetChild(i).gameObject);
						--i;
					}
				}
			}

			// add walls where necessary
			AddWalls();
		} else if (walls.GetLength(0)!=2*width-1 || walls.GetLength(1)!=2*height-1) {
			// effectively resizes walls
			AddWalls();
		}
		// handle wall height
		for (int i=0; i<wallContainer.childCount; ++i) {
			Transform t = wallContainer.GetChild(i);
			Vector3 pos = t.localPosition;
			t.localPosition = new Vector3(pos.x, wallHeight/2, pos.z);
			t.localScale = new Vector3(t.localScale.x, wallHeight, t.localScale.z);

			// if the wall already had the right height, assume all other walls have the appropriate height
			if (pos==t.localPosition)
				break;
		}

		// handle cells
		if (cells==null) {
			// handle cellContainer
			if (transform.Find("tile container")!=null && transform.Find("cell container")==null)
				transform.Find("tile container").gameObject.name = "cell container";
			if (cellContainer==null)
				cellContainer = transform.Find("cell container");
			if (cellContainer==null) {
				cellContainer = new GameObject("cell container").transform;
				cellContainer.transform.parent = transform;
			}

			// consolidate or destroy old cells
			cells = new MazeToolCell[2*width-1, 2*height-1];
			for (int i=0; i<cellContainer.childCount; ++i) {
				// parse the cell's name to determine if it's valid
				string[] split = cellContainer.GetChild(i).gameObject.name.Split(new char[] { ' ' });
				if (split[0]=="tile") {
					string str = cellContainer.GetChild(i).gameObject.name;
					cellContainer.GetChild(i).gameObject.name = "tile" + str.Substring(4, str.Length-4);
					split[0] = "cell";
				}

				if (split[0]=="cell" || split[0]=="cell") {
					int p1, p2;
					if (int.TryParse(split[1], out p1) && int.TryParse(split[2], out p2) &&
					    p1<cells.GetLength(0) && p2<cells.GetLength(1)) {
						// the cell corresponds to a valid position
						cells[p1, p2] = FindComponent<MazeToolCell>(cellContainer.GetChild(i).gameObject);
						cells[p1, p2].renderer.sharedMaterial =
							((walls[p1, p2]!=null && walls[p1, p2].gameObject.activeSelf)?wallMaterial:defaultDiffuse);
					} else {
						// the cell is garbage
						DestroyImmediate(cellContainer.GetChild(i).gameObject);
						--i;
					}
				}
			}

			// add cells where necessary
			AddCells();
		} else if (cells.GetLength(0)!=2*width-1 || cells.GetLength(1)!=2*height-1) {
			// effectively resizes cells
			AddCells();
		}

		// handle border
		if (border==null)
			// try to find a border
			border = transform.Find("border");
		if (border==null) {
			// create a border
			border = new GameObject("border").transform;
			border.parent = transform;

			// add border walls
			GameObject[] borderWalls = new GameObject[4];
			for (int i=0; i<4; ++i) {
				borderWalls[i] = GameObject.CreatePrimitive(PrimitiveType.Cube);
				borderWalls[i].transform.parent = border;
			}
			borderWalls[0].gameObject.name = "top";
			borderWalls[1].gameObject.name = "bottom";
			borderWalls[2].gameObject.name = "left";
			borderWalls[3].gameObject.name = "right";
		}
		for (int i=0; i<4; ++i)
			border.GetChild(i).renderer.sharedMaterial = wallMaterial;

		// resize the border
		border.Find("top").localPosition =    new Vector3(width-0.5f, wallHeight/2, 2*height-0.5f);
		border.Find("bottom").localPosition = new Vector3(width-0.5f, wallHeight/2, -0.5f);
		border.Find("left").localPosition =   new Vector3(-0.5f, wallHeight/2, height-0.5f);
		border.Find("right").localPosition =  new Vector3(2*width-0.5f, wallHeight/2, height-0.5f);
		border.Find("top").localScale =    new Vector3(2*width, wallHeight, 0.1f);
		border.Find("bottom").localScale = new Vector3(2*width, wallHeight, 0.1f);
		border.Find("left").localScale =   new Vector3(0.1f, wallHeight, 2*height);
		border.Find("right").localScale =  new Vector3(0.1f, wallHeight, 2*height);

		// handle display variables
		wallContainer.gameObject.SetActive(displayWalls);
		cellContainer.gameObject.SetActive(displayCells);
		border.gameObject.SetActive(displayBorders);


		// if anything changed
		/*if (// if anything is null
			wallContainer==null || cellContainer==null || border==null || wallMaterial==null || walls==null || cells==null ||
			// if a display variable changed
			wallContainer.gameObject.activeSelf!=displayWalls || cellContainer.gameObject.activeSelf!=displaycells ||
			border.gameObject.activeSelf!=displayBorders ||
			// if dimensions changed
			2*height-1!=walls.GetLength(1) || 2*height-1!=cells.GetLength(1) ||
			2*width-1!=walls.GetLength(0) || 2*width-1!=cells.GetLength(0) ||
			// if material/shader variables changed
			(visibleWalls==(wallMaterial.shader==Shader.Find("Transparent/Diffuse"))) ||
			(visibleWalls && unlitShader==(wallMaterial.shader==Shader.Find("Diffuse"))) ||
			(unlitShader&&!visibleWalls) || wallMaterial.color!=wallColor ||
			// if wallHeight changed
			(wallContainer.GetChild(0)!=null && wallContainer.GetChild(0).localPosition.y!=wallHeight/2) ||
			// if trigger variables
			resetMaze || clearMaze || generateMaze)

			Start();*/
		StoreString();
	}

	void OnRenderObject() {
		// check if activeGameObject is a wall or a floor that has a corresponding wall
		if (activeGameObject!=null && activeGameObject.transform.parent!=null &&
			activeGameObject.transform.parent.parent==transform) {
			for (int i=0; i<cells.GetLength(0); ++i) {
				for (int j=0; j<cells.GetLength(1); ++j) {
					if (walls[i, j]!=null && activeGameObject==cells[i, j].gameObject) {
						ToggleWall(i, j);
						activeGameObject = (focusOnThis?gameObject:null);
						return;
					}
				}
			}
		}
	}

	private void ToggleWall(int i, int j) {
		// toggle wall.active, rename wall
		GameObject wall = walls[i, j].gameObject;
		wall.SetActive(!wall.activeSelf);
		string[] split = wall.name.Split(new char[] { ' ' });
		wall.name = split[0]+" "+split[1]+" "+split[2]+(wall.activeSelf?" (active)":" (inactive)");

		// update cell material, rename cell
		GameObject cell = cells[i, j].gameObject;
		cell.renderer.sharedMaterial = (wall.activeSelf?wallMaterial:defaultDiffuse);
		split = cell.name.Split(new char[] { ' ' });
		cell.name = split[0]+" "+split[1]+" "+split[2]+(wall.activeSelf?" (active)":" (inactive)");
	}

	/// <summary>
	/// Turns off each wall.
	/// Removes all special walls/cells.
	/// </summary>
	public void ClearMaze() {
		ResetMaze();
		clearMaze = false;

		for (int i=0; i<walls.GetLength(0); ++i)
			for (int j=0; j<walls.GetLength(1); ++j)
				if (walls[i, j]!=null && walls[i, j].gameObject.activeSelf)
					ToggleWall(i, j);
	}

	/// <summary>
	/// Turns on each wall.
	/// Removes all special walls/cells.
	/// </summary>
	public void ResetMaze() {
		resetMaze = false;

		for (int i=0; i<walls.GetLength(0); ++i) {
			for (int j=0; j<walls.GetLength(1); ++j) {
				if (walls[i, j]!=null && !walls[i, j].gameObject.activeSelf)
					ToggleWall(i, j);
				if (walls[i, j]!=null)
					walls[i, j].type = MazeToolWall.WallType.normal;
				if (cells[i, j]!=null)
					cells[i, j].type = MazeToolCell.CellType.normal;
			}
		}
	}

	/// <summary>
	/// Generates a random maze using basic depth first maze generation.
	/// Removes all special walls and cells
	/// </summary>
	public void GenerateMaze() {
		generateMaze = false;

		// initialize variables
		bool[,] visited = new bool[walls.GetLength(0), walls.GetLength(1)];
		Point2 cur = new Point2(2*UnityEngine.Random.Range(0, width-1), 2*UnityEngine.Random.Range(0, height-1));

		// establish directions
		Point2[] directions = new Point2[]{
			new Point2(0, 2),
			new Point2(0, -2),
			new Point2(2, 0),
			new Point2(-2, 0)
		};

		// reset visited
		for (int i=0; i<visited.GetLength(0); ++i)
			for (int j=0; j<visited.GetLength(1); ++j)
				visited[i, j] = false;

		// reset the maze walls
		ResetMaze();

		// generate the maze
		visited[cur.x, cur.y] = true;
		GenerateMaze(cur, visited, directions);
	}

	private void GenerateMaze(Point2 cur, bool[,] visited, Point2[] directions) {
		// choose a random order to iterate over directions
		int[] order = new int[] { 0, 1, 2, 3 };
		for (int i=0; i<order.Length-1; ++i) {
			// swap order[i] with order[j], where j>=i&&j<length
			int j = UnityEngine.Random.Range(i, order.Length);
			int temp = order[i];
			order[i] = order[j];
			order[j] = temp;
		}

		// iterate over the directions, in a random order
		for (int i=0; i<directions.Length; ++i) {
			Point2 newPos = cur+directions[order[i]];
			// check if the new position is valid
			if (newPos.x>=0 && newPos.y>=0 && newPos.x<visited.GetLength(0) && newPos.y<visited.GetLength(1) &&
				!visited[newPos.x, newPos.y]) {

				// mark as visited, update the maze
				visited[newPos.x, newPos.y] = true;
				ToggleWall((newPos.x+cur.x)/2, (newPos.y+cur.y)/2);
				GenerateMaze(newPos, visited, directions);
			}
		}
	}

	public override string ToString() {
		version = 1;
		string result = version+" "+width+" "+height;

		// create char buffers
		int wallLength = walls[0, 1].ToString().Length;
		int floorLength = cells[0, 0].ToString().Length;
		char[] wallBuffer = new char[(2*width*height - width-height)*wallLength];
		char[] floorBuffer = new char[width*height*floorLength];
		int wallIndex=0;
		int floorIndex=0;

		// loop over walls
		foreach (Point2 p in allWalls()) {
			string str = walls[p.x, p.y].ToString();
			for (int i=0; i<str.Length; )
				wallBuffer[wallIndex++] = str[i++];
		}

		// loop over cells
		for (int i=0; i<cells.GetLength(0); i+=2) {
			for (int j=0; j<cells.GetLength(1); j+=2) {
				string str = cells[i, j].ToString();
				for (int k=0; k<str.Length; )
					floorBuffer[floorIndex++] = str[k++];
			}
		}

		return result +" "+ new string(wallBuffer) +" "+ new string(floorBuffer);
	}

	public IEnumerable<Point2> allWalls() {
		for (int i=0; i<walls.GetLength(0); ++i) {
			for (int j=1-(i%2); j<walls.GetLength(1); j+=2) {
				yield return new Point2(i, j);
			}
		}
	}

	private T FindComponent<T>(GameObject g) where T:Component {
		T result = g.GetComponent<T>();
		if (result==null)
			result = g.AddComponent<T>();
		return result;
	}

	private void StoreString() {
		toString = _toString = ToString();
	}

	private void PathFind() {
		pathFind = false;
		Pathfinding path = new MazeStructure(this).Pathfind(new Point3(1, 1, 1));
		for (int i=0; i<cells.GetLength(0); i+=2) {
			for (int j=0; j<cells.GetLength(1); j+=2) {
				Vector3 v = cells[i, j].transform.localPosition;
				cells[i, j].transform.localPosition = new Vector3(v.x, path.GetGamePos(new Point3(1+i/2, 1, 1+j/2)), v.z);
			}
		}
	}

	private void PathFindToPos() {
		pathFindToPos = false;
		Pathfinding path = new MazeStructure(this).Pathfind(new Point3(1, 1, 1));
		Point3[] pathToPoint = path.PathToPoint(new Point3(width, 1, height));
		print("path length: "+pathToPoint.Length);
		for (int i=0; i<pathToPoint.Length; ++i) {
			Point3 pos = MazeStructure.Point3FromGameToData(new Point3[] { pathToPoint[i] })-1;
			print("point: "+pos);
			Vector3 v = cells[pos.x, pos.z].transform.localPosition;
			cells[pos.x, pos.z].transform.localPosition = new Vector3(v.x, i, v.z);
		}
	}

	private void ResetPathFind() {
		resetPathFind = false;
		for (int i=0; i<cells.GetLength(0); i+=2) {
			for (int j=0; j<cells.GetLength(1); j+=2) {
				Vector3 v = cells[i, j].transform.localPosition;
				cells[i, j].transform.localPosition = new Vector3(v.x, 0, v.z);
			}
		}
	}
}