using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject enemies;
    public GameObject floor;
    public GameObject obstacles;

    [Header("Prefabs Settings")]
    public GameObject grassPrefab;
    public int nbGrass = 10;

    public GameObject obstaclePrefab;
    public int nbObstacles = 4;

    public GameObject[] enemyPrefabs;
    public int nbEnemies = 2;

    public PillarController pillar;

    void Awake()
    {
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
    }

    void Start()
    {
        PlaceGrass();
        PlaceObstacles();
        Invoke(nameof(SpawnEnemies), 1f);
        InvokeRepeating(nameof(CheckIfAllEnemiesDefeated), 2f, 2f);
    }

    private void PlaceGrass()
    {
        Vector2[] positions = new Vector2[] { };

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
                GameObject grass = Instantiate(grassPrefab, floor.transform);
                grass.transform.localPosition = randVector2;
                positions.Append(randVector2);
                nbGrassPositioned++;
            }   
        }
    }

    private void PlaceObstacles()
    {
        Vector2[] positions = new Vector2[] { Vector2.zero };

        int nbObstaclesPositioned= 0;
        while (nbObstaclesPositioned < nbObstacles)
        {
            var tooCloseToOtherObstacle = false;

            var randX = Random.Range(-6, 6);
            var randY = Random.Range(-4, 4);
            var randVector2 = new Vector2(randX, randY);

            foreach (var position in positions) {
                if (Vector2.Distance(randVector2, position) < 2) {
                    tooCloseToOtherObstacle = true;
                    break;
                }
            }

            if (tooCloseToOtherObstacle)
            {
                continue;
            }
            else
            {
                GameObject obstacle = Instantiate(obstaclePrefab, obstacles.transform);
                obstacle.transform.localPosition = randVector2;
                positions.Append(randVector2);
                nbObstaclesPositioned++;
            }
        }
    }

    private void SpawnEnemies()
    {
        Vector2[] positions = new Vector2[] { };

        int nbEnemiesPositioned = 0;
        while (nbEnemiesPositioned < nbEnemies)
        {
            var randX = Random.Range(-9f, 9f);
            var randY = Random.Range(-5f, 5f);
            var randVector2 = new Vector2(randX, randY);

            enemyPrefabs.Shuffle();

            GameObject enemy = Instantiate(enemyPrefabs[0], enemies.transform);
            enemy.transform.localPosition = randVector2;
            positions.Append(randVector2);
            nbEnemiesPositioned++;
        }
    }

    private void CheckIfAllEnemiesDefeated()
    {
        if(enemies.transform.childCount <= 0)
        {
            pillar.PopUp();
        }
    }
}
