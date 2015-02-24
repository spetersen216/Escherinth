using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CollectionMechanic : MonoBehaviour {
	
	// Initializes two Integer variables that keep track of our collectable counts
	private int collectableCountTotal = 0;
	private int collectableCountCurrent = 0;
	
	// Creates a variable reference to store the connection to the HUD Text object
	private Text CollectionText;
	
	// The Start function is called once when the level loads, you'll want to perform all your setup actions here
	void Start () {
		
		// Uses the handy "Find" and "Get Component" functions to grab a reference to the CollectionText game object and store it in our Collection Text variable
		//CollectionText = GameObject.Find("CollectionText").GetComponent<Text>();
		
		// Grab an array of all the game objects with the tag "collectable"
		GameObject[] collectables = GameObject.FindGameObjectsWithTag("collectable");
		
		// Iterate through each element in the array, each time increasing the total ammount of items to be collected 
		foreach (GameObject g in collectables){
			collectableCountTotal++;
			//Debug.Log(""+g.ToString());
		}    
		
		// Set the HUD to display the current amount of items collected out of the total
		//CollectionText.text = "" + collectableCountCurrent + "/" + collectableCountTotal + " ITEMS COLLECTED";
	}
	
	// The OnTriggerEnter function is called when the collider attached to this game object (whatever object the script is attached to) overlaps another collider set to be a "trigger"
	void OnTriggerEnter (Collider collider)
	{
		// We want to check if the thing we're colliding with is a collectable, this will differentiate it from other trigger objects which we might add in the future
		if (collider.tag == "collectable")
		{
			Debug.Log("Collision with collection");
			// We want to increase our current collectable count by 1
			collectableCountCurrent++;
			// We also want to destroy the other actor, making it disapear
			Destroy(collider.gameObject);
			
			// If we have collected all the objects on the level show the win text
			if (collectableCountCurrent == collectableCountTotal){
				//CollectionText.text = "ALL ITEMS COLLECTED!";
			}
			
			// Otherwise update the HUD with the new collection total
			else{
				//CollectionText.text = "" + collectableCountCurrent + "/" + collectableCountTotal + " ITEMS COLLECTED";
			}
		}
	}
}