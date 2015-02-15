using UnityEngine;
using System.Collections;
using UnityEditor;

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
	public GameObject[,] walls;
	private GameObject[,] tiles;
	private Transform wallContainer;
	private Transform tileContainer;
	private Transform border;

	// display variables
	public bool focusOnThis=true;
	public bool displayTiles=true;
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

	void Start() {
		print("start()");

		// handle trigger variables
		if (resetMaze)
			ResetMaze();
		if (clearMaze)
			ClearMaze();
		if (generateMaze)
			GenerateMaze();
		
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
			walls = new GameObject[2*width-1, 2*height-1];
			for (int i=0; i<wallContainer.childCount; ++i) {
				// parse the wall's name to determine if it's valid
				string[] split = wallContainer.GetChild(i).gameObject.name.Split(new char[] { ' ' });
				if (split[0]=="wall") {
					int p1, p2;
					if (int.TryParse(split[1], out p1) && int.TryParse(split[2], out p2) &&
						p1<walls.GetLength(0) && p2<walls.GetLength(1)) {
						// the wall corresponds to a valid position
						walls[p1, p2] = wallContainer.GetChild(i).gameObject;
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
		}
		else if (walls.GetLength(0)!=2*width-1 || walls.GetLength(1)!=2*height-1) {
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

		// handle tiles
		if (tiles==null) {
			// handle tileContainer
			if (tileContainer==null)
				tileContainer = transform.Find("tile container");
			if (tileContainer==null) {
				tileContainer = new GameObject("tile container").transform;
				tileContainer.transform.parent = transform;
			}

			// consolidate or destroy old tiles
			tiles = new GameObject[2*width-1, 2*height-1];
			for (int i=0; i<tileContainer.childCount; ++i) {
				// parse the tile's name to determine if it's valid
				string[] split = tileContainer.GetChild(i).gameObject.name.Split(new char[] { ' ' });
				if (split[0]=="tile") {
					int p1, p2;
					if (int.TryParse(split[1], out p1) && int.TryParse(split[2], out p2) &&
						p1<tiles.GetLength(0) && p2<tiles.GetLength(1)) {
						// the tile corresponds to a valid position
						tiles[p1, p2] = tileContainer.GetChild(i).gameObject;
						tiles[p1, p2].renderer.sharedMaterial =
							((walls[p1, p2]!=null && walls[p1, p2].activeSelf)?wallMaterial:defaultDiffuse);
					} else {
						// the tile is garbage
						DestroyImmediate(tileContainer.GetChild(i).gameObject);
						--i;
					}
				}
			}

			// add tiles where necessary
			AddTiles();
		}
		else if (tiles.GetLength(0)!=2*width-1 || tiles.GetLength(1)!=2*height-1) {
			// effectively resizes tiles
			AddTiles();
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
		tileContainer.gameObject.SetActive(displayTiles);
		border.gameObject.SetActive(displayBorders);
	}

	/// <summary>
	/// Resizes tiles and adds old tiles into the new object.
	/// Old tiles are deleted if there isn't space for them.
	/// New tiles are created to fill up tiles.
	/// </summary>
	private void AddTiles() {
		GameObject[,] old = tiles;
		tiles = new GameObject[2*width-1, 2*height-1];

		// consolidate old tiles (if any)
		for (int i=0; i<old.GetLength(0) && i<tiles.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1) && j<tiles.GetLength(1); ++j)
				tiles[i, j] = old[i, j];

		// delete extra tiles (if any)
		for (int i=0; i<old.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1); ++j)
				if ((i>=tiles.GetLength(0) || j>=tiles.GetLength(1)) && old[i, j]!=null)
					DestroyImmediate(old[i, j]);

		// add tiles (if necessary)
		for (int i=0; i<tiles.GetLength(0); ++i)
			for (int j=0; j<tiles.GetLength(1); ++j)
				if (tiles[i, j]==null)
					tiles[i, j] = CreateTile(i, j);
	}

	/// <summary>
	/// Creates a new tile from the given indexes
	/// </summary>
	private GameObject CreateTile(int i, int j) {
		if (i%2==1 && j%2==1)
			return null;
		bool hasWall = (i%2==0) != (j%2==0);
		GameObject result = GameObject.CreatePrimitive(PrimitiveType.Quad);
		result.name = "tile "+i+" "+j+(hasWall?" (active)":"");
		result.transform.parent = tileContainer;
		result.transform.localEulerAngles = new Vector3(90, 0, 0);
		result.transform.localPosition = new Vector3(i+0.5f, 0, j+0.5f);
		if (hasWall)
			result.renderer.sharedMaterial = wallMaterial;
		else {
			defaultDiffuse = result.renderer.sharedMaterial;
			defaultDiffuse.color = Color.white;
		}
		return result;
	}

	/// <summary>
	/// Resizes walls and adds old walls into the new object.
	/// Old walls are deleted if there isn't space for them.
	/// New walls are created to fill up walls.
	/// </summary>
	private void AddWalls() {
		GameObject[,] old = walls;
		walls = new GameObject[2*width-1, 2*height-1];

		// consolidate old walls (if any)
		for (int i=0; i<old.GetLength(0) && i<walls.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1) && j<walls.GetLength(1); ++j)
				walls[i, j] = old[i, j];

		// delete extra walls (if any)
		for (int i=0; i<old.GetLength(0); ++i)
			for (int j=0; j<old.GetLength(1); ++j)
				if ((i>=walls.GetLength(0) || j>=walls.GetLength(1)) && old[i, j]!=null)
					DestroyImmediate(old[i, j]);

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
	private GameObject CreateWall(int i, int j) {
		GameObject result = GameObject.CreatePrimitive(PrimitiveType.Cube);
		result.name = "wall "+i+" "+j+" (active)";
		result.transform.parent = wallContainer;
		result.transform.localPosition = new Vector3(i+0.5f, 0.25f, j+0.5f);
		result.transform.localScale = (i%2==0?new Vector3(2f, 0.5f, 0.1f):new Vector3(0.1f, 0.5f, 2f));
		result.renderer.sharedMaterial = wallMaterial;
		return result;
	}

	void Update() {
		// if anything changed
		if (// if anything is null
			wallContainer==null || tileContainer==null || border==null || wallMaterial==null || walls==null || tiles==null ||
			// if a display variable changed
			wallContainer.gameObject.activeSelf!=displayWalls || tileContainer.gameObject.activeSelf!=displayTiles ||
			border.gameObject.activeSelf!=displayBorders ||
			// if dimensions changed
			2*height-1!=walls.GetLength(1) || 2*height-1!=tiles.GetLength(1) ||
			2*width-1!=walls.GetLength(0) || 2*width-1!=tiles.GetLength(0) ||
			// if material/shader variables changed
			(visibleWalls==(wallMaterial.shader==Shader.Find("Transparent/Diffuse"))) ||
			(visibleWalls && unlitShader==(wallMaterial.shader==Shader.Find("Diffuse"))) ||
			(unlitShader&&!visibleWalls) || wallMaterial.color!=wallColor ||
			// if wallHeight changed
			(wallContainer.GetChild(0)!=null && wallContainer.GetChild(0).localPosition.y!=wallHeight/2) ||
			// if trigger variables
			resetMaze || clearMaze || generateMaze)

			Start();
	}

	void OnRenderObject() {
		// check if activeGameObject is a wall or a floor that has a corresponding wall
		if (activeGameObject!=null && activeGameObject.transform.parent!=null &&
			activeGameObject.transform.parent.parent==transform)

			for (int i=0; i<tiles.GetLength(0); ++i)
				for (int j=0; j<tiles.GetLength(1); ++j)
					if (activeGameObject==walls[i, j] ||
						(activeGameObject==tiles[i, j] && walls[i, j]!=null)) {

						ToggleWall(i, j);
						activeGameObject = (focusOnThis?gameObject:null);
						return;
					}
	}

	private void ToggleWall(int i, int j) {
		// toggle wall.active, rename wall
		walls[i, j].SetActive(!walls[i, j].activeSelf);
		string[] split = walls[i, j].name.Split(new char[]{' '});
		walls[i, j].name = split[0]+" "+split[1]+" "+split[2]+(walls[i, j].activeSelf?" (active)":" (inactive)");

		// update tile material, rename tile
		tiles[i, j].renderer.sharedMaterial = (walls[i, j].activeSelf?wallMaterial:defaultDiffuse);
		split = tiles[i, j].name.Split(new char[]{' '});
		tiles[i, j].name = split[0]+" "+split[1]+" "+split[2]+(walls[i, j].activeSelf?" (active)":" (inactive)");
	}

	/// <summary>
	/// Turns off each wall.
	/// </summary>
	public void ClearMaze() {
		clearMaze = false;
		
		for (int i=0; i<walls.GetLength(0); ++i)
			for (int j=0; j<walls.GetLength(1); ++j)
				if (walls[i, j]!=null && walls[i, j].activeSelf)
					ToggleWall(i, j);
	}

	/// <summary>
	/// Turns on each wall.
	/// </summary>
	public void ResetMaze() {
		resetMaze = false;
		
		for (int i=0; i<walls.GetLength(0); ++i)
			for (int j=0; j<walls.GetLength(1); ++j)
				if (walls[i, j]!=null && !walls[i, j].activeSelf)
					ToggleWall(i, j);
	}

	/// <summary>
	/// Generates a random maze using basic depth first maze generation.
	/// </summary>
	public void GenerateMaze() {
		generateMaze = false;
		
		// initialize variables
		bool[,] visited = new bool[walls.GetLength(0), walls.GetLength(1)];
		Point2 cur = new Point2(2*Random.Range(0, width-1), 2*Random.Range(0, height-1));
		
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
		int[] order = new int[]{0, 1, 2, 3};
		for (int i=0; i<order.Length-1; ++i) {
			// swap order[i] with order[j], where j>=i&&j<length
			int j = Random.Range(i, order.Length);
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
}


/*[CustomEditor(typeof(MazeTool))]
class MazeToolEditor:Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		MazeTool mazeTool = (MazeTool)target;
		if (GUILayout.Button("Reset Maze"))
			mazeTool.ResetMaze();
		if (GUILayout.Button("Clear Maze"))
			mazeTool.ClearMaze();
		if (GUILayout.Button("Generate Maze"))
			mazeTool.GenerateMaze();
	}
}*/