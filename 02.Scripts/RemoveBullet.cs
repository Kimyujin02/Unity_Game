using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;

    void OnCollisionEnter(Collision coll)
    {
        //if (coll.collider.tag == "BULLET")
        if(coll.collider.CompareTag("BULLET"))
        {
            ContactPoint cp = coll.GetContact(0);
            Quaternion rot = Quaternion.LookRotation(-cp.normal);
            GameObject spark = Instantiate(sparkEffect, cp.point, rot);
            //Instantiate(sparkEffect, coll.transform.position, Quaternion.identity);
            //Destroy(sparkEffect, 0.5f);
            //Destroy(sparkEffect);
            Destroy(coll.gameObject);
            Destroy(spark, 0.5f);
        }
    }
}
