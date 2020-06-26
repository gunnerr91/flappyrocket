using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FlappyRocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;

    [SerializeField]
    float boosterThrust = 1f;

    [SerializeField]
    float mainThrust = 1f;

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
        if (state.Equals(State.Dead))
        {
            audioSource.Stop();
            return;
        }
        else
        {
            Thrust();
            Rotate();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Trancending;
                Invoke("LoadNextScene", 2f);
                break;
            case "Respawn":
                state = State.Dead;
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    Invoke("LoadCurrentScene", 1f);
                }
                else
                {
                    Invoke("LoadCurrentScene", 1f);
                }
                print("game over");
                break;
            default:
                if (SceneManager.GetActiveScene().buildIndex == 1)
                {
                    SceneManager.LoadScene(1);
                }
                else
                {
                    SceneManager.LoadScene(0);
                }
                print("game over");
                break;
        }
    }

    private void LoadCurrentScene()
    {
        state = State.Alive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        state = State.Alive;
        SceneManager.LoadScene(1);
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            rigidBody.AddRelativeForce(Vector3.up * mainThrust);
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void Rotate()
    {
        rigidBody.freezeRotation = true;
        var rotationThisFrame = boosterThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

}
