using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    private float time;
    private bool type;
    public float MaxTime;
    public float Speed;
    public bool direction;

    void Start()
    {
        this.type = true;
    }

    void Update()
    {
        if (this.type)
        {
            this.time += Time.deltaTime;
            if (this.direction)
            {
                base.transform.Translate(Vector2.left * this.Speed * Time.deltaTime);
            }
            else
            {
                base.transform.Translate(Vector2.down * this.Speed * Time.deltaTime);
            }
        }
        else
        {
            this.time -= Time.deltaTime;
            if (this.direction)
            {
                base.transform.Translate(Vector2.right * this.Speed * Time.deltaTime);
            }
            else
            {
                base.transform.Translate(Vector2.up * this.Speed * Time.deltaTime);
            }
        }
        if (this.time >= this.MaxTime)
        {
            this.time = 0f;
            this.type = false;
        }
        if (this.time <= -this.MaxTime)
        {
            this.time = 0f;
            this.type = true;
        }
    }

    
}
