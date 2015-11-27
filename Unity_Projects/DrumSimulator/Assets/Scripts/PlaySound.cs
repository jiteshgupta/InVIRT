using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

	//public GameObject sparks;

	//Volume:	 0..1
	//Magnitude: 
	void OnCollisionEnter(Collision col){

		ContactPoint pos = col.contacts[0];

		//Collider c = col.collider;

		AudioSource audio = GetComponent<AudioSource>();
		audio.Play();
		//Instantiate (sparks, pos.point, Quaternion.identity);
	}
}
