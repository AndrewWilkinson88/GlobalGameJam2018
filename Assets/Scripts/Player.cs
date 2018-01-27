using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Player : MonoBehaviour
{
    enum Facing
    {
        LEFT = 0,
        RIGHT,
        UP,
        DOWN
    }


    public float speed = 5.0f;
    public float shotDelay = 0.5f;
    public GameObject bullet;
    public List<GameColor> colorCycle;

    private bool cooldown = false;
    private float cooldownTimer = 0.0f;

    private Facing playerFacing;
    private GameColor playerColor;
    private SpriteRenderer sprite;

    // Use this for initialization
    void Start ()
    {
        playerFacing = Facing.RIGHT;
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
        DetermineFacing(movement);

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

    void DetermineFacing(Vector3 movement)
    {
        if( Mathf.Abs(movement.x) > Mathf.Abs(movement.y) )
        {
            playerFacing = (movement.x < 0) ? Facing.LEFT : Facing.RIGHT;
        }
        else
        {
            playerFacing = (movement.y < 0) ? Facing.DOWN : Facing.UP;
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

        Quaternion rotation = new Quaternion();
        switch( playerFacing )
        {
            case (Facing.LEFT):
                rotation = Quaternion.AngleAxis(-90.0f, Vector3.forward);
                break;
            case (Facing.RIGHT):
                rotation = Quaternion.AngleAxis(90.0f, Vector3.forward);
                break;
            case (Facing.UP):
                rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
                break;
            case (Facing.DOWN):
                rotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
                break;
        }

        GameObject bulletInstance = Instantiate(bullet, transform.position, rotation);
        Bullet bulletComp = bulletInstance.GetComponent<Bullet>();
        bulletComp.SetColor(playerColor);
        bulletComp.SetShooter(gameObject);
        cooldown = true;
    }
}
