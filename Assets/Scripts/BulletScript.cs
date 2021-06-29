using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public float velX;
    public float velY;
    Rigidbody2D rb;
    Joystick joystick;
    FoxMove player;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        joystick = GameObject.FindObjectOfType<Joystick>();
        player = GameObject.FindObjectOfType<FoxMove>();
        velX = (float)joystick.Horizontal * player.weaponSpeed;
        velY = (float)joystick.Vertical * player.weaponSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        rb.velocity = new Vector2(velX, velY);
        Destroy(gameObject, 2f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            Destroy(this.gameObject);
        }
    }

}
