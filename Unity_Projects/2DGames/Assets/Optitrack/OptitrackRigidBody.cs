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

    public bool usePostionTracking = true;
    public bool useRotationTracking = false;

	public float upForce;			//upward force of the "flap"
	public float forwardSpeed;		//forward movement speed
	public bool isDead = false;		//has the player collided with a wall?
	
	Animator anim;					//reference to the animator component
	bool flap = false;				//has the player triggered a "flap"?

    public GameObject originOverride;
	private Vector2 previous;
	private Vector2 temp;
    void Start() {
		//get reference to the animator component
		anim = GetComponent<Animator> ();
		//set the bird moving forward
		GetComponent<Rigidbody2D>().velocity = new Vector2 (forwardSpeed, 0);
	}

	void Update () {
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
							temp.y = OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y;
							previous.y = temp.y; 
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
					/***************** Make Changes Here *************************/
					//don't allow control if the bird has died
					if (isDead)
						return;
					//look for input to trigger a "flap"
                    temp.y = OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y;
					if(temp.y - previous.y > 0.03){
						//Debug.Log ("Check");
						flap = true;
					}
					previous.y = temp.y;
				}
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
		}
	}

	void FixedUpdate()
	{
		//if a "flap" is triggered...
		if (flap) 
		{
			flap = false;
			
			//...tell the animator about it and then...
			anim.SetTrigger("Flap");
			//...zero out the birds current y velocity before...
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x, 0);
			//..giving the bird some upward force
			GetComponent<Rigidbody2D>().AddForce(new Vector2(0, upForce));
		}
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		//if the bird collides with something set it to dead...
		isDead = true;
		//...tell the animator about it...
		anim.SetTrigger ("Die");
		//...and tell the game control about it
		GameControlScript.current.BirdDied ();
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