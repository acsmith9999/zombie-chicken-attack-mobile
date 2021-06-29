using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FoxMove : MonoBehaviour
{
    public float moveSpeed = 7;

    public Rigidbody2D rb;

    private Vector2 playerPos;

    public GameObject Fox;
    public GameObject bullet;
    public GameObject stain;

    public Vector2 startPos, direction;

    public KeyCode bombKey;
    private bool invisible;

    private Vector2 bulletPos;
    private float fireRate = 0.1f;
    private float nextFire = 0.2f;
    public float weaponSpeed;

    public float waitToReload;

    private int lifeCount;

    public int levelAccess;

    public Controller gameLevelManager;

    public Animator animator;

    private Touch touch;
    private Joystick joystick;

    // Start is called before the first frame update
    void Start()
    {
        gameLevelManager = FindObjectOfType<Controller>();
        joystick = FindObjectOfType<Joystick>();

        rb = GetComponent<Rigidbody2D>();
        if (PlayerPrefs.HasKey("currentlives"))
        {
            lifeCount = PlayerPrefs.GetInt("currentlives");
        }
        else { PlayerPrefs.SetInt("currentlives", PlayerPrefs.GetInt("maxlives")); }
        moveSpeed = 7;
    }

    // Update is called once per frame
    void Update()
    {
        playerPos = new Vector2(joystick.Horizontal, joystick.Vertical);
        animator.SetFloat("Speed", moveSpeed);
        if (invisible == true)
        {
            animator.SetBool("Invisible", true);
        }
        else { animator.SetBool("Invisible", false); }
    }

    private void FixedUpdate()
    {

        rb.MovePosition((Vector2)transform.position + (playerPos * moveSpeed * Time.deltaTime));
        if (Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            fire();
        }

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Enemy" && invisible == false)
        {
            gameLevelManager.Respawn();
            lifeCount -= 1;
            PlayerPrefs.SetInt("currentlives", lifeCount);
            SoundManagerScript.PlaySound("FoxDeath");
            Instantiate(stain, this.transform.position, Quaternion.identity);
            fireRate = 0.1f;
            StopAllCoroutines();
            EnemyBullet[] enemyBullets = GameObject.FindObjectsOfType<EnemyBullet>();
            foreach (EnemyBullet bullet in enemyBullets)
            {
                Destroy(bullet.gameObject);
            }
        }
        if (other.gameObject.tag == "Finish")
        {
            Destroy(this.gameObject);
            SceneManager.LoadScene("Farm");
            if(PlayerPrefs.GetInt("levelaccess") == gameLevelManager.levelNumber)
            {
                PlayerPrefs.SetInt("levelaccess", gameLevelManager.levelNumber +1);
            }
        }
        if (other.gameObject.tag == "Bomb" && PlayerPrefs.GetInt("bombcount") < 3)
        {
            PlayerPrefs.SetInt("bombcount", PlayerPrefs.GetInt("bombcount") + 1);
            SoundManagerScript.PlaySound("Shotgun");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Life" && PlayerPrefs.GetInt("currentlives") < PlayerPrefs.GetInt("maxlives"))
        {
            PlayerPrefs.SetInt("currentlives", PlayerPrefs.GetInt("currentlives") + 1);
            SoundManagerScript.PlaySound("Life");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Shoes")
        {
            StartCoroutine(RunningShoes());
            SoundManagerScript.PlaySound("RunningShoes");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Invisibility")
        {
            StartCoroutine(InvisibilityPotion());
            SoundManagerScript.PlaySound("Invisibility");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Gun")
        {
            StartCoroutine(RapidFire());
            SoundManagerScript.PlaySound("RapidFire");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "EnemyBullet" && invisible == false)
        {
            lifeCount -= 1;
            PlayerPrefs.SetInt("currentlives", lifeCount);
            gameLevelManager.Respawn();
            SoundManagerScript.PlaySound("FoxDeath");
            Instantiate(stain, this.transform.position, Quaternion.identity);
            fireRate = 0.1f;
            StopAllCoroutines();
            EnemyBullet[] enemyBullets = GameObject.FindObjectsOfType<EnemyBullet>();
            foreach (EnemyBullet bullet in enemyBullets)
            {
                Destroy(bullet.gameObject);
            }
        }
        if (other.gameObject.tag == "Money")
        {
            GoldScript[] gold = GameObject.FindObjectsOfType<GoldScript>();
            foreach (GoldScript coin in gold)
            {
                coin.magnetic = true;
            }
            SoundManagerScript.PlaySound("MoneyMagnet");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "Egg" && invisible == false)
        {
            lifeCount -= 1;
            PlayerPrefs.SetInt("currentlives", lifeCount);
            gameLevelManager.Respawn();
            SoundManagerScript.PlaySound("FoxDeath");
            SoundManagerScript.PlaySound("Explosion");
            Instantiate(stain, this.transform.position, Quaternion.identity);
            fireRate = 0.1f;
            StopAllCoroutines();
            Destroy(this.gameObject);
            EnemyBullet[] enemyBullets = GameObject.FindObjectsOfType<EnemyBullet>();
            foreach (EnemyBullet bullet in enemyBullets)
            {
                Destroy(bullet.gameObject);
            }
        }
        if (other.gameObject.tag == "Farmer" && invisible == false)
        {
            lifeCount -= 1;
            PlayerPrefs.SetInt("currentlives", lifeCount);
            gameLevelManager.Respawn();
            SoundManagerScript.PlaySound("FoxDeath");
            Instantiate(stain, this.transform.position, Quaternion.identity);
            fireRate = 0.1f;
            StopAllCoroutines();
            EnemyBullet[] enemyBullets = GameObject.FindObjectsOfType<EnemyBullet>();
            foreach (EnemyBullet bullet in enemyBullets)
            {
                Destroy(bullet.gameObject);
            }
        }
    }

    public IEnumerator RunningShoes()
    {
        moveSpeed += 3;
        yield return new WaitForSeconds(5);
        moveSpeed -= 3;
    }

    public IEnumerator InvisibilityPotion()
    {
        invisible = true;
        yield return new WaitForSeconds(5);
        invisible = false;
    }

    public IEnumerator RapidFire()
    {
        fireRate = fireRate / 2;
        yield return new WaitForSeconds(5);
        fireRate = fireRate * 2;
    }

    void fire()
    {
        bulletPos = transform.position;

        if (joystick.Horizontal < -0.5f)
            {
                bulletPos += new Vector2(-1.5f, 0);
                Instantiate(bullet, bulletPos, Quaternion.identity);
            }

            else if (joystick.Vertical < -0.5f)
            {
                bulletPos += new Vector2(0, -1.5f);
                Instantiate(bullet, bulletPos, Quaternion.identity);
            
            }

            else if (joystick.Vertical > 0.5f)
            {
                bulletPos += new Vector2(0, 1.5f);
                Instantiate(bullet, bulletPos, Quaternion.identity);
            }

            else if (joystick.Horizontal > 0.5f)
            {
                bulletPos += new Vector2(1.5f, 0);
                Instantiate(bullet, bulletPos, Quaternion.identity);
            }

    }

}
