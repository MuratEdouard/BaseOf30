using UnityEngine;

public class PillarController : MonoBehaviour
{
    public Animator animator;
    public PlayerController player;
    public AudioSource audioSource;

    private bool isPoppingUp = false;
    private bool isPoppedUp = false;

    public void PopUp()
    {
        if (!isPoppingUp)
        {
            isPoppingUp = true;
            animator.Play("PopUp");

            audioSource.Play();

            float clipLength = Utils.GetAnimationClipLength(animator, "PopUp");
            Invoke(nameof(OnPoppedUp), clipLength);
        }
    }
    private void OnPoppedUp()
    {
        isPoppedUp = true;
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (isPoppedUp && other.GetComponent<PlayerController>() != null)
        {
            Activate();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (isPoppedUp && other.GetComponent<PlayerController>() != null)
        {
            Activate();
        }
    }


    public void Activate()
    {
        animator.Play("Activate");
        float clipLength = Utils.GetAnimationClipLength(animator, "Activate");
        Invoke(nameof(OnActivated), clipLength);
    }
    private void OnActivated()
    {
        player.TeleportOut();
    }
}
