using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

	public GameObject sparks;
	public bool playsound = true;

	void OnCollisionEnter(Collision col){

			ContactPoint pos = col.contacts[0];
			AudioSource audio = GetComponent<AudioSource> ();

		    if (playsound == true) {
			    audio.Play ();
				playsound = false;
		    }
	
			Instantiate(sparks, pos.point + new Vector3(0,50,0), Quaternion.identity);
	}

	void OnCollisionExit(Collision col){
		    playsound = true;
	}

	void OnCollisionStay(Collision col){
		   	playsound = false;
	}
}
