using System;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public MenuPlayerController player;
    public Canvas canvas;
    public GameObject textMenu;
    public GameObject textStart;
    public GameObject textExit;
    public Color textBlinkColor;
    public AudioSource clickAudioSource;

    private TextMeshProUGUI textStartComponent;
    private TextMeshProUGUI textExitComponent;
    private int tweenStartId;
    private int tweenExitId;
    private bool isStartSelected;

    private void Start()
    {
        textStartComponent = textStart.GetComponent<TextMeshProUGUI>();
        textExitComponent = textExit.GetComponent<TextMeshProUGUI>();

        canvas.enabled = false;
        LoopTextStartColor();
    }

    public void PlayerJetpack()
    {
        player.Jetpack();
    }
    public void PlayerJetpackDown()
    {
        player.JetpackDown();
    }

    public void PlayerIdle()
    {
        player.Idle();
    }

    public void ShowMenu()
    {
        canvas.enabled = true;
    }

    private void LoopTextStartColor()
    {
        isStartSelected = true;

        tweenStartId = LeanTween.value(gameObject, Color.white, textBlinkColor, 0.5f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnUpdate((Color val) =>
            {
                textStartComponent.color = val;
            }).id;

        LeanTween.cancel(tweenExitId);
        textExitComponent.color = Color.white;
    }

    private void LoopTextExitColor()
    {
        isStartSelected = false;

        tweenExitId = LeanTween.value(gameObject, Color.white, textBlinkColor, 0.5f)
            .setEaseInOutSine()
            .setLoopPingPong()
            .setOnUpdate((Color val) =>
            {
                textExitComponent.color = val;
            }).id;

        LeanTween.cancel(tweenStartId);
        textStartComponent.color = Color.white;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isStartSelected)
        {
            clickAudioSource.Play();
            LoopTextStartColor();
        }

        if (Input.GetKeyDown(KeyCode.DownArrow) && isStartSelected)
        {
            clickAudioSource.Play();
            LoopTextExitColor();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            clickAudioSource.Play();
            if (isStartSelected)
                StartGame();
            else
                ExitGame();
        }
    }

    private void StartGame()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void ExitGame()
    {
        Application.Quit();
    }
}
