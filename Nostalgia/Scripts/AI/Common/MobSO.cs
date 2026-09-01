using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SO_", menuName = "Scriptable Object/Mob", order = 0)]
public class MobSO : ScriptableObject
{
    [Header("State Speeds")] 
    public float[] StateSpeeds = { 2f, 3f, 5f };
    
    [Header("Observing")]
    public float ViewRadius = 1.5f;
    public float ViewAngle = 60f;
    
    [Header("Attack")]
    public float AttackCoolTime = 0f;
    public int AttackDamage = 100;
    
    [Header("Layer Mask")]
    public LayerMask PlayerLayerMask;
    public LayerMask ObstacleLayerMask;
}
