using UnityEngine;
using System.Collections;

// This script will read the tracking data from OptitrackRigidBodyManager.cs
// for the rigid body that corresponds to the ID defined in this script.
// Usage: Attach OptitrackRigidBody.cs to an empty Game Object
// and enter the ID number as specified in the Motive > Rigid Body Settings > Advanced > User Data field.
// Requirements:
// 1. Instance of OptitrackRigidBodyManager.cs

public class OptitrackRigidBody : MonoBehaviour {	
	public int ID;
	
	private bool foundIndex = false;
	[HideInInspector]
	public int index;
	public float initialX;
	public float initialY;
	public float initialZ;
	public bool usePostionTracking = true;
	public bool useRotationTracking = false;

	public AudioSource[] sounds;
	public AudioSource sound_ft;
	public AudioSource sound_lt;
	public AudioSource sound_rt;
	public AudioSource sound_sd;
	public AudioSource sound_ch;
	public AudioSource sound_hh;
	public GameObject originOverride;

	public GameObject[] effects;
	public GameObject effects_ft;
	public GameObject effects_lt;
	public GameObject effects_rt;
	public GameObject effects_sd;
	public GameObject effects_ch;
	public GameObject effects_hh;
	public bool isPlayEffects = true;
	private bool isPlay = true;
	private Vector3 previous;
	
	
	void Start() {
		//optitrackTransform  = new GameObject().transform;
		sounds = GetComponents<AudioSource>();
		sound_ft = sounds[0]; 
		sound_sd = sounds[1];
		sound_rt = sounds[2];
		sound_lt = sounds[3];
		sound_ch = sounds[4];
		sound_hh = sounds[5];
	}
	
	void FixedUpdate() {
		//If we have received a packet from Motive then look for the rigid body ID index
		
		if(foundIndex == false) 
		{
			if(OptitrackRigidBodyManager.instance.receivedFirstRigidBodyPacket) 
			{
				if(foundIndex == false) 
				{
					for(int i = 0; i < OptitrackRigidBodyManager.instance.rigidBodyIDs.Length; i++) 
					{
						//Looking for ID in array of rigid body IDs
						if(OptitrackRigidBodyManager.instance.rigidBodyIDs[i] == ID) 
						{
							index = i; //Found ID
							//Setting the height of drums according to drumemr	
							//initialY = OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y;
							//previous.y = initialY;
						}
					}
					foundIndex = true;
				}
			}
		}
		else {
			if (usePostionTracking)
				if (originOverride != null)
			{
				transform.position = (OptitrackRigidBodyManager.instance.rigidBodyPositions[index] - OptitrackRigidBodyManager.instance.origin.position) + originOverride.transform.position;
			}
			else{
				/***************** Make changes here *******************************/ 
				transform.position = OptitrackRigidBodyManager.instance.rigidBodyPositions[index] * 100;
				Vector3 current = transform.position;
				
				//Checking if y coordinate is less than or greater than initialY set above initially
				if(current.y < previous.y - 1.8f && isPlay == true){ // displacement [measuring if change occurs in range] = 0.02
					
					isPlay = false;
					
					if(current.x > 5){ // Hi-Hat, Snare Drum, Left Tom  ----- Separating Margin = 0
						
						if(current.x < 30){ // Left Tom, Snare Drum
							
							//Snare Drum
							if(current.z > -45){
								sound_sd.Play();
								if(isPlayEffects == true){
									Object clone = Instantiate(effects_sd, GameObject.Find("Snare").transform.position + new Vector3(0,60,0), Quaternion.identity);
									Destroy (clone, 1);
								}
							}

							//Left Tom
							else{ 
								sound_rt.Play();
								if(isPlayEffects == true){
									Object clone = Instantiate(effects_lt, GameObject.Find("Tom_left").transform.position + new Vector3(0,80,0), Quaternion.identity);
									Destroy (clone, 1);
								}
							}
						}
						
						else{
							
							//Hi-Hat
							//if(current.z > -45){
								sound_hh.Play();
								if(isPlayEffects == true){
									Object clone = Instantiate(effects_hh, GameObject.Find("HiHat").transform.position + new Vector3(0,90,0), Quaternion.identity);
									Destroy (clone, 1);
								}
							//}
						}
						
					}
					
					else{ 	// Chisel, Right Tom, Floor Tom
						
						if(current.z > -40){  // Floor
							
							//Floor Tom
							sound_ft.Play();
							if(isPlayEffects == true){
								Object clone = Instantiate(effects_ft, GameObject.Find("Floor_tom").transform.position + new Vector3(0,60,0), Quaternion.identity);
								Destroy (clone, 1);
							}
							
						}
						
						else{ //Right Tom, Chisel
							
							//Right Tom
							if(current.x > -25){
								sound_lt.Play();
								if(isPlayEffects == true){
									Object clone = Instantiate(effects_rt, GameObject.Find("Tom_right").transform.position + new Vector3(0,80,0), Quaternion.identity);
									Destroy (clone, 1);
								}
							}

							//Chisel
							else{
								sound_ch.Play();
								if(isPlayEffects == true){
									Object clone = Instantiate(effects_ch, GameObject.Find("Ride").transform.position + new Vector3(0,110,0), Quaternion.identity);
									Destroy (clone, 1);
								}
							}
						}	
						
					}
				}
				
				
				else{
					// do not play anything
					if(current.y > previous.y + 1.8f){
						isPlay = true;
					}
				}
				
				previous.y = current.y;
			}
			
			/*
			 * As of now we dont need rotation
            if (useRotationTracking)
                if (originOverride != null)
                {
                    //Subtract the origin rotation used by OptitrackRigidBodyManager
                    transform.rotation = Quaternion.Inverse(OptitrackRigidBodyManager.instance.origin.rotation) * OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
                    //Add the originOverride rotation applied to this rigid body
                    transform.rotation = originOverride.transform.rotation * transform.rotation;
                }
                else
                    transform.rotation = OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
           */         
		}	
	}
	
	public Vector3 GetPostion() {
		return OptitrackRigidBodyManager.instance.rigidBodyPositions[index];
	}
	
	public Quaternion GetRotationQuaternion() {
		if (foundIndex)
			return OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
		else
			return Quaternion.identity;
	}
	
	public Vector3 GetRotationEuler() {
		if (foundIndex)
			return OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index].eulerAngles;
		else
			return Vector3.zero;
	}
	
	/*
    public Transform GetTransform() {
        optitrackTransform.position = OptitrackRigidBodyManager.instance.rigidBodyPositions[index];
        optitrackTransform.rotation = OptitrackRigidBodyManager.instance.rigidBodyQuaternions[index];
        return optitrackTransform;
    }*/
}