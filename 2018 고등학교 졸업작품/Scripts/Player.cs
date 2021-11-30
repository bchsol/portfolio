using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public float movePower = 5f;
    public float jumpPower = 5f;
    public float HighjumpPower;
    private Rigidbody2D rigid;
    private Rigidbody2D vel;
    private Vector3 movement;
    public int jumpCount = 1;

    // 아이템
    private bool HighJumpItem;
    private bool BarrierItem;
    private bool invincibility;

    private float TimeCount;
    private float BarrierCount;

    // 좌우 움직임 스프라이트
    public Sprite Lsprite1;
    public Sprite Lsprite2;
    public Sprite Rsprite1;
    public Sprite Rsprite2;
    private SpriteRenderer sr;

    // 좌우 움직임 시간변수
    private float time1;
    private float time2;

    private bool type = true;
    public float ObstacleSec = 2f;
    public bool canMove = true; // 움직일수있는지
    private bool isGrounded;    // 땅을 밟고있는지
    public Transform feetPos;
    public float checkRadius;
    public LayerMask whatIsGround;
    public GameObject Barrier;
 

    //  오디오 파일
    private AudioSource TotalSound;
    public AudioClip JumpSound;
    public AudioClip BarrierSound;
    public AudioClip HighJumpSound;

    private bool Ck;

    private void Start()
    {

        this.vel = base.gameObject.GetComponent<Rigidbody2D>();
        this.rigid = base.gameObject.GetComponent<Rigidbody2D>();
        this.sr = base.gameObject.GetComponent<SpriteRenderer>();
        this.TotalSound = base.GetComponent<AudioSource>();

        this.HighJumpItem = false;
        this.BarrierItem = false;
        this.invincibility = false;
    }
    private void FixedUpdate()
    {
        if (this.canMove)
        {
            this.Move();
        }
        this.isGrounded = Physics2D.OverlapCircle(this.feetPos.position, this.checkRadius, this.whatIsGround);
        if (this.HighJumpItem)
        {
            this.vel.velocity = new Vector2(0f, this.HighjumpPower * this.TimeCount);
            this.TimeCount += Time.deltaTime;
            if (this.TimeCount >= 0.5f)
            {
                this.HighJumpItem = false;
            }
        }
        else
        {
            base.GetComponent<Collider2D>().isTrigger = false;
            if (this.canMove && this.isGrounded && Input.GetKey(KeyCode.Space))
            {
                this.Jump();
                this.TotalSound.clip = this.JumpSound;
                this.TotalSound.Play();
            }
        }
        if (this.invincibility)
        {
            this.BarrierCount += Time.deltaTime;
            this.Barrier.SetActive(false);
            if ((double)this.BarrierCount % 0.3 < 0.15)
            {
                base.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
            }
            else
            {
                base.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
            }
            if (this.BarrierCount >= 1.5f)
            {
                base.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
                this.BarrierItem = false;
                this.invincibility = false;
                this.BarrierCount = 0f;
            }
        }
        

    }
    private void Move()
    {
        Vector3 a = Vector3.zero;
        if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            a = Vector3.left;
            this.time1 += Time.deltaTime;
            if (this.time1 % 0.5f < 0.25f)
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Lsprite1;
            }
            else
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Lsprite2;
            }
        }
        else if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            a = Vector3.right;
            this.time2 += Time.deltaTime;
            if (this.time2 % 0.5f < 0.25f)
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Rsprite1;
            }
            else
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Rsprite2;
            }
        }
        base.transform.position += a * this.movePower * Time.deltaTime;
    }

    private void Jump()
    {
        this.rigid.velocity = Vector2.zero;
        Vector2 force = new Vector2(0f, this.jumpPower);
        this.rigid.AddForce(force, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "HighJumpItem")
        {
            UnityEngine.Object.Destroy(collision.gameObject);
            this.TotalSound.clip = this.HighJumpSound;
            this.TotalSound.Play();
            this.HighJumpItem = true;
            base.GetComponent<Collider2D>().isTrigger = true;
        }
        if (collision.tag == "BarrierItem")
        {
            UnityEngine.Object.Destroy(collision.gameObject);
            this.TotalSound.clip = this.BarrierSound;
            this.TotalSound.Play();
            this.BarrierCount = 0f;
            this.BarrierItem = true;
            this.Barrier.SetActive(true);
        }
        if (collision.tag == "Monster")
        {
            if (this.BarrierItem)
            {
                this.invincibility = true;
            }
            else
            {
                UnityEngine.Object.Destroy(base.gameObject);
            }
        }
        if (collision.tag == "obstacle")
        {
            UnityEngine.Object.Destroy(collision.gameObject);
            base.StartCoroutine(this.Stay());
        }
        if (collision.tag == "Next1")
        {
            this.Ck = true;
            SceneManager.LoadScene("next1");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Monster")
        {
            if (this.BarrierItem)
            {
                this.invincibility = true;
            }
            else
            {
                SceneManager.LoadScene("gameover");
            }
        }
    }
    public IEnumerator Stay()
    {
        this.canMove = false;
        yield return new WaitForSeconds(this.ObstacleSec);
        this.canMove = true;
        yield break;
    }
    private void OnBecameInvisible()
    {
        if (!this.Ck)
        {
            SceneManager.LoadScene("gameover");
        }
    }

}
