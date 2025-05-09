using NUnit.Framework;
using UnityEngine;
using UnityEngine.UIElements;

public class GrassController : MonoBehaviour
{
    public Sprite[] sprites;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        sprites.Shuffle();
        sr.sprite = sprites[0];
    }
}
