using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseBomb : MonoBehaviour
{
    public GameObject bombButton;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Bomb()
    {
        if (PlayerPrefs.GetInt("bombcount") > 0)
        {
            SoundManagerScript.PlaySound("Explosion");
            PlayerPrefs.SetInt("bombcount", PlayerPrefs.GetInt("bombcount") - 1);
            EnemyMove[] enemies = GameObject.FindObjectsOfType<EnemyMove>();
            foreach (EnemyMove enemy in enemies)
            {
                enemy.hitPoints -= 50;
                if (enemy.hitPoints < 1)
                {
                    enemy.ChickenDeath();
                }
            }
            EnemyBullet[] enemyBullets = GameObject.FindObjectsOfType<EnemyBullet>();
            foreach (EnemyBullet bullet in enemyBullets)
            {
                Destroy(bullet.gameObject);
            }
        }
    }
}
