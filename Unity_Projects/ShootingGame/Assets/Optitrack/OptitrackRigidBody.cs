using UnityEngine;
using System.Collections;

[System.Serializable]
public class Done_Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

public class OptitrackRigidBody : MonoBehaviour {	
	public int ID;

	private bool foundIndex = false;
	[HideInInspector]
    public int index;

    public bool usePostionTracking = true;
    public bool useRotationTracking = false;

    public GameObject originOverride;

	public float speed;
	public float tilt;
	public Done_Boundary boundary;
	
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	
	private float nextFire;
	private Vector2 temp;
	private Vector2 previous;
    void Start() {
        //optitrackTransform  = new GameObject().transform;
		temp = previous;
    }
	void Update ()
	{	
		temp.y = OptitrackRigidBodyManager.instance.rigidBodyPositions[index].y;
		if (temp.y-previous.y < -0.01f && Time.time > nextFire) 
		{	
			Debug.Log ("Check");
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play ();
		}
		previous.y = temp.y;
	}
	void FixedUpdate () {
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
					float moveHorizontal = -(OptitrackRigidBodyManager.instance.rigidBodyPositions[index].x);
					//float moveVertical = -(OptitrackRigidBodyManager.instance.rigidBodyPositions[index].z)*12.0f - 4.0f;
					//if(moveVertical < -0.4f){
					//	moveVertical = -0.4f;
					//}				
					Vector3 movement = new Vector3 (moveHorizontal, 0.0f, 0.0f);
					GetComponent<Rigidbody>().velocity = movement * speed;
				
					GetComponent<Rigidbody>().position = new Vector3
						(
						Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
						0.0f, 
						Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
						);
				
					GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
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