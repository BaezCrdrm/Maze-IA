using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {
	Vector3 velocity;
	float x;
	float z;
	Rigidbody myRigidBody;
	void Start () {
		myRigidBody = GetComponent<Rigidbody>();
	}

	public void Move(Vector3 _velocity)
	{
		velocity = _velocity;
	}

    public void Move(Vector3 _velocity, GameObject _goTo)
    {
        this.gameObject.transform.LookAt(_goTo.transform.position);
        velocity = _velocity;
    }

    public void Move(float _x, float _z)
	{
		x = _x;
		z = _z;
	}

    public void FixedUpdate()
    {
        myRigidBody.MovePosition(myRigidBody.position + velocity * Time.fixedDeltaTime);
        //myRigidBody.MovePosition(myRigidBody.position + new Vector3(x,0f,z) + velocity * Time.deltaTime);
    }
}
