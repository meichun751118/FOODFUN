﻿using UnityEngine;
using UnityEngine.AI;       // 引用 人工智慧 API
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("怪物資料")]
    public EnemyData data;                   // 一般欄位才可以獨立使用

    private Rigidbody2D rig;
    private float hp;
    private Animator ani;                    // 動畫控制器
    private Transform target;                // 目標變形
    private float timer;                     // 計時器
    private HpValueManager hpValueManager;   // 血條數值管理器
    public bool Wating;                      // 是否正在等人

    [Header("走路速度")]
    public float speed;

    public bool dead;

    private void Start()
    {
        hp = data.hpMax;
        ani = GetComponent<Animator>();
        target = GameObject.Find("玩家").transform;                   // 目標 = 尋找
        hpValueManager = GetComponentInChildren<HpValueManager>();    // 取得子物件元件
        rig = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Move();     // 呼叫移動方法
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        if (Wating) return;
            StartCoroutine(WalkDelay());
            ani.SetBool("走路開關", true);      // 走路
            transform.Translate(-speed * Time.deltaTime, 0, 0);
    }

    /// <summary>
    /// 走路延遲
    /// </summary>
    /// <returns></returns>
    private IEnumerator WalkDelay()
    {
        yield return new WaitForSeconds(data.WalkDelay);
    }

    /// <summary>
    /// 等待
    /// </summary>
    public void Wait()
    {
        Wating = true;                          //當開始等待 Wating=true
        ani.SetBool("走路開關", false);         // 等待動畫
        timer += Time.deltaTime;                // 計時器累加
        
        if (timer >= data.cd)                   // 如果 計時器 >= 資料.冷卻
        {
            Attack();                           // 攻擊
        }
    }

    // protected 保護 : 允許子類別存取，禁止外部類別存取
    // virtual 虛擬 : 允許子類別複寫
    /// <summary>
    /// 攻擊
    /// </summary>
    protected virtual void Attack()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.up * data.attackY, -transform.right, data.attackLength, 512);

        if (hit)
        {
            if (timer < data.cd)                // 如果 計時器 < 冷卻時間
            {
                timer += Time.deltaTime;        // 計時器 累加
            }
            else
            {
                timer = 0;                      // 計時器 歸零
                ani.SetTrigger("攻擊開關");     // 攻擊動畫
                hit.collider.GetComponent<Pet>().Hit(data.attack);
            }
        }
    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">接收玩家給予的傷害值</param>
    public void Hit(float damage)
    {
        if (ani.GetBool("死亡開關")) return;
        StartCoroutine(AttackDelay());
        hp -= damage;
        hpValueManager.SetHP(hp, data.hpMax);      // 更新血量(目前，最大)
        StartCoroutine(hpValueManager.ShowValue(damage, "-", Color.white));
        if (hp <= 0) Dead();
    }

    /// <summary>
    /// 受傷退後
    /// </summary>
    public void HurtBack()
    {
        rig.AddForce(new Vector2(3, 2));
    }

    /// <summary>
    /// 攻擊延遲
    /// </summary>
    /// <returns></returns>
    private IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(data.attackDelay);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        dead = true;
        gameObject.layer = 0;
        ani.SetBool("死亡開關", true);      // 死亡動畫
        Destroy(this);                      // Destroy(GetComponent<元件>()); 刪除元件
        Destroy(gameObject, 3f);
        CreateCoin();
    }


    [Header("金幣")]
    public GameObject coin;

    /// <summary>
    /// 掉落金幣
    /// </summary>
    private void CreateCoin()
    {
        // (int) 強制轉型 - 強制將浮點數轉為整數，去小數點
        int r = (int)Random.Range(data.coinRange.x, data.coinRange.y);

        for (int i = 0; i < r; i++)
        {
            Instantiate(coin, transform.position + transform.up * (-1.5f), transform.rotation);
        }
    }

    public GameObject tempEnemy;

    /// <summary>
    /// 碰撞器後等待
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "我方" && collision.GetComponent<Pet>())
        {

            if (collision.GetComponent<Pet>().dead) Wating = false;

            if (collision.GetType().Equals(typeof(CapsuleCollider2D)))
            {
                tempEnemy = collision.gameObject;

                Wait();
            }
        }

        else if (collision.tag == "我方" && collision.GetComponent<Player>())
        {

            if (collision.GetComponent<Player>().dead) Wating = false;

            if (collision.GetType().Equals(typeof(CapsuleCollider2D)))
            {
                tempEnemy = collision.gameObject;

                Wait();
            }
        }
    }
}
