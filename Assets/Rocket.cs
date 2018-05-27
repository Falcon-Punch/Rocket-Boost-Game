using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
	[SerializeField] float rcsThrust = 100f;
	[SerializeField] float mainThrust = 100f;
	[SerializeField] float levelLoadDelay = 1f;

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
		Invoke("LoadNextLevel", levelLoadDelay);
	}

	private void DeathSequence()
	{
		state = State.Dying;
		audioSource.Stop();
		audioSource.PlayOneShot(death);
		deathParticles.Play();
		Invoke("LoadFirstLevel", levelLoadDelay);
	}

	private void LoadNextLevel()
	{
		SceneManager.LoadScene(1); // todo allow for more than 2 levels
	}

	private void LoadFirstLevel()
	{
		SceneManager.LoadScene(0);
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

		//resume physics control 
		rigidBody.freezeRotation = false;
	}
}
