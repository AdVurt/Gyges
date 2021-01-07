using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu( fileName = "Enemy Data", menuName = "Gyges/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public float startingHealth = 5f;
    public int bounty = 50;

}
