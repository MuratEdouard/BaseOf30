using UnityEngine;

public class PlayerAfterImageController : MonoBehaviour
{
    public float fadeSpeed = 1f;

    private SpriteRenderer sr;
    private Vector3 scale;
    private Color color;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        scale = transform.localScale;
        color = sr.color;
    }

    public void Init(Vector3 position, bool facingRight)
    {
        transform.position = position;
        if (!facingRight)
            transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
    }

    void Update()
    {
        color.a -= fadeSpeed * Time.deltaTime;
        sr.color  = color;

        if (color.a <= 0f)
            Destroy(gameObject);
    }
}
