using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ColorGame;

public class Goal : MonoBehaviour
{
    public GameColor requiredColor;
    public MeshRenderer bulbRenderer;
    public float chargedTime = 0.5f;

    public GameObject wiresRoot;

    private bool isCharged = false;
    private bool isCorrectCharge = false;
    private bool isSolved = false;
    private float chargeCountdown = 0.0f;
    private GameColor currentColor;

    // Use this for initialization
    void Start ()
    {
        currentColor = GameColor.COLOR_WHITE;
        setWires(false);
    }
	
	// Update is called once per frame
	void Update ()
    {
		if( isCharged && !isSolved )
        {
            if (chargeCountdown < chargedTime)
            {
                chargeCountdown += Time.deltaTime;
            }
            else
            {
                isCharged = false;
                isCorrectCharge = false;
                chargeCountdown = 0.0f;
                Color newColor = Color.white;
                bulbRenderer.material.color = new Color(newColor.r, newColor.g, newColor.b, 0.0f);
                currentColor = GameColor.COLOR_WHITE;
                setWires(false);
            }
        }
	}

    public bool CheckCharged()
    {
        return isCorrectCharge;
    }

    public void SetComplete()
    {
        isSolved = true;
    }

    void setWires(bool wireState)
    {
        foreach( Transform wire in wiresRoot.transform)
        {
            wire.gameObject.GetComponent<SpriteRenderer>().color = (wireState) ? ColorDefs.GetColor(currentColor) : Color.grey;
        }
    }

    void FixedUpdate()
    {
        if (!isSolved)
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
                    GameColor color = currentBullet.GetColor();
                    currentColor = color;

                    Color newColor = ColorDefs.GetColor(color);
                    bulbRenderer.material.color = new Color(newColor.r, newColor.g, newColor.b, 0.0f);

                    currentBullet.RemoveBullet();
                }

                isCharged = true;

                if (currentColor == requiredColor)
                {
                    isCorrectCharge = true;
                    setWires(true);
                }
            }
        }
    }
}
