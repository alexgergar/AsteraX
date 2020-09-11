using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityStandardAssets.CrossPlatformInput;

[RequireComponent(typeof(Rigidbody))]
public class PlayerShipController : MonoBehaviour
{
    static private PlayerShipController _S;
    static public PlayerShipController S
    {

        get
        {
            return _S;
        }
        private set
        {
            if (_S != null)
            {
                Debug.LogWarning("Second attempt to get Playership singleton _S!");
            }
            _S = value;
        }
    }

    [Header("Set In Header")]
    [SerializeField] private float speed = 10f;
    [Tooltip("Time Delay of Respawn")]
    [SerializeField]
    private float respawnDelay = 1f;

    [Header("Prefabs")]
    [SerializeField] private GameObject bulletSpawnInfo;
    [SerializeField] private BulletController bulletPrefab;

    private Rigidbody rigid;

    static public float LAST_COLLISION = -1000;
    static public float COLLISION_DELAY = 1;


    private void Awake()
    {
        S = this;
        rigid = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        EventBroker.RespawnShip += HitByAsteroid;
        EventBroker.GameOver += DestroyShip;
    }

    private void OnDestroy()
    {
        EventBroker.ShipHitAsteroid -= HitByAsteroid;
        EventBroker.GameOver -= DestroyShip;
    }

    void Update()
    {
        MoveShip();
        if (CrossPlatformInputManager.GetButtonDown("Fire1"))
        {
            ShootBullet();
        }

    }


    private void MoveShip()
    {
        float hortizontalInput = CrossPlatformInputManager.GetAxis("Horizontal");
        float verticalInput = CrossPlatformInputManager.GetAxis("Vertical");

        Vector3 shipInput = new Vector3(hortizontalInput, verticalInput);
        // so we don't get a square root effect that causes diagionals to speed up
        if (shipInput.magnitude > 1)
        {
            shipInput.Normalize();
        }

        rigid.velocity = shipInput * speed;
    }


    private void ShootBullet()
    {
        // Find location of where to spawn
        Vector3 spawnPosition = bulletSpawnInfo.transform.position;

        // the Bullet Transform Ref's rotation is set to zero to fix the Vector3.forward ability
        Instantiate(bulletPrefab, spawnPosition, bulletSpawnInfo.transform.rotation);

    }

    // <summary>Once the Asteroid publishes the event that it hit the Playership (events are is in the Event Broker),
    // it first sets the Playership inactive, then Invokes the SpawnPlayerShip method after the respawnDelay. </summary>
    void HitByAsteroid()
    {
        gameObject.SetActive(false);
        // Here will be the particle effect would go.
        Invoke("SpawnPlayerShip", respawnDelay);
    }

    void DestroyShip()
    {
        gameObject.SetActive(false);
    }

    // First this sets a random location on the screen for the PS to respawn.
    // The while loop first checks if the PS can spawn there (first saying it's false then by calling
    // method CanShipSpawnHere). The CanShipSpawnHere gets a list of colliders from ScreenBounds and then
    // runs through all in list (all asteroids) and sees if it contains the positionForShipRespawn. If it doesn't then ship can
    // respawn.  While loops can go for infinity so there is a safetynet amount for run throughs. Lastly, transform is reset for the PS and then set back to being Active.
    // After reviewing the solution in the coursework (Jeremey's coroutine) I still liked my solution better.
    // It's easier to understand and it can be made reusable. 

    //<summary>This is the method that will Spawn the PlayerShip in a random location</summary>
    private void SpawnPlayerShip()
    {
        Vector3 positionForShipRespawn = Vector3.zero;
        bool canSpawnHere = false;
        int safetyNet = 0;

        while (!canSpawnHere)
        {
            positionForShipRespawn = ScreenBounds.RandomLocationOnScreen();
            // Can also add logic to make sure the positionForShipRespawn can't be too close to the original location
            canSpawnHere = CanShipSpawnHere(positionForShipRespawn);

            if (canSpawnHere) break;

            safetyNet++;

            if (safetyNet > 50)
            {
                Debug.LogWarning("Too many attemps to Spawn Playership.");
                break;
            }
        }
        transform.position = positionForShipRespawn;
        gameObject.SetActive(true);
    }

    // Figures out if it can spawn here based on the colliders from bounding box in ScreenBounds BoxCollider. 
    private bool CanShipSpawnHere(Vector3 spawnPosition, float distanceAway = 5)
    {
        Collider[] allColliders = ScreenBounds.ALL_COLLIDERS_IN_BOUNDS;
        for (int i = 0; i > allColliders.Length; i++)
        {
            if (allColliders[i])
            {
                // increases the collider's area by the MIN_ASTEROID_DIST_FROM_PLAYER_SHIP of where the ship can't spawn into
                Bounds boundsIncreased = new Bounds(allColliders[i].bounds.center, new Vector3(allColliders[i].bounds.size.x + distanceAway, allColliders[i].bounds.size.y + distanceAway, allColliders[i].bounds.size.z + distanceAway));
                // check to see if the new bounds contains spawn position 
                if (boundsIncreased.Contains(spawnPosition)) return false;
            }

        }
        return true;
    }



    static public float MAX_SPEED
    {
        get
        {
            return S.speed;
        }
    }

    public static Vector3 POSITION
    {
        get
        {
            return S.transform.position;
        }
    }



}