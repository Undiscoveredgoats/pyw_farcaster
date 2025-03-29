//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class ObstacleSpawner : MonoBehaviour
//{
//    [SerializeField] private List<GameObject> obstaclePrefabs; // List of obstacle prefabs to spawn
//    [SerializeField] private GameObject player;               // Reference to the player for tracking position
//    [SerializeField] private float initialSpawnRate = 2f;     // Initial time between spawns (seconds)
//    [SerializeField] private float minSpawnRate = 0.5f;       // Minimum spawn rate (highest difficulty)
//    [SerializeField] private float initialMinXDistance = 5f;  // Initial minimum X distance between obstacles
//    [SerializeField] private float initialMaxXDistance = 10f; // Initial maximum X distance between obstacles
//    [SerializeField] private float minXDistanceLimit = 2f;    // Minimum X distance at max difficulty
//    [SerializeField] private float initialMoveSpeed = 2f;     // Initial movement speed of obstacles
//    [SerializeField] private float maxMoveSpeed = 5f;         // Maximum movement speed at highest difficulty
//    [SerializeField] private float ySpawnRangeBelow = 3f;     // Max distance below player to spawn
//    [SerializeField] private float ySpawnRangeAbove = 5f;     // Max distance above player to spawn

//    private float currentSpawnRate;
//    private float currentMinXDistance;
//    private float currentMaxXDistance;
//    private float currentMoveSpeed;
//    private float lastSpawnX = 0f; // Track the last spawn position
//    private float difficultyFactor = 0f; // Ranges from 0 (start) to 1 (max difficulty)
//    private const float difficultyIncreaseRate = 0.01f; // How fast difficulty increases per second

//    void Start()
//    {
//        // Initialize starting values
//        currentSpawnRate = initialSpawnRate;
//        currentMinXDistance = initialMinXDistance;
//        currentMaxXDistance = initialMaxXDistance;
//        currentMoveSpeed = initialMoveSpeed;

//        if (player == null)
//        {
//            Debug.LogError("Player reference not set in ObstacleSpawner!");
//        }

//        // Start endless spawning
//        StartCoroutine(SpawnObstaclesEndlessly());
//    }

//    void Update()
//    {
//        // Increase difficulty over time
//        difficultyFactor = Mathf.Clamp01(difficultyFactor + difficultyIncreaseRate * Time.deltaTime);
//        UpdateDifficulty();
//    }

//    void UpdateDifficulty()
//    {
//        // Gradually adjust spawn rate, X distance, and move speed based on difficulty factor
//        currentSpawnRate = Mathf.Lerp(initialSpawnRate, minSpawnRate, difficultyFactor);
//        currentMinXDistance = Mathf.Lerp(initialMinXDistance, minXDistanceLimit, difficultyFactor);
//        currentMaxXDistance = Mathf.Lerp(initialMaxXDistance, minXDistanceLimit + 2f, difficultyFactor); // Keep some range
//        currentMoveSpeed = Mathf.Lerp(initialMoveSpeed, maxMoveSpeed, difficultyFactor);
//    }

//    IEnumerator SpawnObstaclesEndlessly()
//    {
//        while (true)
//        {
//            SpawnObstacle();
//            yield return new WaitForSeconds(currentSpawnRate);
//        }
//    }

//    void SpawnObstacle()
//    {
//        if (player == null) return;

//        // Spawn ahead of the player on X-axis
//        float spawnX = player.transform.position.x + Random.Range(currentMinXDistance, currentMaxXDistance);
//        if (spawnX < lastSpawnX + currentMinXDistance) // Ensure minimum spacing
//        {
//            spawnX = lastSpawnX + currentMinXDistance;
//        }

//        // Spawn Y position relative to player's current Y
//        float playerY = player.transform.position.y;
//        float spawnY = playerY + Random.Range(-ySpawnRangeBelow, ySpawnRangeAbove);
//        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

//        // Pick a random obstacle prefab
//        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

//        // Instantiate the obstacle
//        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, transform);

//        // Assign a random movement pattern
//        ObstacleMovement movement = obstacle.AddComponent<ObstacleMovement>();
//        movement.Initialize(GetRandomMovementType(), currentMoveSpeed);

//        lastSpawnX = spawnX; // Update last spawn position
//    }

//    MovementType GetRandomMovementType()
//    {
//        MovementType[] movementTypes = (MovementType[])System.Enum.GetValues(typeof(MovementType));
//        return movementTypes[Random.Range(0, movementTypes.Length)];
//    }
//}

//public class ObstacleMovement : MonoBehaviour
//{
//    private MovementType movementType;
//    private float speed;
//    private float timeElapsed = 0f;
//    private Vector3 startPosition;

//    // Movement-specific parameters
//    private float amplitude = 2f;  // For up-down, zigzag, and wavy
//    private float frequency = 1f;  // For up-down, zigzag, and wavy
//    private float radius = 2f;     // For circular

//    public void Initialize(MovementType type, float moveSpeed)
//    {
//        movementType = type;
//        speed = moveSpeed;
//        startPosition = transform.position;
//    }

//    void Update()
//    {
//        timeElapsed += Time.deltaTime;
//        MoveObstacle();
//    }

//    void MoveObstacle()
//    {
//        Vector3 newPosition = transform.position;

//        switch (movementType)
//        {
//            case MovementType.ZigZag:
//                newPosition.x = startPosition.x + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
//                newPosition.y = startPosition.y + Mathf.Cos(timeElapsed * speed * frequency) * amplitude / 2f;
//                break;

//            case MovementType.Circular:
//                newPosition.x = startPosition.x + Mathf.Cos(timeElapsed * speed) * radius;
//                newPosition.y = startPosition.y + Mathf.Sin(timeElapsed * speed) * radius;
//                break;

//            case MovementType.UpDown:
//                newPosition.y = startPosition.y + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
//                break;

//            case MovementType.LeftRight:
//                newPosition.x = startPosition.x + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
//                break;

//            case MovementType.Wavy:
//                newPosition.x = startPosition.x + timeElapsed * speed; // Moves right over time
//                newPosition.y = startPosition.y + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
//                break;
//        }

//        transform.position = newPosition;
//    }
//}

//public enum MovementType
//{
//    ZigZag,
//    Circular,
//    UpDown,
//    LeftRight,
//    Wavy
//}

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] private List<GameObject> obstaclePrefabs; // List of obstacle prefabs to spawn
    [SerializeField] private GameObject player;               // Reference to the player for tracking position
    [SerializeField] private float initialSpawnRate = 2f;     // Initial time between spawns (seconds)
    [SerializeField] private float minSpawnRate = 0.5f;       // Minimum spawn rate (highest difficulty)
    [SerializeField] private float initialMinXDistance = 5f;  // Initial minimum X distance between obstacles
    [SerializeField] private float initialMaxXDistance = 10f; // Initial maximum X distance between obstacles
    [SerializeField] private float minXDistanceLimit = 2f;    // Minimum X distance at max difficulty
    [SerializeField] private float initialMoveSpeed = 2f;     // Initial movement speed of obstacles
    [SerializeField] private float maxMoveSpeed = 5f;         // Maximum movement speed at highest difficulty
    [SerializeField] private float ySpawnRangeBelow = 3f;     // Max distance below player to spawn
    [SerializeField] private float ySpawnRangeAbove = 5f;     // Max distance above player to spawn

    private float currentSpawnRate;
    private float currentMinXDistance;
    private float currentMaxXDistance;
    private float currentMoveSpeed;
    private float lastSpawnX = 0f; // Track the last spawn position
    private float difficultyFactor = 0f; // Ranges from 0 (start) to 1 (max difficulty)
    private const float difficultyIncreaseRate = 0.01f; // How fast difficulty increases per second

    void Start()
    {
        // Initialize starting values
        currentSpawnRate = initialSpawnRate;
        currentMinXDistance = initialMinXDistance;
        currentMaxXDistance = initialMaxXDistance;
        currentMoveSpeed = initialMoveSpeed;

        if (player == null)
        {
            Debug.LogError("Player reference not set in ObstacleSpawner!");
        }

        // Start endless spawning
        StartCoroutine(SpawnObstaclesEndlessly());
    }

    void Update()
    {
        // Increase difficulty over time
        difficultyFactor = Mathf.Clamp01(difficultyFactor + difficultyIncreaseRate * Time.deltaTime);
        UpdateDifficulty();
    }

    void UpdateDifficulty()
    {
        // Gradually adjust spawn rate, X distance, and move speed based on difficulty factor
        currentSpawnRate = Mathf.Lerp(initialSpawnRate, minSpawnRate, difficultyFactor);
        currentMinXDistance = Mathf.Lerp(initialMinXDistance, minXDistanceLimit, difficultyFactor);
        currentMaxXDistance = Mathf.Lerp(initialMaxXDistance, minXDistanceLimit + 2f, difficultyFactor); // Keep some range
        currentMoveSpeed = Mathf.Lerp(initialMoveSpeed, maxMoveSpeed, difficultyFactor);
    }

    IEnumerator SpawnObstaclesEndlessly()
    {
        while (true)
        {
            SpawnObstacle();
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }

    void SpawnObstacle()
    {
        if (player == null) return;

        // Spawn ahead of the player on X-axis
        float spawnX = player.transform.position.x + Random.Range(currentMinXDistance, currentMaxXDistance);
        if (spawnX < lastSpawnX + currentMinXDistance) // Ensure minimum spacing
        {
            spawnX = lastSpawnX + currentMinXDistance;
        }

        // Spawn Y position relative to player's current Y
        float playerY = player.transform.position.y;
        float spawnY = playerY + Random.Range(-ySpawnRangeBelow, ySpawnRangeAbove);
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0f);

        // Pick a random obstacle prefab
        GameObject obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Count)];

        // Instantiate the obstacle
        GameObject obstacle = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity, transform);

        // Assign a random movement pattern
        ObstacleMovement movement = obstacle.AddComponent<ObstacleMovement>();
        movement.Initialize(GetRandomMovementType(), currentMoveSpeed, player);

        lastSpawnX = spawnX; // Update last spawn position
    }

    MovementType GetRandomMovementType()
    {
        MovementType[] movementTypes = (MovementType[])System.Enum.GetValues(typeof(MovementType));
        return movementTypes[Random.Range(0, movementTypes.Length)];
    }
}

public class ObstacleMovement : MonoBehaviour
{
    private MovementType movementType;
    private float speed;
    private float timeElapsed = 0f;
    private Vector3 startPosition;
    private GameObject player;
    private GameManager gameManager;
    private bool bonusAwarded = false;

    // Movement-specific parameters
    private float amplitude = 2f;  // For up-down, zigzag, and wavy
    private float frequency = 1f;  // For up-down, zigzag, and wavy
    private float radius = 2f;     // For circular

    public void Initialize(MovementType type, float moveSpeed, GameObject playerRef)
    {
        movementType = type;
        speed = moveSpeed;
        startPosition = transform.position;
        player = playerRef;
        gameManager = FindObjectOfType<GameManager>();
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
        MoveObstacle();

        // Award bonus XP when player passes the obstacle
        if (!bonusAwarded && player != null && player.transform.position.x > transform.position.x + 2f)
        {
            bonusAwarded = true;
            if (gameManager != null)
            {
                gameManager.AddObstacleBonus();
            }
        }

        // Destroy obstacle if far behind player
        if (player != null && transform.position.x < player.transform.position.x - 20f)
        {
            Destroy(gameObject);
        }
    }

    void MoveObstacle()
    {
        Vector3 newPosition = transform.position;

        switch (movementType)
        {
            case MovementType.ZigZag:
                newPosition.x = startPosition.x + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
                newPosition.y = startPosition.y + Mathf.Cos(timeElapsed * speed * frequency) * amplitude / 2f;
                break;

            case MovementType.Circular:
                newPosition.x = startPosition.x + Mathf.Cos(timeElapsed * speed) * radius;
                newPosition.y = startPosition.y + Mathf.Sin(timeElapsed * speed) * radius;
                break;

            case MovementType.UpDown:
                newPosition.y = startPosition.y + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
                break;

            case MovementType.LeftRight:
                newPosition.x = startPosition.x + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
                break;

            case MovementType.Wavy:
                newPosition.x = startPosition.x + timeElapsed * speed; // Moves right over time
                newPosition.y = startPosition.y + Mathf.Sin(timeElapsed * speed * frequency) * amplitude;
                break;
        }

        transform.position = newPosition;
    }
}

public enum MovementType
{
    ZigZag,
    Circular,
    UpDown,
    LeftRight,
    Wavy
}