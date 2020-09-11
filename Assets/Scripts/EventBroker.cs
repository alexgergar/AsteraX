using System;
using System.Collections;
using System.Security.Policy;
using UnityEngine;

/// <summary>
/// Event Broker will hold all of the actions/events for this game, which uses the Publisher/Subscriber pattern.
/// <para>If this game was larger, I would break the EventBroker into unique scripts
///  for specific areas of the game. However, since this game is realitively small I have
///  only one EventBroker to hold all events. Each section then is broken up by region based on the publisher.</para>
/// </summary>
public class EventBroker
{
    #region Asteroid 
    public static event Action<int> BulletHitsAsteroid;
    public static void CallBulletHitsAsteroid(int asteroidSize)
    {
        if (BulletHitsAsteroid != null)
        {
            BulletHitsAsteroid(asteroidSize);
        }
    }

    public static event Action ShipHitAsteroid;
    public static void CallShipHitAsteroid()
    {
        if (ShipHitAsteroid != null)
        {
            ShipHitAsteroid();
        }
    }

    #endregion



    #region AsteraX

    public static event Action<int> UpdateScore;
    public static void CallUpdateScore(int totalScore)
    {
        if (UpdateScore != null)
        {
            UpdateScore(totalScore);
        }
    }

    public static event Action<int> UpdateJumps;
    public static void CallUpdateJumps(int totalJumpsLeft)
    {
        if (UpdateJumps != null)
        {
            UpdateJumps(totalJumpsLeft);
        }
    }

    public static event Action RespawnShip;
    public static void CallRespawnShip()
    {
        if (RespawnShip != null)
        {
            RespawnShip();
        }
    }


    public static event Action GameOver;
    public static void CallGameOver()
    {
        if (GameOver != null)
        {
            GameOver();
        }
    }
    #endregion

    #region
    public static event Action GameStateChange;
    public static void CallGameStateChange()
    {
        if (GameStateChange != null)
        {
            GameStateChange();
        }
    }
    #endregion
}
