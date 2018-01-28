using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallPiece : MonoBehaviour
{
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
                results[i].GetComponent<Bullet>().RemoveBullet();
            }
        }
    }
}
