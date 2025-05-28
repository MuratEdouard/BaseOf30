using System.Collections;
using System.Linq;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("General Settings")]
    public GameObject enemies;
    public GameObject floor;
    public GameObject obstacles;
    public PillarController pillar;
    public Sprite[] floorSprites;
    public GameObject leftDigit;
    public GameObject rightDigit;
    [Header("Prefabs Settings")]
    public GameObject grassPrefab;
    public int nbGrass = 10;

    public GameObject obstaclePrefab;
    public int nbObstacles = 4;

    public GameObject[] enemyPrefabs;

    void Awake()
    {
        Random.InitState(System.DateTime.Now.Ticks.GetHashCode());
    }

    void Start()
    {
        UpdateFloorDigits();
        PlaceGrass();
        PlaceObstacles();
        StartCoroutine(SpawnEnemies());
        InvokeRepeating(nameof(CheckIfAllEnemiesDefeated), 2f, 2f);
    }

    private void UpdateFloorDigits()
    {
        int tens = GameManager.instance.roomNb / 10;
        int units = GameManager.instance.roomNb % 10;

        if (tens < floorSprites.Length && units < floorSprites.Length)
        {
            leftDigit.GetComponent<SpriteRenderer>().sprite = floorSprites[tens];
            rightDigit.GetComponent<SpriteRenderer>().sprite = floorSprites[units];
        }
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

    private IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(1f);

        Vector2[] positions = new Vector2[] { };

        int nbEnemies = 0;

        int roomNb = GameManager.instance.roomNb;
        if (roomNb >= 9 && roomNb <= 10)
            nbEnemies = 2; // Calm intro
        else if (roomNb >= 7 && roomNb <= 8)
            nbEnemies = 3; // First spike
        else if (roomNb == 6)
            nbEnemies = 5; // First boss challenge
        else if (roomNb >= 4 && roomNb <= 5)
            nbEnemies = 3; // Midgame heat
        else if (roomNb == 3)
            nbEnemies = 4; // Mini panic
        else if (roomNb == 2)
            nbEnemies = 5; // Second boss challenge
        else if (roomNb == 1)
            nbEnemies = 6; // FINAL ROOM


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

            float waitTime = Random.Range(0.2f, 0.6f);
            yield return new WaitForSeconds(waitTime);
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
