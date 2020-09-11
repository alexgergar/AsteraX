using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Level Definition")]
public class LevelDefinition_SO : ScriptableObject
{
    public string levelName;
    public int levelNumber;
    public int initialAsteroidCount;
    public int subAsteriodsToSpawn;
    public int initialAsteroidSize;
}
