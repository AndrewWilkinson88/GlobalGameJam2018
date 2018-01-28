using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Splitter : MonoBehaviour
{
    public GameObject bullet;
    public float shotDelay = 0.5f;

    public List<MeshRenderer> baseMaterials;
    public List<MeshRenderer> bulbRenderers;

    public Facing splitterFace1;
    public Facing splitterFace2;

    private bool willFireNextFrame = false;
    private bool isCharged = false;
    private bool cooldown = false;
    private float cooldownTimer = 0.0f;
    private float collisionTimer = 0.0f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private GameColor splitterColor;

    // Use this for initialization
    void Start ()
    {
        splitterColor = GameColor.COLOR_WHITE;
        SetEmitterColor(Color.grey);
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collisionTimer += Time.deltaTime;
            if (collisionTimer > 0.5f)
            {
                ContactPoint2D[] contacts = new ContactPoint2D[5];
                int numContacts = collision.GetContacts(contacts);
                PushBlock(numContacts, contacts);
                collisionTimer = 0.0f;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collisionTimer = 0;
        }
    }

    private void PushBlock(int numContacts, ContactPoint2D[] contacts)
    {
        Vector3 thisPos = transform.position;
        Vector3 otherPos = contacts[0].collider.transform.position;
        if (Mathf.Abs(thisPos.x - otherPos.x) > Mathf.Abs(thisPos.y - otherPos.y))
        {
            if (thisPos.x > otherPos.x)
            {
                //push right
                transform.Translate(-5.0f, 0.0f, 0.0f);
            }
            else
            {
                //push left
                transform.Translate(5.0f, 0.0f, 0.0f);
            }
        }
        else
        {
            if (thisPos.y > otherPos.y)
            {
                //push up
                transform.Translate(0.0f, 5.0f, 0.0f);
            }
            else
            {
                //push down
                transform.Translate(0.0f, -5.0f, 0.0f);
            }
        }

    }

    Quaternion GetRotation(Facing face)
    {
        Quaternion rotation = new Quaternion();
        switch (face)
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
            case (Facing.LEFT_UP):
                rotation = Quaternion.AngleAxis(45.0f, Vector3.forward);
                break;
            case (Facing.RIGHT_UP):
                rotation = Quaternion.AngleAxis(-45.0f, Vector3.forward);
                break;
            case (Facing.LEFT_DOWN):
                rotation = Quaternion.AngleAxis(135.0f, Vector3.forward);
                break;
            case (Facing.RIGHT_DOWN):
                rotation = Quaternion.AngleAxis(-135.0f, Vector3.forward);
                break;
        }

        return rotation;
    }

    void FireBullet()
    {
        if (cooldown)
        {
            return;
        }

        GameObject bulletInstance_1 = Instantiate(bullet, transform.position, GetRotation(splitterFace1) );
        GameObject bulletInstance_2 = Instantiate(bullet, transform.position, GetRotation(splitterFace2) );

        Bullet bulletComp_1 = bulletInstance_1.GetComponent<Bullet>();
        bulletComp_1.SetColor(splitterColor);
        bulletComp_1.SetShooter(gameObject);

        Bullet bulletComp_2 = bulletInstance_2.GetComponent<Bullet>();
        bulletComp_2.SetColor(splitterColor);
        bulletComp_2.SetShooter(gameObject);

        cooldown = true;
        isCharged = false;
        splitterColor = GameColor.COLOR_WHITE;
        SetEmitterColor(Color.grey);
    }

    void ProcessBullet(Bullet lastBullet)
    {
        splitterColor = lastBullet.GetColor();
        isCharged = (splitterColor != GameColor.COLOR_WHITE);
        SetEmitterColor(ColorDefs.GetColor(splitterColor));
        Destroy(lastBullet.gameObject);
    }

    void SetEmitterColor(Color newColor)
    {
        foreach (MeshRenderer render in bulbRenderers)
        {
            render.material.color = new Color(newColor.r, newColor.g, newColor.b, 0.0f);
        }

        foreach (MeshRenderer render in baseMaterials)
        {
            render.material.color = newColor;
        }
    }
}
