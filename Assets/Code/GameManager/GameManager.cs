using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int roomNb = 10;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void DecreaseRoomNumber()
    {
        roomNb = Mathf.Max(0, roomNb - 1);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }
}
