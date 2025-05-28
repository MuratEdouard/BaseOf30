using UnityEngine;
using UnityEngine.SceneManagement;

public class WinController : MonoBehaviour
{
    public WinPlayerController player;
    public GameObject blackOverlay;

    public void PlayerJetpack()
    {
        player.Jetpack();
    }
    public void PlayerJetpackUp()
    {
        player.JetpackUp();
    }

    public void PlayerIdle()
    {
        player.Idle();
    }

    private void GoBackToMenu()
    {

        SpriteRenderer sr = blackOverlay.GetComponent<SpriteRenderer>();
        var color = sr.color;

        LeanTween.value(gameObject, color, Color.black, 2f)
            .setEaseInOutSine()
            .setOnUpdate((Color val) =>
            {
                sr.color = val;
            })
            .setOnComplete(() =>
            {
                SceneManager.LoadScene("MenuScene");
            });
    }
}
