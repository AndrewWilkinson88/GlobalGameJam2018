using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Receiver : MonoBehaviour
{
    public GameObject bullet;
    public float shotDelay = 0.5f;

    private bool isCharged = false;
    private bool cooldown = false;
    private float cooldownTimer = 0.0f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private GameColor receiverColor;
    private SpriteRenderer sprite;

    // Use this for initialization
    void Start ()
    {
        sprite = GetComponent<SpriteRenderer>();
        receiverColor = GameColor.COLOR_WHITE;
    }
	
	// Update is called once per frame
	void Update ()
    {
        Collider2D[] results = new Collider2D[10];
        int resultCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D(), results);
        for( int i = 0; i < resultCount; i++ )
        {
            Bullet bulletHit = results[i].gameObject.GetComponent<Bullet>();
            if( bulletHit != null && bulletHit.GetShooter() != gameObject)
            {
                ProcessBullet(bulletHit);
            }
        }

        if (cooldown)
        {
            cooldownTimer += Time.deltaTime;
            if (cooldownTimer > shotDelay)
            {
                cooldownTimer = 0.0f;
                cooldown = false;
            }
        }
        else if (isCharged)
        {
            FireBullet();
        }
    }

     void OnMouseDown()
     {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void FireBullet()
    {
        if (cooldown)
        {
            return;
        }

        GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
        Bullet bulletComp = bulletInstance.GetComponent<Bullet>();
        bulletComp.SetColor(receiverColor);
        bulletComp.SetShooter(gameObject);
        cooldown = true;
    }

    void ProcessBullet(Bullet lastBullet)
    {
        GameColor color = ColorDefs.CombineColors(receiverColor, lastBullet.GetColor());
        receiverColor = color;
        sprite.color = ColorDefs.GetColor(color);
        isCharged = (receiverColor != GameColor.COLOR_WHITE);

        Destroy(lastBullet.gameObject);
    }
}
