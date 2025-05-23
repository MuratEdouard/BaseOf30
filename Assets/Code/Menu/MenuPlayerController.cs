using UnityEngine;

public class MenuPlayerController : MonoBehaviour
{

    public Animator animator;

    public void Jetpack()
    {
        animator.Play("Jetpack");
    }


    public void JetpackDown()
    {
        animator.Play("JetpackDown");
    }

    public void Idle()
    {
        animator.Play("Idle");
    }
}
