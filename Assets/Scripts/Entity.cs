using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {
	public List<Node> Visited;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void PadsMovement()
    {
        //float x = 0, z = 0;
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //    x = movementDistance;
        //else if (Input.GetKeyDown(KeyCode.LeftArrow))
        //    x = -movementDistance;
        //if (Input.GetKeyDown(KeyCode.UpArrow))
        //    z = movementDistance;
        //else if (Input.GetKeyDown(KeyCode.DownArrow))
        //    z = -movementDistance;

        //Vector3 moveInput = new Vector3(x, 0f, z);
        //Move(moveInput);
    }
}
