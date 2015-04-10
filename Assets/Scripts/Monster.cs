using UnityEngine;
using System.Collections;
using System;

public class Monster:MonoBehaviour {

	//private Pathfinding temp;
	private MazeStructure mazeStruct;
	private Transform player;
	public Point3[] path;
	public Vector3[] positions;
	private Vector3 startPos;
	public float distance=0;

	// sound variables
	public AudioSource sounds;
	public AudioClip sightEstab;
	public AudioClip sightLost;

	// other variables
	private Mesh mesh;
	private bool isStarting=true;
	public bool wasInSight=false;
	public float timeSinceSight;

	public void Init(MazeStructure mazeStruct, MazeCell[, ,] cells, Transform player, Vector3 startPos) {
		this.player = player;
		this.mazeStruct = mazeStruct; transform.
		 transform.position = startPos;
		this.startPos = startPos;
		sounds = player.gameObject.AddComponent<AudioSource>();
		sightEstab = Resources.LoadAssetAtPath<AudioClip>("Assets/Sounds/grudge-sound.wav");
		sightLost = Resources.LoadAssetAtPath<AudioClip>("Assets/Sounds/SuspensefulViolinPopcorn.wav");
		gameObject.SetActive(true);
		mesh = GetComponent<MeshFilter>().mesh;
	}

	void Update() {
		// handle distance, isStarting
		distance += Time.deltaTime;
		if (distance>2)
			isStarting = false;

		// find player and monster positions
		Vector3 playerV = mazeStruct.Vector3FromSphereToCube(player.position.normalized*mazeStruct.radius);
		Vector3 monsterV = mazeStruct.Vector3FromSphereToCube(transform.position.normalized*mazeStruct.radius);
		Point3 playerPos = mazeStruct.FromCubeToGame(playerV);
		Point3 monsterPos = mazeStruct.FromCubeToGame(monsterV);

		// handle first path generation
		if (path==null || path.Length==0) {
			CalcPath(playerPos, monsterPos);

			// insert 2 positions in front to make the monster rise
			Vector3[] temp = new Vector3[positions.Length+2];
			for (int i=0; i<positions.Length; ++i)
				temp[i+2] = positions[i];
			for (int i=1; i>=0; --i)
				temp[i] = temp[i+1].normalized*(temp[i+1].magnitude+2*mazeStruct.radius/mazeStruct.length);
			temp[0] = temp[2].normalized*(temp[2].magnitude+2);
			temp[1] = temp[2].normalized*(temp[2].magnitude+1);
			positions = temp;
		}

		// if the monster and player are in the same cell
		if ((int)distance+1>path.Length) {
			Application.LoadLevel(Application.loadedLevelName);
			//throw new Exception("monster hit the player");
		}

		// handle player movement
		while (path[path.Length-1]!=playerPos && !isStarting) {
			float dist = distance;
			Vector3 prev1 = positions[(int)distance];
			Vector3 prev2 = positions[(int)distance+1];
			CalcPath(playerPos, monsterPos);

			// handle each special case
			Vector3[] temp = new Vector3[positions.Length+1];
			for (int i=0; i<positions.Length; ++i)
				temp[i+1] = positions[i];
			if (positions[0]==prev1) {
				if (positions[1]==prev2) {
					distance = dist-Mathf.Floor(dist);
					break;
				}
				temp[0] = prev2;
				distance = Mathf.Ceil(dist)-dist;
				//Debug.Log("start at prev2");
			} else if (positions[0]==prev2) {
				if (positions[1]==prev1) {
					distance = Mathf.Ceil(dist)-dist;
					break;
				}
				temp[0] = prev1;
				distance = dist-Mathf.Floor(dist);
				//Debug.Log("start at prev1");
			} else{
				throw new Exception("monster pathfinding error");
			}
			positions = temp;
			break;
		
		}

		// move the monster
		int lower = (int)distance;
		int higher = lower + 1;
		float weight = (distance - lower);
		transform.position = ((1 - weight) * positions[lower] + weight * positions[higher]);

		// rotate the monster
		Vector3 up = -transform.position.normalized;
		Vector3 forward = positions[higher]-positions[lower];
		transform.rotation = Quaternion.LookRotation(forward, up);

		// handle sight of the player
		bool isInSight = IsInSight();
		float newTimeSinceSight = (isInSight?0:timeSinceSight+Time.deltaTime);
		// handle sight being established
		if (isInSight && !wasInSight) {
			if (sounds.isPlaying)
				sounds.Stop();
			sounds.clip = sightEstab;
			sounds.Play();
			print("sight established");
		}
			// handle sight being lsot
		else if (sounds.isPlaying && sounds.clip==sightEstab && !isInSight) {
			sounds.Stop();
			print("sight lost");
		}
		// handle sight lost for too long
		if (newTimeSinceSight>10 && timeSinceSight<10) {
			sounds.clip = sightLost;
			sounds.Play();
			print("sight lost for 10 seconds");
		}
		// store sight vars
		wasInSight = isInSight;
		timeSinceSight = newTimeSinceSight;
	}

	/// <summary>
	/// Uses pathfinding to generate a path between the 2 given points in game-space.
	/// </summary>
	private void CalcPath(Point3 playerPos, Point3 monsterPos) {
		// path find around the monster, find path to player
		print("playerPos: "+playerPos);
		print("monsterPos: "+monsterPos);
		Pathfinding pathfinding = mazeStruct.Pathfind(monsterPos);
		path = pathfinding.PathToPoint(playerPos);

		// fill in positions
		distance = 0;
		//Debug.Log("path: "+path);
		//Debug.Log("path length: "+path.Length);
		positions = new Vector3[path.Length];
		for (int i=0; i<positions.Length; ++i) {
			Point3 p = path[i];
			Vector3 v = mazeStruct.FromGameToCube(p);
			positions[i] = mazeStruct.Vector3FromCubeToSphere(v);
			//print(path[i]+" -> "+v+" -> "+v+" -> "+positions[i]);
		}
	}

	/// <summary>
	/// Returns true if any vertex in the mesh is in sight of the player
	/// </summary>
	private bool IsInSight() {
		// calculate verts in world space
		Vector3[] verts = mesh.vertices;
		for (int i=0; i<verts.Length; ++i)
			verts[i] = transform.TransformPoint(verts[i]);

		// do ray casting
		for (int i=0; i<verts.Length; ++i) {
			Ray r = new Ray(verts[i], player.transform.position-verts[i]);
			RaycastHit target;
			if (Physics.Raycast(r, out target))
				if (target.transform==player)
					return true;

		}

		return false;
	}
}
