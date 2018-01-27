using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Splitter : MonoBehaviour
{
    public GameObject bullet;
    public float shotDelay = 0.5f;

    private bool willFireNextFrame = false;
    private bool isCharged = false;
    private bool cooldown = false;
    private float cooldownTimer = 0.0f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private GameColor splitterColor;
    private SpriteRenderer sprite;

    // Use this for initialization
    void Start ()
    {
        sprite = GetComponent<SpriteRenderer>();
        splitterColor = GameColor.COLOR_WHITE;
    }
	
	// Update is called once per frame
	void Update ()
    {
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
            willFireNextFrame = true;
        }
    }

    void FixedUpdate()
    {
        if (willFireNextFrame)
        {
            FireBullet();
            willFireNextFrame = false;
        }

        Collider2D[] results = new Collider2D[10];
        int resultCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D(), results);
        for (int i = 0; i < resultCount; i++)
        {
            Bullet bulletHit = results[i].gameObject.GetComponent<Bullet>();
            if (bulletHit != null && bulletHit.GetShooter() != gameObject)
            {
                ProcessBullet(bulletHit);
            }
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

        GameObject bulletInstance_1 = Instantiate(bullet, transform.position, Quaternion.AngleAxis(transform.rotation.eulerAngles.z + 45.0f, Vector3.forward));
        GameObject bulletInstance_2 = Instantiate(bullet, transform.position, Quaternion.AngleAxis(transform.rotation.eulerAngles.z - 45.0f, Vector3.forward));

        Bullet bulletComp_1 = bulletInstance_1.GetComponent<Bullet>();
        bulletComp_1.SetColor(splitterColor);
        bulletComp_1.SetShooter(gameObject);

        Bullet bulletComp_2 = bulletInstance_2.GetComponent<Bullet>();
        bulletComp_2.SetColor(splitterColor);
        bulletComp_2.SetShooter(gameObject);

        cooldown = true;
    }

    void ProcessBullet(Bullet lastBullet)
    {
        splitterColor = lastBullet.GetColor();
        sprite.color = ColorDefs.GetColor(splitterColor);
        isCharged = (splitterColor != GameColor.COLOR_WHITE);
        Destroy(lastBullet.gameObject);
    }
}
