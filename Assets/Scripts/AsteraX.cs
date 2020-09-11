using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteraX : MonoBehaviour
{
    // this makes the Singleton - although we are modifying it
    // allows us to access some of the fields here for other classes
    // 
    private static AsteraX _S;

    static List<AsteroidController> ASTEROIDS;

    #region Field Declarations
    [Header("Asteriod Prefabs")]
    [Space]
    public AsteroidsScriptableObject asteroidValues;

    const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;

    public static int TOTAL_SCORE = 0;
    public static int JUMPS = 0;
    public static eGameState _GAME_STATE = eGameState.mainMenu;
    public static float RELOAD_SCENE_DELAY;

    [Header("Level Design")]
    public List<LevelDefinition_SO> levels;
    [HideInInspector] public LevelDefinition_SO currentLevel;
    [HideInInspector] public int levelIndex = 0;
    [Space]
    [SerializeField]
    private int startingJumps = 3;
    [Tooltip("Set the amount of time the reload of the level should be")]
    public float sceneDeleyTime = 4f;

    private float MIN_DISTANCE_AST_FROM_SHIP = 5f;

    // System.Flags changes how eGameStates are viewed in the Inspector and lets multiple 
    //  values be selected simultaneously (similar to how Physics Layers are selected).
    // It's only valid for the game to ever be in one state, but I've added System.Flags
    //  here to demonstrate it and to make the ActiveOnlyDuringSomeGameStates script easier
    //  to view and modify in the Inspector.
    // When you use System.Flags, you still need to set each enum value so that it aligns 
    //  with a power of 2. You can also define enums that combine two or more values,
    //  for example the all value below that combines all other possible values.
    [System.Flags]
    public enum eGameState
    {
        // Decimal      // Binary
        none = 0,       // 00000000
        mainMenu = 1,   // 00000001
        preLevel = 2,   // 00000010
        level = 4,      // 00000100
        postLevel = 8,  // 00001000
        gameOver = 16,  // 00010000
        all = 0xFFFFFFF // 11111111111111111111111111111111
    }
    #endregion

    private void Awake()
    {
        // last step to make the singleton
        S = this;
        SetNumberOfJumps();
    }


    void Start()
    {
        #region Event Listeners
        EventBroker.BulletHitsAsteroid += AddPoints;
        EventBroker.ShipHitAsteroid += ShipGotHitByAsteroid;
        #endregion

        #region Set Members
        RELOAD_SCENE_DELAY = sceneDeleyTime;
        GAME_STATE = eGameState.level;
        ASTEROIDS = new List<AsteroidController>();
        #endregion

        StartLevel();
    }

    private void OnDisable()
    {
        EventBroker.BulletHitsAsteroid -= AddPoints;
        EventBroker.ShipHitAsteroid -= ShipGotHitByAsteroid;
    }

    private void OnDestroy()
    {
        AsteraX.GAME_STATE = eGameState.none;
    }

    private void SetNumberOfJumps()
    {
        JUMPS = startingJumps;
    }

    private void StartLevel()
    {
        currentLevel = levels[levelIndex];

        for (int i = 0; i < currentLevel.initialAsteroidCount; i++)
        {
            SpawnParentAsteroid(i);
        }

    }


    private void SpawnParentAsteroid(int index)
    {
        AsteroidController asteroid = AsteroidController.SpawnAsteroid();
        Vector3 spawnLocation;
        do
        {
            spawnLocation = ScreenBounds.RandomLocationOnScreen();
        } while ((spawnLocation - PlayerShipController.POSITION).magnitude < MIN_DISTANCE_AST_FROM_SHIP);
        asteroid.transform.position = spawnLocation;
        asteroid.transform.rotation = Random.rotation;
        asteroid.asteroidSize = currentLevel.initialAsteroidSize;
        asteroid.gameObject.name = "Asteroid_" + index.ToString("00");

    }

    // subscribed to the bullet hitting asteroid event (set up in EventBroker)
    private void AddPoints(int sizeOfAsteroid)
    {
        int pointValue = AsteroidsSO.pointsForAsteroidSize[sizeOfAsteroid];
        TOTAL_SCORE += pointValue;
        EventBroker.CallUpdateScore(TOTAL_SCORE);
    }


    private void ShipGotHitByAsteroid()
    {
        JUMPS--;

        EventBroker.CallUpdateJumps(JUMPS);

        if (JUMPS < 0)
        {
            EventBroker.CallGameOver();
            EndGame();
            return;
        }
        EventBroker.CallRespawnShip();
    }

    private void EndGame()
    {
        GAME_STATE = eGameState.gameOver;
        Invoke("ReloadScene", RELOAD_SCENE_DELAY);
    }

    private void ReloadScene()
    {
        SetNumberOfJumps();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }



    #region Static Section


    // Setting the Static private member to interact with the Script. Used for Singleton purpose only
    // Makes sure this isn't null prior to accessing members/functions
    private static AsteraX S
    {
        get
        {
            if (_S == null)
            {
                Debug.LogError("Trying to get AsteraX prior to it being set");
                return null;
            }
            return _S;
        }
        set
        {
            if (_S != null)
            {
                Debug.LogError("AsteraX setter already set, trying to reset this");
            }
            _S = value;
        }
    }


    static public AsteroidsScriptableObject AsteroidsSO
    {
        get
        {
            if (S != null)
            {
                return S.asteroidValues;
            }
            return null;
        }
    }

    // allows AsteroidController access to sub asteroid data in the Level Data Scriptable Object
    // since AsteroidSO is public (bc it's a SO) only can access it through another static public variable declaration
    static public int SubAsteroidsToSpawn
    {
        get
        {
            if (S != null)
            {
                return S.currentLevel.subAsteriodsToSpawn;
            }
            return 0;
        }
    }

    static public void AddAsteroid(AsteroidController asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) == -1)
        {
            ASTEROIDS.Add(asteroid);
        }
    }
    static public void RemoveAsteroid(AsteroidController asteroid)
    {
        if (ASTEROIDS.IndexOf(asteroid) != -1)
        {
            ASTEROIDS.Remove(asteroid);
        }
    }

    static public int CurrentLevel
    {
        get
        {
            if (S != null)
            {
                return S.levelIndex + 1;
            }
            return 1;
        }
    }


    static public eGameState GAME_STATE
    {
        get
        {
            return _GAME_STATE;
        }
        set
        {
            if (value != _GAME_STATE)
            {

                _GAME_STATE = value;
                EventBroker.CallGameStateChange();
            }
        }
    }
    #endregion
}