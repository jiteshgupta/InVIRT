using UnityEngine;
using System.Collections;

public class PlaySound : MonoBehaviour {

	public GameObject sparks;
	public bool playsound = true;

	void OnCollisionEnter(Collision col){

			ContactPoint pos = col.contacts[0];
			AudioSource audio = GetComponent<AudioSource> ();
			Object clone = Instantiate(sparks, pos.point + new Vector3(0,40,0), Quaternion.identity);
			Destroy (clone, 1);
			if (playsound == true) {
			    audio.Play ();
				playsound = false;
		    }	
	}
	
	void OnCollisionExit(Collision col){
		    playsound = true;
	}

	void OnCollisionStay(Collision col){
		   	playsound = false;
	}

}
