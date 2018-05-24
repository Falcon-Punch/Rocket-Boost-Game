using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	Rigidbody rigidBody;
	AudioSource audioSource;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		Thrust();
		Rotate();
	}

	private void Thrust()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			//print("Thrusting!!");
			rigidBody.AddRelativeForce(Vector3.up);

			if (!audioSource.isPlaying)
			{
				audioSource.Play();
			}
		}
		else
		{
			audioSource.Stop();
		}
	}

	private void Rotate()
	{
		if (Input.GetKey(KeyCode.A))
		{
			//Take manual control of rotation
			rigidBody.freezeRotation = true;

			//print("Rotating Left!");
			transform.Rotate(Vector3.forward);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			//print("Rotating Right!");
			transform.Rotate(-Vector3.forward);
		}

		//resume physics control 
		rigidBody.freezeRotation = false;
	}
}
