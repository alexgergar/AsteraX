using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(OffScreenWrapper))]
public class AsteroidController : MonoBehaviour
{

    public int asteroidSize = 0;
    private float asteroidVelocity;

    private Rigidbody rigid;
    private OffScreenWrapper offScreenWrapper;

    public static Action<float> OnAsteriodHitSize;
    public static Action<GameObject> OnAsteroidHitBullet;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        offScreenWrapper = GetComponent<OffScreenWrapper>();
    }

    // Use this for initialization
    void Start()
    {
        if (asteroidSize == 0) return;

        AsteraX.AddAsteroid(this);

        transform.localScale = Vector3.one * asteroidSize * AsteraX.AsteroidsSO.asteroidScale;

        if (transform.parent == null)
        {
            InitializeParentAsteroid();
        }
        else
        {
            InitializeChildAsteroid();
        }

        if (asteroidSize > 1)
        {
            for (int i = 0; i < AsteraX.SubAsteroidsToSpawn; i++)
            {
                SpawnChildAsteroid(asteroidSize - 1, i);
            }
        }

        EventBroker.GameOver += DestroyMe;
    }

    private void OnDestroy()
    {
        AsteraX.RemoveAsteroid(this);
        EventBroker.GameOver -= DestroyMe;
    }

    private void DestroyMe()
    {
        Destroy(gameObject);
    }


    private void InitializeParentAsteroid()
    {
        offScreenWrapper.enabled = true;
        // this moves the asteroid based on physics
        rigid.isKinematic = false;
        // makes sure the z position is at 0
        Vector3 position = transform.position;

        position.z = 0;
        transform.position = position;
        InitVelocityOfAsteroid();

    }

    private void InitializeChildAsteroid()
    {
        // if child asteroid, then it shouldn't bump into it's sibling asteroids
        rigid.isKinematic = true;
        offScreenWrapper.enabled = false;
        // this makes sure the scale is properly set up (via the parent)
        transform.localScale = transform.localScale.ComponentDivide(transform.parent.lossyScale);
    }


    private void InitVelocityOfAsteroid()
    {
        Vector3 velocity;

        //TODO - if asteroids get lost off screen will need to add ScreenBound check here.


        // this will pick a random direction to go to in a circle, then normalize it (so doesn't go faster on diagionals)
        // the conditional then make sure you don't get (0,0,0) for the random.indsideunitcircle since it could happen
        do
        {
            velocity = Random.insideUnitCircle;
            velocity.Normalize();
        } while (Mathf.Approximately(velocity.magnitude, 0f));

        // finding the random range for velocity, multiple by the random direction from above / the size of asteroid
        float velocityRange = Random.Range(AsteraX.AsteroidsSO.minVelocity, AsteraX.AsteroidsSO.maxVelocity);
        velocity = velocity * velocityRange / asteroidSize;


        rigid.velocity = velocity;
        rigid.angularVelocity = Random.insideUnitSphere * AsteraX.AsteroidsSO.maxVelocity;

    }

    private void SpawnChildAsteroid(int sizeOfAsteroid, int index)
    {
        AsteroidController childAsteroid = SpawnAsteroid();
        childAsteroid.asteroidSize = sizeOfAsteroid;
        childAsteroid.transform.SetParent(transform);
        childAsteroid.transform.rotation = Random.rotation;
        Vector3 randomLocalPosition = Random.onUnitSphere / 2;
        childAsteroid.transform.localPosition = randomLocalPosition;

        childAsteroid.gameObject.name = gameObject.name + "_" + index.ToString();

    }

    private void OnCollisionEnter(Collision collision)
    {
        // if this GO is a child of a asteroid, it will pass the collision up the chain
        if (parentOfAsteroid != null)
        {
            parentOfAsteroid.OnCollisionEnter(collision);
            return;
        }

        GameObject collidedGO = collision.gameObject;

        if (collidedGO.tag == "Bullet" || collidedGO.transform.root.gameObject.tag == "Player")
        {
            if (collidedGO.tag == "Bullet")
            {
                // if the collision was caused by bullet - destroy the bullet
                EventBroker.CallBulletHitsAsteroid(asteroidSize);
                Destroy(collidedGO);
            }

            if (collidedGO.tag == "Player")
            {
                // This makes sure that if another asteroid collided with PS within a second nothing would happen.
                if (Time.time < PlayerShipController.LAST_COLLISION + PlayerShipController.COLLISION_DELAY)
                {
                    return;
                }
                else
                {
                    PlayerShipController.LAST_COLLISION = Time.time;
                }
                EventBroker.CallShipHitAsteroid();
            }


            if (asteroidSize > 1)
            {

                AsteroidController[] childrenOfParentAsteroid = GetComponentsInChildren<AsteroidController>();
                for (int i = 0; i < childrenOfParentAsteroid.Length; i++)
                {
                    if (childrenOfParentAsteroid[i].transform.parent == null ||
                        childrenOfParentAsteroid[i].transform.parent != transform)
                    {
                        // continue stops the for loop cycle - so only direct children will get to the below
                        continue;
                    }

                    childrenOfParentAsteroid[i].transform.SetParent(null, true);
                    childrenOfParentAsteroid[i].InitializeParentAsteroid();
                }
            }

            Destroy(gameObject);
        }

    }

    // finding parentOfAsteriod
    private AsteroidController parentOfAsteroid
    {
        get
        {
            if (transform.parent != null)
            {
                AsteroidController parentOfAsteroid = transform.parent.GetComponent<AsteroidController>();
                if (parentOfAsteroid != null)
                {
                    return parentOfAsteroid;
                }
            }
            return null;
        }
    }


    public static AsteroidController SpawnAsteroid()
    {
        AsteroidController asteroidPrefab = AsteraX.AsteroidsSO.GetRandomModel();
        AsteroidController asteroid = Instantiate(asteroidPrefab);
        return asteroid;
    }

}
