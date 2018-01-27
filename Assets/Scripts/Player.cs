using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public float shotDelay = 0.5f;
    public GameObject bullet;

    private bool cooldown = false;
    private float cooldownTimer = 0.0f;

    public List<GameColor> colorCycle;
    private GameColor playerColor;
    private SpriteRenderer sprite;

    // Use this for initialization
    void Start ()
    {
        playerColor = GameColor.COLOR_WHITE;
        sprite = gameObject.GetComponent<SpriteRenderer>();
	}

    void Update()
    {
        //Movement
        float moveHorizontal = Input.GetAxisRaw("Horizontal");
        float moveVertical = Input.GetAxisRaw("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, moveVertical, 0.0f);
        transform.Translate(movement * speed * Time.deltaTime);

        //Commands
        if (Input.GetKeyDown(KeyCode.Q))
        {
            int newIndex = colorCycle.IndexOf(playerColor) - 1;
            playerColor = colorCycle[newIndex >= 0 ? newIndex : colorCycle.Count - 1];
            SetColor();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            playerColor = colorCycle[(colorCycle.IndexOf(playerColor) + 1) % colorCycle.Count];
            SetColor();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            FireBullet();
        }

        //Timers
        if (cooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer > shotDelay)
            {
                cooldownTimer = 0.0f;
                cooldown = false;
            }
        }
    }

    void SetColor()
    {
        sprite.color = ColorDefs.GetColor(playerColor);
    }

    void FireBullet()
    {
        if( cooldown )
        {
            return;
        }

        GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
        Bullet bulletComp = bulletInstance.GetComponent<Bullet>();
        bulletComp.SetColor(playerColor);
        bulletComp.SetShooter(gameObject);
        cooldown = true;
    }
}
