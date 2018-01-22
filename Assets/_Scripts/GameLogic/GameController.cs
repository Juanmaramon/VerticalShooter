using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using UnityEngine.SceneManagement;

public enum EnemyType
{
    COLUMN,
    SIDE
}

/// <summary>
/// Enemy wave item.
/// </summary>
[System.Serializable]
public class EnemyWaveItem
{
    // Type of enemy
    public EnemyType type;
    // Left spawn area
    public SpawnArea leftSpawnArea;
    // Right spawn area
    public SpawnArea rightSpawnArea;
    // Time to wait until first wave
    public float firstWait;
    // Number of enemies on a wave
    public int waveCount;
    // Time to wait between enemies creation in the same wave
    public float spawnWait;
    // Time between waves
    public float waveWait;
    // If enemies are spawned from left or right spawn area
    public bool leftSpawn;
}

public class GameController : MonoBehaviour 
{

    #region Private attributes

    // If enemy spawn is executing
    public bool execute = true;

    // Number enemies spawned on left side
    int leftSpawnNumber;
    // Number enemies spawned on right side
    int rightSpawnNumber;
    // Max number enemies spawned on two sides
    int maxSpawnNumber;

    // Time that needs to pass to win the game
    float timeFinishGame;

    int currentScore;

    #endregion 
    
    #region Public attributes

    public List<EnemyWaveItem> waves;

    [Header("Win conditions")]

    // Player can win due to two conditions score or time
    [SerializeField] int scoreToWin;
    [SerializeField] float timeToWin;

    #endregion

	// Use this for initialization
	void Start () 
    {
        StartCoroutine(SpawnColumnEnemies());
        StartCoroutine(SpawnSideEnemies());

        timeFinishGame = Time.time + timeToWin;

        EventManager.StartListening<BasicEvent>(Common.ON_RAISE_SCORE, OnRaiseScore);
	}
	
    /// <summary>
    /// Spawns the enemies. Spawn enemies following this logic:
    ///  There are two enemy spawn areas: top left and top right
    ///  Enemies spawned from one side with maxSpawnNumber on the other side only 1 enemy spawned
    ///  and swaping this rule
    /// </summary>
    IEnumerator SpawnColumnEnemies () 
    {
        // First wait
        yield return new WaitForSeconds(waves[1].firstWait);

        // While executing...
        while (execute)
        {
            // maxSpawnNumber from left side
            if (waves[1].leftSpawn)
            {
                leftSpawnNumber = waves[1].waveCount;
                rightSpawnNumber = 1;
            }
            // maxSpawnNumber from right side
            else
            {
                leftSpawnNumber = 1;
                rightSpawnNumber = waves[1].waveCount;
            }

            // Spawn waves...
            for (int i = 0; i < waves[1].waveCount; i++)
            {
                // Right spawn column
                if (i < rightSpawnNumber)
                {
                    CreateWave(waves[1].rightSpawnArea, Common.Side.RIGHT, EnemyType.COLUMN);
                }
                // Left spawn column
                if (i < leftSpawnNumber)
                {
                    CreateWave(waves[1].leftSpawnArea, Common.Side.LEFT, EnemyType.COLUMN);
                }

                // Wait time between enemy waves
                yield return new WaitForSeconds(waves[1].spawnWait);                
            }

            // Change number of enemies spawned by side 
            waves[1].leftSpawn = !waves[1].leftSpawn;

            // Wait time between waves
            yield return new WaitForSeconds(waves[1].waveWait);
        }
	}

    /// <summary>
    /// Spawns the enemies. Spawn enemies following this logic:
    ///  There are two enemy spawn areas: top left and top right
    ///  Enemies spawned from one side with maxSpawnNumber on the other side only 1 enemy spawned
    ///  and swaping this rule
    /// </summary>
    IEnumerator SpawnSideEnemies()
    {
        // First wait
        yield return new WaitForSeconds(waves[0].firstWait);

        // While executing...
        while (execute)
        {
            // Spawn waves...
            for (int i = 0; i < waves[0].waveCount; i++)
            {
                // Left spawn
                if (waves[0].leftSpawn)
                {
                    CreateWave(waves[0].leftSpawnArea, Common.Side.LEFT, EnemyType.SIDE);
                }
                // Right spawn
                else
                {
                    CreateWave(waves[0].rightSpawnArea, Common.Side.RIGHT, EnemyType.SIDE);
                }

                // Wait time between enemy waves
                yield return new WaitForSeconds(waves[0].spawnWait);
            }

            // Change number of enemies spawned by side 
            waves[0].leftSpawn = !waves[0].leftSpawn;

            // Wait time between waves
            yield return new WaitForSeconds(waves[0].waveWait);
        }
    }


    /// <summary>
    /// Creates a wave of column enemies on one spawn side
    /// </summary>
    /// <param name="spawnArea">Spawn area.</param>
    /// <param name="side">Side.</param>
    void CreateWave(SpawnArea spawnArea, Common.Side side, EnemyType enemyType)
    {
        string enemyName = "ColumnEnemy";
        switch (enemyType)
        {
            case EnemyType.SIDE:
                enemyName = "SideEnemy";
                break;
            case EnemyType.COLUMN:
                enemyName = "ColumnEnemy";
                break;
        }
        GameObject enemyInstantiate = ObjectPooler.Instance.GetPooledObject(enemyName);
        enemyInstantiate.transform.position = new Vector3(
                Random.Range(spawnArea.area.xMin, spawnArea.area.xMax),
                Random.Range(spawnArea.area.yMin, spawnArea.area.yMax),
                0f
        );
        enemyInstantiate.GetComponent<EnemyBase>().ConfigureMovement(side);       
    }

    /// <summary>
    /// Restarts the level.
    /// </summary>
    public void RestartLevel()
    {
        Cleanup();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// On player score changed. Updates screen text
    /// </summary>
    /// <param name="e">Event information. Player score</param>
    void OnRaiseScore(BasicEvent e)
    {
        var intScore = (int)e.Data;

        currentScore += intScore;

        if (currentScore >= scoreToWin)
        {
            Persistence.SetMaxValue("Score", currentScore);
            EventManager.TriggerEvent(Common.ON_WIN_GAME);
            execute = false;            
        }
    }

    void Update()
    {
        if (Time.time >= timeFinishGame)
        {
            Persistence.SetMaxValue("Score", currentScore);
            EventManager.TriggerEvent(Common.ON_WIN_GAME);
            execute = false;
        }
    }

    void Cleanup()
    {
        EventManager.StopListening<BasicEvent>(Common.ON_RAISE_SCORE, OnRaiseScore);
    }
}
