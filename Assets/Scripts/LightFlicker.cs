using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {
	private Light l;
	private float r;
	public AnimationCurve brightness;
	private bool flickering = true;
	public float low_intensity = .1f;
	public float high_intensity = .4f;
	public float randomness = .2f;
	// Use this for initialization
	void Start () {
	//	flickering = true;
		l = this.gameObject.light;
		l.enabled = true;
//		Debug.Log (this.gameObject.light.ToString ());
	}

	// Update is called once per frame
	void Update () {
		if (flickering == true) {
			r = Random.Range (0f, 2f);
			if (r <= randomness) {
				float change = high_intensity;
				while (change > low_intensity) {
					change -= Time.deltaTime;
					l.intensity = brightness.Evaluate(change);
				}
			} else {
				float change = low_intensity;
				while (change < high_intensity) {
					change += Time.deltaTime;
					l.intensity = brightness.Evaluate(change);
				}
			}
		}
	}
	public void stopFlicker(){
		this.flickering = false;
		this.gameObject.light.intensity = 2f;
	}
	public void startFlicker(){
		this.flickering = true;
	}
}
