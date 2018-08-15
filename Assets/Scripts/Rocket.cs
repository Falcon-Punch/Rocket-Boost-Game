using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;
	[SerializeField] float levelLoadDelay = 1f;
	[SerializeField] int gameOverIndex = 8;

	[SerializeField] AudioClip mainEngine;
	[SerializeField] AudioClip success;
	[SerializeField] AudioClip death;

	[SerializeField] ParticleSystem mainEngineParticles;
	[SerializeField] ParticleSystem successParticles;
	[SerializeField] ParticleSystem deathParticles;

	Rigidbody rigidBody;
	AudioSource audioSource;

	enum State { Alive, Dying, Transcending }
	State state = State.Alive;
	
	private static int currentScenceIndex;

	public int CurrentScenceIndex
	{
		get
		{
			return currentScenceIndex;
		}
		set
		{
			currentScenceIndex = value;
		}
	}

	// Use this for initialization
	void Start ()
	{
		rigidBody = GetComponent<Rigidbody>();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (state == State.Alive)
		{
			RespondToThrustInput();
			RespondToRotateInput();
		}

		if (Debug.isDebugBuild)
		{
			RespondToDebugKeys();
		}

		if (state != State.Alive) { return; }
		currentScenceIndex = SceneManager.GetActiveScene().buildIndex;
	}

	private void RespondToDebugKeys()
	{
		if (Input.GetKeyDown(KeyCode.L))
		{
			LoadNextLevel();
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		if (state != State.Alive) { return; }

		switch (collision.gameObject.tag)
		{
			case "Friendly":
				// Do Nothing
				break;
			case "Finish":
				FinishSequence();
				break;
			default:
				DeathSequence();
				break;
		}
	}

	private void FinishSequence()
	{
		state = State.Transcending;
		audioSource.Stop();
		audioSource.PlayOneShot(success);
		successParticles.Play();
		// You Passed!
		Invoke("LoadNextLevel", levelLoadDelay);
	}

	private void DeathSequence()
	{
		state = State.Dying;
		audioSource.Stop();
		audioSource.PlayOneShot(death);
		deathParticles.Play();
		// You Died!
		Invoke("LoadGameOver", levelLoadDelay);
	}

	public void ReloadLevel()
	{
		SceneManager.LoadScene(currentScenceIndex);
	}

	public void LoadGameOver()
	{
		SceneManager.LoadScene(gameOverIndex);
	}

	public void LoadNextLevel()
	{
		currentScenceIndex = SceneManager.GetActiveScene().buildIndex;
		int nextSceneIndex = currentScenceIndex + 1;

		if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
		{
			nextSceneIndex = 0;
		}

		SceneManager.LoadScene(nextSceneIndex);
	}
	
	private void RespondToThrustInput()
	{
		if (Input.GetKey(KeyCode.Space))
		{
			ApplyThrust();
		}
		else
		{
			audioSource.Stop();
			mainEngineParticles.Stop();
		}
	}

	private void ApplyThrust()
	{
		rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);

		if (!audioSource.isPlaying)
		{
			audioSource.PlayOneShot(mainEngine);
		}

		mainEngineParticles.Play();
	}

	private void RespondToRotateInput()
	{
		rigidBody.freezeRotation = true;
		float rotationThisFrame = rcsThrust * Time.deltaTime;

		if (Input.GetKey(KeyCode.A))
		{
			transform.Rotate(Vector3.forward * rotationThisFrame);
		}
		else if (Input.GetKey(KeyCode.D))
		{
			transform.Rotate(-Vector3.forward * rotationThisFrame);
		}

		// Resume physics control 
		rigidBody.freezeRotation = false;
	}
}
