using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlappyRocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField] float boosterThrust = 65f;
    [SerializeField] float mainThrust = 3f;
    [SerializeField] float levelLoadDelay = 2f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticle;
    [SerializeField] ParticleSystem deathParticle;
    [SerializeField] ParticleSystem successParticle;

    bool isCollisionDetectionOn = true;

    enum State { Alive, Trancending, Dead }

    [SerializeField]
    State state = State.Alive;

    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (state == State.Alive)
        {
            ResponseToThrustInput();
            ResponseToRotateInput();
        }

        if (Debug.isDebugBuild)
        {
            ResponseToDebug();
        }
    }

    void ResponseToDebug()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            LoadFirstLevel();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            isCollisionDetectionOn = !isCollisionDetectionOn;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collisions when dead

        if (isCollisionDetectionOn)
        {
            switch (collision.gameObject.tag)
            {
                case "Friendly":
                    break;
                case "Finish":
                    StartSuccessSequence();
                    break;
                default:
                    StartDeathSequence();
                    break;
            }
        }
    }

    private void StartDeathSequence()
    {
        state = State.Dead;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticle.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Trancending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticle.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void ResponseToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
            //mainEngineParticle.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying)
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticle.Play();
    }

    private void ResponseToRotateInput()
    {
        rigidBody.angularVelocity = Vector3.zero;
        //rigidBody.freezeRotation = true;
        var rotationThisFrame = boosterThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        //rigidBody.freezeRotation = false;
    }

}
