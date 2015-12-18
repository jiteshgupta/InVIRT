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
	public float initialX;
	public float initialY;
	public float initialZ;
	private bool foundIndex = false;
	[HideInInspector]
    public int index;

    public bool usePostionTracking = true;
    public bool useRotationTracking = false;
	public Rigidbody rb;
    public GameObject originOverride;

	private float maxVelocity = 300.0f;
	private float sqrMaxVelocity;
	private Vector3 previous;
	private Vector3 temp ;
	private bool collision_done = true;
	private float value_y = 0.0f;

	void Start() {
		rb = GetComponent<Rigidbody>();
		//SetMaxVelocity(maxVelocity);
		//InvokeRepeating ("ModifiedFixedUpdate", 0, 0.01F);
    }

	void SetMaxVelocity(float maxVelocity){
		this.maxVelocity = maxVelocity;
		sqrMaxVelocity = maxVelocity * maxVelocity;
	}

	void FixedUpdate () {
		//If we have received a packet from Motive then look for the rigid body ID index
		//Debug.Log (Time.deltaTime);

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
							temp = transform.position;
							//previous = (OptitrackRigidBodyManager.instance.rigidBodyPositions[index])*5000.0f;
							/*
							temp.x = initialX + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].x)*1.0f;
							temp.y = initialY + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y)*1.0f;
							temp.z = initialZ + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].z)*1.0f;
							previous = temp;
							max_speed = 30;
							*/
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
					/****************** Position Tracking Code *************************/
			
				    if(collision_done == true){
					    //change positions
						temp.x = initialX + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].x)*240.0f;
						temp.y = initialY + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y)*200.0f;
						temp.z = initialZ + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].z)*240.0f;
						transform.position = temp;
					}

					else{
							//check if ball moved up
							if((initialY + (OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y)*200.0f) > value_y){
								collision_done = true;
							}
							else{
								//dont change positions
								transform.position = temp;	
							}
								
					}

					/*
				 	*************** Velocity Tracking Code*******************************
					temp = (OptitrackRigidBodyManager.instance.rigidBodyPositions[index])*5000.0f;
					var v = (temp-previous);
					if((temp-previous).magnitude > 0.05){
						Debug.Log((temp-previous).magnitude);
						if(v.sqrMagnitude > sqrMaxVelocity){
							Debug.Log("Max Limit Reached");
							rb.velocity = v.normalized * maxVelocity;
						}else{
							rb.velocity = v;
						}
					}
					else{
						Debug.Log("Zero Velocity Given");
						rb.velocity = new Vector3(0, 0, 0);
					}
					previous = temp;
					*/

			}
		}
	}

	void OnCollisionEnter(Collision col){
		//when ball hits surface - dont let it go in - dont change position
		//store the value of y
		value_y = temp.y;
		collision_done = false;	
	}

	/*
	void OnCollisionExit(Collision col){
		Debug.Log ("Exit");
		tracking = true;
	}


	void OnCollisionStay(Collision col){
		Debug.Log ("Stay");
	}
	*/

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
}