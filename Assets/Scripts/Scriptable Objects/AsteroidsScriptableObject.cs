using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Asteriod Models", menuName = "Asteroid Base")]
public class AsteroidsScriptableObject : ScriptableObject
{
    public float minVelocity = 5f;
    public float maxVelocity = 10f;
    public float asteroidScale = 0.75f;
    public int[] pointsForAsteroidSize = { 0, 400, 200, 100 };

    public List<AsteroidController> asteroidModelList;


    public AsteroidController GetRandomModel()
    {
        return asteroidModelList[Random.Range(0, asteroidModelList.Count)];
    }

}
