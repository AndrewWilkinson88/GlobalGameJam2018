﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Receiver : MonoBehaviour
{
    public GameObject bullet;
    public float shotDelay = 0.5f;

    private bool willFireNextFrame = false;
    private bool chargeToggle = false;
    private bool isCharged = false;
    private bool cooldown = false;
    private float cooldownTimer = 0.0f;

    private Vector3 screenPoint;
    private Vector3 offset;

    private GameColor receiverCharge_1;
    private GameColor receiverCharge_2;
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

        Collider2D[] results = new Collider2D[5];
        int resultCount = Physics2D.OverlapCollider(GetComponent<Collider2D>(), new ContactFilter2D(), results);
        if( resultCount > 0)
        {
            Collider2D collider = Array.Find(results, (x => x != null && x.GetComponent<Bullet>() != null && x.GetComponent<Bullet>().GetShooter() != gameObject));
            if (collider)
            {
                ProcessBullet(collider.GetComponent<Bullet>());
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

        GameObject bulletInstance = Instantiate(bullet, transform.position, transform.rotation);
        Bullet bulletComp = bulletInstance.GetComponent<Bullet>();
        bulletComp.SetColor(receiverColor);
        bulletComp.SetShooter(gameObject);
        cooldown = true;
    }

    void ProcessBullet(Bullet bulletHit)
    {
        if( chargeToggle )
        {
            receiverCharge_1 = bulletHit.GetColor();
            chargeToggle = !chargeToggle;
        }
        else
        {
            receiverCharge_2 = bulletHit.GetColor();
            chargeToggle = !chargeToggle;
        }

        if (receiverCharge_1 != GameColor.COLOR_WHITE && receiverCharge_2 != GameColor.COLOR_WHITE)
        {
            GameColor color = ColorDefs.CombineColors(receiverCharge_1, receiverCharge_2);
            receiverColor = color;
            sprite.color = ColorDefs.GetColor(color);
            isCharged = (receiverColor != GameColor.COLOR_WHITE);

            receiverCharge_1 = GameColor.COLOR_WHITE;
            receiverCharge_2 = GameColor.COLOR_WHITE;
        }

        Destroy(bulletHit.gameObject);
    }
}