using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {
	
	private Pathfinding temp;
	private Transform player;
	private Point3[] path;
	private Vector3[] positions;
	private MazeStructure mazeStruct;
	private float distance=0;

	public void Init(MazeStructure mazeStruct, Transform player, Vector3 startPos) {

		this.player = player;
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
		Vector3 v = MazeStructure.Vector3FromSphereToCube(player.position.normalized * mazeStruct.radius, mazeStruct.length, mazeStruct.radius);
		Point3 playerPos = MazeStructure.FromCubeToGame(v);

		// if the current path is invalid
		if (path==null || distance>path.Length /*|| path[path.Length-1]!=playerPos*/) {
			// find monster position
			v = MazeStructure.Vector3FromSphereToCube(transform.position.normalized * mazeStruct.radius, mazeStruct.length, mazeStruct.radius);
			Point3 monsterPos = MazeStructure.FromCubeToGame(v);

			// path find around the monster, find path to player
			print ("monster pos: "+monsterPos);
			temp = mazeStruct.Pathfind(monsterPos);
			print ("player pos: "+playerPos);
			path = temp.PathToPoint(playerPos);

			// fill in positions
			distance = 0;
			positions = new Vector3[path.Length];
			for (int i=0; i<positions.Length; ++i) {
				//Point3 p = MazeStructure.Point3FromDataToGame(path[i])[0];
				Point3 p = path[i];
				v = MazeStructure.FromGameToCube(p);
				Vector3 floor = v;
				if (floor.x<1)
					floor.x=0;
				else if (floor.x>mazeStruct.length)
					floor.x = mazeStruct.length;
				else if (floor.y<1)
					floor.y=0;
				else if (floor.y>mazeStruct.length)
					floor.y = mazeStruct.length;
				else if (floor.y<0)
					floor.y = 0;
				else
					floor.z=0;
				positions[i] = MazeStructure.Vector3FromCubeToSphere(v, mazeStruct.length, floor, mazeStruct.radius);
				print (path[i]+" -> "+v+" -> "+floor+" -> "+positions[i]);
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
