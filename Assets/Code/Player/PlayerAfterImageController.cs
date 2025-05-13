using UnityEngine;

public class PlayerAfterImageController : MonoBehaviour
{
    public float fadeSpeed = 1f;

    private SpriteRenderer sr;
    private Color color;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        color = sr.color;
    }

    public void Init(Vector3 position, bool facingRight)
    {
        transform.position = position;
        sr.flipX = (!facingRight);
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color  = color;

        if (color.a <= 0f)
            Destroy(gameObject);
    }
}
