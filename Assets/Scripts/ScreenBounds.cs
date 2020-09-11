using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenBounds : MonoBehaviour
{
    private static ScreenBounds S; // private but unprotected Singleton

    private static Vector3 bounds;

    public static float left { get { return -bounds.x; } }
    public static float right { get { return bounds.x; } }
    public static float top { get { return bounds.y; } }
    public static float bottom { get { return -bounds.y; } }


    // Use this for initialization
    void Awake()
    {
        S = this;
        bounds = GetScreenBounds();
    }

    private static Vector3 GetScreenBounds()
    {

        Vector3 screenVector = new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z);

        return Camera.main.ScreenToWorldPoint(screenVector);
    }

    public static bool OutOfBounds(Vector2 position)
    {
        float x = Mathf.Abs(position.x);
        float y = Mathf.Abs(position.y);
        return (x > bounds.x || y > bounds.y);
    }

    public static bool IsXOutOfBounds(Vector2 position)
    {
        float x = Mathf.Abs(position.x);
        return (x > bounds.x);
    }

    public static bool IsYOutOfBounds(Vector2 position)
    {
        float y = Mathf.Abs(position.y);
        return (y > bounds.y);
    }

    public static Vector3 RandomLocationOnScreen()
    {
        float xRange = Random.Range(-bounds.x, bounds.x);
        float yRange = Random.Range(-bounds.y, bounds.y);
        return new Vector3(xRange, yRange, 0);
    }

    public static Vector3 OutOfBoundsFromWorldPos(Vector3 worldPos)
    {
        Vector3 localPosition = S.transform.InverseTransformPoint(worldPos);
        return localPosition;
    }

    // this will get all the GameObjects with colliders in the bounding box and then return the array of colliders
    static public Collider[] ALL_COLLIDERS_IN_BOUNDS
    {
        get
        {
            return Physics.OverlapBox(S.transform.position, S.transform.localScale);
        }
    }
}
