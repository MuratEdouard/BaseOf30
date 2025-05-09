using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Prefabs Settings")]
    public GameObject grassPrefab;
    public int nbGrass = 10;

    public GameObject obstaclePrefab;
    public int nbObstacles = 4;

    public GameObject wallPrefab;

    void Awake()
    {
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
    }

    void Start()
    {
        PlaceGrass();
        PlaceObstacles();
    }

    private void PlaceGrass()
    {
        Vector2[] positions = new Vector2[]{};

        int nbGrassPositioned = 0;
        while (nbGrassPositioned < nbGrass)
        {
            var randX = Random.Range(-9, 9);
            var randY = Random.Range(-5, 5);
            var randVector2 = new Vector2(randX, randY);

            if (positions.Contains(randVector2))
            {
                continue;
            }
            else
            {
                GameObject grass = Instantiate(grassPrefab, transform);
                grass.transform.localPosition = randVector2;
                positions.Append(randVector2);
                nbGrassPositioned++;
            }   
        }
    }

    private void PlaceObstacles()
    {

    }
}
