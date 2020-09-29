using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f; 

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip playerDead;
    [SerializeField] AudioClip levelLoad;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    Rigidbody rigidBody;
    AudioSource audioSource;

    bool isTransitioning = false; 

    [SerializeField] bool collisionsDisabled = true; 

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>(); 
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTransitioning)
        {

            RespondToThrustInput();
            RespondToRotateInput();

        }

        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }
        
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel(); 
        } 
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isTransitioning || collisionsDisabled) { return; } //ignore collisions when dead

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                //do nothing
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }


    private void StartSuccessSequence()
    {
        isTransitioning = true;
        audioSource.Stop(); //stopping current sound 
        audioSource.PlayOneShot(levelLoad);
        successParticles.Play(); 
        Invoke("LoadNextLevel", levelLoadDelay); 
    }

    private void StartDeathSequence()
    {
        isTransitioning = true; 
        audioSource.Stop();
        audioSource.PlayOneShot(playerDead);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1; 
        if(nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0; //loop back to start
        }
        SceneManager.LoadScene(nextSceneIndex); //todo allow for more than 2 levels 
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
            StopApplyingThrust();
        }

    }

    private void StopApplyingThrust()
    {
        audioSource.Stop();
        mainEngineParticles.Stop();
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime); //Multiple by Time.deltaTime to make frame independent
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }

    private void RespondToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero; // remove rotation due to physics
        
        float rotationThisFrame = rcsThrust * Time.deltaTime; 

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        
    }

}
