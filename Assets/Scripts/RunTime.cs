using UnityEngine;
using System.Collections;

public class RunTime : MonoBehaviour {

	private LightSystem lights;
	private GameObject door;
	private Key key;
	public Material skyboxMaterial;


	public void Init(LightSystem lights, GameObject door, Key key, Material mat)
	{
		this.lights = lights;
		this.door = door;
		this.key = key;
		skyboxMaterial = mat;
		mat.SetColor("_Tint", new Color32 ((byte)128,(byte)128,(byte)128,(byte)128));
		//GetComponent<OVRPlayerController> ().GetComponentInChildren<Light> ().enabled = false;
	}


	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter(Collider collider)
	{
		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.GetComponent<Key>() == key)
		{
			/*foreach(Light l in lights.cells)
			{
				l.GetComponent<LightFlicker>().stopFlicker();
			}*/
			//Debug.Log("Collision with collection");
			door.SetActive(false);
			collider.gameObject.SetActive(false);
			lights.keyTime = 0;
			print (skyboxMaterial.GetColor("_Tint"));
			skyboxMaterial.SetColor("_Tint", new Color32((byte)44, (byte)28, (byte)53, (byte)128));
			gameObject.GetComponent<OVRPlayerController>().Acceleration = 0.3f;
			gameObject.GetComponentInChildren<Light>().enabled = true;
			gameObject.GetComponentInChildren<LightFlicker>().enabled = true;

		}
	}
}
