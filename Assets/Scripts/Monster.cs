﻿using UnityEngine;
using System.Collections;
using System;

public class Monster:MonoBehaviour {

	//private Pathfinding temp;
	private MazeStructure mazeStruct;
	private Transform player;
	public Point3[] path;
	public Vector3[] positions;
	public float distance=0;
	private float speed;
	private float height;
	private Main main;
	private bool is3D;

	// sound variables
	public AudioSource sounds;
	public AudioClip sightEstab;
	public AudioClip sightLost;

	// other variables
	private Mesh mesh;
	private bool isStarting=true;
	public bool wasInSight=false;
	public float timeSinceSight;

	public void Init(Main main, MazeStructure mazeStruct, MazeCell[, ,] cells, Transform player, Vector3 startPos, float speed, float height, bool is3D) {
		this.main = main;
		this.player = player;
		this.mazeStruct = mazeStruct;
		this.speed = speed;
		this.height = height;
		this.is3D = is3D;
		transform.position = startPos;
		sounds = player.gameObject.AddComponent<AudioSource>();
		gameObject.SetActive(true);
		mesh = GetComponent<MeshFilter>().mesh;
		print("monster start pos: "+startPos);
	}

	void Update() {
		// handle distance, isStarting
		distance += speed*Time.deltaTime;
		if (distance>2)
			isStarting = false;

		// find player and monster positions
		Vector3 playerV = mazeStruct.Vector3FromSphereToCube(is3D?player.position.normalized*mazeStruct.radius:player.position);
		Vector3 monsterV = mazeStruct.Vector3FromSphereToCube(is3D?transform.position.normalized*mazeStruct.radius:transform.position);
		Point3 playerPos = mazeStruct.FromCubeToGame(playerV);
		Point3 monsterPos = mazeStruct.FromCubeToGame(monsterV);

		// handle first path generation
		if (path==null || path.Length==0) {
			CalcPath(playerPos, monsterPos);

			// insert 2 positions in front to make the monster rise
			Vector3[] temp = new Vector3[positions.Length+2];
			Point3[] temp2 = new Point3[path.Length+2];
			temp2[0] = temp2[1] = new Point3(-1, -1, -1);
			for (int i=0; i<positions.Length; ++i) {
				temp[i+2] = positions[i];
				temp2[i+2] = path[i];
			}
			if (is3D) {
				temp[0] = temp[2].normalized*(temp[2].magnitude+2);
				temp[1] = temp[2].normalized*(temp[2].magnitude+1);
			} else {
				temp[0] = temp[2]-2*Vector3.up;
				temp[1] = temp[2]-1*Vector3.up;
			}
			positions = temp;
			path = temp2;
		}

		// if the monster and player are in the same cell
		if (((int)distance)+1>path.Length) {
			print("monster hit");
			main.Show(()=>main.LevelEndMenu(false));
			Destroy(player.gameObject);
			Destroy(gameObject);
			return;
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
			} else if (positions[0]==prev2) {
				if (positions[1]==prev1) {
					distance = Mathf.Ceil(dist)-dist;
					break;
				}
				temp[0] = prev1;
				distance = dist-Mathf.Floor(dist);
			} else
				throw new Exception("monster pathfinding error");
			positions = temp;
			break;
		}

		// move the monster
		int lower = (int)distance;
		int higher = lower + 1;
		float weight = (distance - lower);
		transform.position = ((1 - weight) * positions[lower] + weight * positions[higher]);

		// rotate the monster
		Vector3 up = (is3D?-transform.position.normalized:Vector3.up);
		//Vector3 forward = positions[higher]-positions[lower];
		Vector3 forward = player.position-transform.position;
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
		print("path length: "+path.Length);

		// fill in positions
		distance = 0;
		positions = new Vector3[path.Length];
		MazeGame temp = player.GetComponent<MazeGame>();
		for (int i=0; i<positions.Length; ++i) {
			Point3 p = path[i];
			Vector3 v = mazeStruct.FromGameToCube(p);
			positions[i] = temp.EnforceHeight(mazeStruct.Vector3FromCubeToSphere(v), height);
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
