using UnityEngine;
using System.Collections;

// This script will read the tracking data from OptitrackRigidBodyManager.cs
// for the rigid body that corresponds to the ID defined in this script.
// Usage: Attach OptitrackRigidBody.cs to an empty Game Object
// and enter the ID number as specified in the Motive > Rigid Body Settings > Advanced > User Data field.
// Requirements:
// 1. Instance of OptitrackRigidBodyManager.cs

public class PlaySound_BaseDrum : MonoBehaviour {	
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
	public AudioSource sound_base;
	public GameObject originOverride;
	private bool isPlay = true;
	private Vector3 previous;
	void Start() {
		//optitrackTransform  = new GameObject().transform;
		sounds = GetComponents<AudioSource>();
		sound_base = sounds[0]; 

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
				//Make changes here 
				transform.position = OptitrackRigidBodyManager.instance.rigidBodyPositions[index] * 100;
				Vector3 current = transform.position;
				
				//Checking if y coordinate is less than or greater than initialY set above initially
				if(current.y < previous.y - 0.25f && isPlay == true){ // displacement [measuring if change occurs in range] = 0.02
					
					isPlay = false;
					sound_base.Play();
				}
				
				else{
					// do not play anything
					if(current.y > previous.y + 0.25){
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