﻿using UnityEngine;

// ScriptableObject 腳本化物件 : 將腳本的資料存放在專案內 (不需要掛在物件上)

[CreateAssetMenu(fileName = "怪物資料", menuName = "RED/怪物資料")]
public class EnemyData : ScriptableObject
{
    [Header("移動速度"), Range(0, 300)]
    public float speed;
    [Header("血量"), Range(100, 50000)]
    public float hp;
    public float hpMax;
    [Header("攻擊力"), Range(10, 10000)]
    public float attack;
    [Header("冷卻時間"), Range(1, 10)]
    public float cd;
    [Header("停止距離"), Range(0.5f, 100)]
    public float stopDistance;
    [Header("走路延遲")]
    public float WalkDelay;
    [Header("近距離單位")]
    public float attackY;
    public float attackLength;
    public float attackDelay;
    [Header("金幣隨機範圍")]
    public Vector2 coinRange;
}
