using UnityEngine;

public class MenuPlayerController : MonoBehaviour
{

    public Animator animator;
    public AudioSource audioSource;

    public void Jetpack()
    {
        animator.Play("Jetpack");
        audioSource.Play();
    }


    public void JetpackDown()
    {
        animator.Play("JetpackDown");
        audioSource.Stop();
    }

    public void Idle()
    {
        animator.Play("Idle");
    }
}
