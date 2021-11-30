using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShotBullet : MonoBehaviour
{
    public float bulletVelocity = 5f;
    public GameObject bullet;
    public GameObject bullet1;
    public float coolTime = 1f;
    public bool canShoot = true;
    public float bulletcnt = 5f;
    public AudioClip Shot;

    private void Update()
    {
        if (this.canShoot && Input.GetMouseButtonDown(0))
        {
            base.GetComponent<AudioSource>().clip = this.Shot;
            base.GetComponent<AudioSource>().Play();
            Vector3 a = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 a2 = a - base.transform.position;
            a2.Normalize();
            //GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.bullet1, base.transform.position + a2 * 0.5f, Quaternion.identity);
            gameObject.GetComponent<Rigidbody2D>().velocity = a2 * this.bulletVelocity;
            this.bulletcnt -= 1f;
            if (this.bulletcnt == 0f)
            {
                base.StartCoroutine(this.shoot());
            }
        }
        if (this.bulletcnt < 5f && Input.GetKeyDown(KeyCode.R))
        {
            base.StartCoroutine(this.shoot());
        }
    }
    public IEnumerator shoot()
    {
        this.canShoot = false;
        yield return new WaitForSeconds(this.coolTime);
        this.canShoot = true;
        this.bulletcnt = 5f;
        yield break;
    }
}
