using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Emitter : MonoBehaviour
{
    public bool isCharged = false;
    public float shotDelay = 0.5f;
    public GameObject bullet;
    public GameColor emitterColor;
    public GameObject firingPoint;

    public List<MeshRenderer> baseMaterials;
    public MeshRenderer bulbRenderer;
    public Facing emitterFacing;

    private bool willFireNextFrame = false;
    private bool chargeToggle = false;
    private bool cooldown = false;

    private int collisionCount = 0;
    private float collisionTimer = 0.0f;
    private float cooldownTimer = 0.0f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private GameColor emitterCharge_1;
    private GameColor emitterCharge_2;

    // Use this for initialization
    void Start ()
    {
       SetEmitterColor(ColorDefs.GetColor(emitterColor));
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            collisionTimer += Time.deltaTime;
            if( collisionTimer > 0.5f)
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

    private void PushBlock(int numContacts, ContactPoint2D[] contacts )
    {
        Vector3 thisPos = transform.position;
        Vector3 otherPos = contacts[0].collider.transform.position;
        if (Mathf.Abs(thisPos.x - otherPos.x) > Mathf.Abs(thisPos.y - otherPos.y))
        {
            if (thisPos.x > otherPos.x)
            {
                //push right
                transform.Translate(5.0f, 0.0f, 0.0f);
            }
            else
            {
                //push left
                transform.Translate(-5.0f, 0.0f, 0.0f);
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

    void FixedUpdate()
    {
        if (willFireNextFrame)
        {
            FireBullet();
            willFireNextFrame = false;
        }

        Collider2D[] results = new Collider2D[5];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(1 << LayerMask.NameToLayer("Bullet"));
        int resultCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), filter, results);
        if( resultCount > 0)
        {
            Collider2D collider = Array.Find(results, (x => x != null && x.GetComponent<Bullet>().GetShooter() != gameObject));
            if (collider)
            {
                ProcessBullet(collider.GetComponent<Bullet>());
            }
        }
    }

    void FireBullet()
    {
        if (cooldown)
        {
            return;
        }

        Quaternion rotation = new Quaternion();
        switch (emitterFacing)
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
        GameObject bulletInstance = Instantiate(bullet, firingPoint.transform.position, rotation);

        Bullet bulletComp = bulletInstance.GetComponent<Bullet>();
        bulletComp.SetColor(emitterColor);
        bulletComp.SetShooter(gameObject);
        cooldown = true;
    }

    void SetEmitterColor(Color newColor)
    {
        bulbRenderer.material.color = new Color(newColor.r, newColor.g, newColor.b, 0.0f);
        foreach(MeshRenderer render in baseMaterials)
        {
            render.material.color = newColor;
        }
    }

    void ProcessBullet(Bullet bulletHit)
    {
        if( chargeToggle )
        {
            emitterCharge_1 = bulletHit.GetColor();
            chargeToggle = !chargeToggle;
        }
        else
        {
            emitterCharge_2 = bulletHit.GetColor();
            chargeToggle = !chargeToggle;
        }

        if (emitterCharge_1 != GameColor.COLOR_WHITE && emitterCharge_2 != GameColor.COLOR_WHITE)
        {
            GameColor color = ColorDefs.CombineColors(emitterCharge_1, emitterCharge_2);
            emitterColor = color;
            SetEmitterColor(ColorDefs.GetColor(emitterColor));

            isCharged = (emitterColor != GameColor.COLOR_WHITE);
            emitterCharge_1 = GameColor.COLOR_WHITE;
            emitterCharge_2 = GameColor.COLOR_WHITE;
        }

        bulletHit.RemoveBullet();
    }
}
