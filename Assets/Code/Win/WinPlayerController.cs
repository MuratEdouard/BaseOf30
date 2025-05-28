using UnityEngine;

public class WinPlayerController : MonoBehaviour
{

    private Animator animator;
    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void Jetpack()
    {
        animator.Play("Jetpack");
    }


    public void JetpackUp()
    {
        animator.Play("JetpackUp");
        audioSource.Play();
    }

    public void Idle()
    {
        animator.Play("Idle");
    }
}
