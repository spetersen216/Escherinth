using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
	
	private Pathfinding temp;
	private Transform player;
	public Point3[] path;
	public Vector3[] positions;
	private MazeStructure mazeStruct;
	private MazeCell[,,] cells;
	public float distance=0;

	public void Init(MazeStructure mazeStruct, MazeCell[,,] cells, Transform player, Vector3 startPos) {

		this.player = player;
		this.cells = cells;
		this.mazeStruct = mazeStruct;
		gameObject.SetActive(true);
		
		//gameObject.AddComponent<CameraFacing>().cameraFacing = player.transform.Find("LeftEyeAnchor").GetComponent<Camera>();
		
		//temp  = mazeStruct.Pathfind(mazeStruct.FindKey()[0]);
		//Debug.Log(mazeStruct.FindKeySphere());
		//points = temp.PathToPoint(mazeStruct.FindKey()[0]);
		//while (temp.GetDistanceToEnd(new Point3(new Vector3(monster.transform.localPosition.x,monster.transform.localPosition.y,monster.transform.localPosition.z))) > 0) {
		
		//}
		
		//Debug.Log ("pt 1: "+points[0] + " pt2: "+temp.GetDistanceToEnd(points[0]));

		transform.position = startPos;
	}
	
	// Update is called once per frame
	void Update () {
		// find player position
		Vector3 v = mazeStruct.Vector3FromSphereToCube(player.position.normalized*mazeStruct.radius);
		//print("V: "+v);
		Point3 playerPos = mazeStruct.FromCubeToGame(v);

		// if the current path is invalid
		if (path==null || path.Length==0 || distance>path.Length /*|| path[path.Length-1]!=playerPos*/) {
			// find monster position
			v = mazeStruct.Vector3FromSphereToCube(transform.position.normalized*mazeStruct.radius);
			Point3 monsterPos = mazeStruct.FromCubeToGame(v);

			// path find around the monster, find path to player
			//print ("monster pos: "+monsterPos);
			temp = mazeStruct.Pathfind(monsterPos);
			//print ("player pos: "+playerPos);
			path = temp.PathToPoint(playerPos);

			// fill in positions
			distance = 0;
			positions = new Vector3[path.Length];
			for (int i=0; i<positions.Length; ++i) {
				Point3 p = path[i];
				v = mazeStruct.FromGameToCube(p);
				Vector3 floor = cells[p.x, p.y, p.z].GetFloor(v);
				positions[i] = mazeStruct.Vector3FromCubeToSphere(v);
				//print (path[i]+" -> "+v+" -> "+v+" -> "+positions[i]);
			}
		}

		distance += Time.deltaTime;
		int lower = (int)distance;
		int higher = lower + 1;
		float weight = (distance - lower);
		transform.position = ((1 - weight) * positions [lower] + weight * positions [higher]);
		//positions[]

	}
}
