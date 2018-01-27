using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Bullet : MonoBehaviour
{
    public float speed = 10.0f;
    public float travelTime = 3.0f;

    private float timeAlive = 0.0f;
    private GameObject shooter;
    private GameColor bulletColor;
    private SpriteRenderer sprite;

	// Update is called once per frame
	void Update ()
    {
        Vector3 movement = new Vector3(0.0f, 1.0f, 0.0f);
        transform.Translate(movement * speed * Time.deltaTime);

        timeAlive += Time.deltaTime;
        if( timeAlive > travelTime)
        {
            Destroy(gameObject);
        }
    }

    public GameColor GetColor()
    {
        return bulletColor;
    }

    public void SetColor(GameColor newColor)
    {
        if( sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        bulletColor = newColor;
        sprite.color = ColorDefs.GetColor(bulletColor);
    }

    public void SetShooter(GameObject go)
    {
        shooter = go;
    }

    public GameObject GetShooter()
    {
        return shooter;
    }
}
