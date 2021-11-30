using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float count;
    private void Update()
    {
        this.count += Time.deltaTime;
        base.transform.rotation = Quaternion.Euler(0f, 0f, this.count * 700f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "foothold" || collision.tag == "Monster")
        {
            UnityEngine.Object.Destroy(base.gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        UnityEngine.Object.Destroy(base.gameObject);
    }
}
