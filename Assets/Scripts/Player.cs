using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Player : MonoBehaviour
{
    public float speed = 5.0f;
    public float shotDelay = 0.5f;
    public GameObject bullet;
    public GameObject gunTip;

    private bool cooldown = false;
    private float cooldownTimer = 0.0f;

    public Facing playerFacing;
    public Sprite LeftSprite;
    public Sprite RightSprite;
    public Sprite UpSprite;
    public Sprite DownSprite;

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
        if (Mathf.Abs(moveHorizontal) + Mathf.Abs(moveVertical) > 0.1f)
        {
            transform.Translate(movement * speed * Time.deltaTime);
            DetermineFacing(movement);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (playerColor != GameColor.COLOR_WHITE)
            {
                FireBullet();
            }
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

    void FixedUpdate()
    {
        Collider2D[] results = new Collider2D[5];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(1 << LayerMask.NameToLayer("Bullet"));
        int resultCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, results);
        if (resultCount > 0)
        {
            for (int i = 0; i < resultCount; i++)
            {
                Bullet currentBullet = results[i].GetComponent<Bullet>();
                if (currentBullet.GetShooter() != gameObject)
                {
                    GameColor color = currentBullet.GetColor();
                    playerColor = color;
                    SetColor();
                    currentBullet.RemoveBullet();
                }
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

        switch( playerFacing)
        {
            case (Facing.LEFT):
                sprite.sprite = LeftSprite;
                break;
            case (Facing.RIGHT):
                sprite.sprite = RightSprite;
                break;
            case (Facing.UP):
                sprite.sprite = UpSprite;
                break;
            case (Facing.DOWN):
                sprite.sprite = DownSprite;
                break;
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
                rotation = Quaternion.AngleAxis(90.0f, Vector3.forward);
                break;
            case (Facing.RIGHT):
                rotation = Quaternion.AngleAxis(-90.0f, Vector3.forward);
                break;
            case (Facing.UP):
                rotation = Quaternion.AngleAxis(0.0f, Vector3.forward);
                break;
            case (Facing.DOWN):
                rotation = Quaternion.AngleAxis(180.0f, Vector3.forward);
                break;
        }

        GameObject bulletInstance = Instantiate(bullet, gunTip.transform.position, rotation);
        Bullet bulletComp = bulletInstance.GetComponent<Bullet>();
        bulletComp.SetColor(playerColor);
        bulletComp.SetShooter(gameObject);
        cooldown = true;
    }
}
