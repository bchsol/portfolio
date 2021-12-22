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
        // 움직일 수 있을때
        if (this.canMove)
        {
            this.Move();
        }

        // 플레이어가 땅에 있는지 체크
        this.isGrounded = Physics2D.OverlapCircle(this.feetPos.position, this.checkRadius, this.whatIsGround);

        // HighJump 아이템을 먹었을 때
        if (this.HighJumpItem)
        {
            // 위 방향으로 점프
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
            // 스페이스를 눌러서 점프를 할수있을 때
            if (this.canMove && this.isGrounded && Input.GetKey(KeyCode.Space))
            {
                this.Jump();
                this.TotalSound.clip = this.JumpSound;
                this.TotalSound.Play();     // 점프소리 재생
            }
        }
        // Barrier아이템을 먹었을 때 시간이 지나면 점점 사라지는 효과
        if (this.invincibility)
        {
            this.BarrierCount += Time.deltaTime;
            this.Barrier.SetActive(false);  // Barrier 비활성화

            if ((double)this.BarrierCount % 0.3 < 0.15)
            {
                base.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 0f);
            }
            else
            {
                base.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
            }

            // 시간이 다되면
            if (this.BarrierCount >= 1.5f)
            {
                base.gameObject.GetComponent<SpriteRenderer>().color = new Color(255f, 255f, 255f, 1f);
                this.BarrierItem = false;
                this.invincibility = false;
                this.BarrierCount = 0f;
            }
        }
        

    }
    
    // 캐릭터 좌우 움직임
    private void Move()
    {
        Vector3 a = Vector3.zero;
        // 왼쪽 키 눌렀을 때
        if (Input.GetAxisRaw("Horizontal") < 0f)
        {
            a = Vector3.left;
            this.time1 += Time.deltaTime;
            // 왼쪽으로 걷는 스프라이트
            if (this.time1 % 0.5f < 0.25f)
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Lsprite1;
            }
            else
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Lsprite2;
            }
        }
        // 오른쪽 키 눌렀을때
        else if (Input.GetAxisRaw("Horizontal") > 0f)
        {
            a = Vector3.right;
            this.time2 += Time.deltaTime;
            // 오른쪽으로 걷는 스프라이트
            if (this.time2 % 0.5f < 0.25f)
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Rsprite1;
            }
            else
            {
                base.GetComponent<SpriteRenderer>().sprite = this.Rsprite2;
            }
        }
        base.transform.position += a * this.movePower * Time.deltaTime; // 위치를 움직인 힘만큼 옮겨줌
    }

    private void Jump()
    {
        this.rigid.velocity = Vector2.zero;
        Vector2 force = new Vector2(0f, this.jumpPower);
        this.rigid.AddForce(force, ForceMode2D.Impulse);
    }

    // 플레이어와 오브젝트 충돌 체크
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // HighJump 아이템을 먹었을 때
        if (collision.tag == "HighJumpItem")
        {
            UnityEngine.Object.Destroy(collision.gameObject);   // 아이템 파괴
            this.TotalSound.clip = this.HighJumpSound;
            this.TotalSound.Play();     // 효과음 재생
            this.HighJumpItem = true;
            base.GetComponent<Collider2D>().isTrigger = true;
        }
        // Barrier 아이템을 먹었을 때
        if (collision.tag == "BarrierItem")
        {
            UnityEngine.Object.Destroy(collision.gameObject);
            this.TotalSound.clip = this.BarrierSound;
            this.TotalSound.Play();
            this.BarrierCount = 0f;
            this.BarrierItem = true;
            this.Barrier.SetActive(true);   // Barrier 활성화
        }
        // 적에게 공격을 받았을 때
        if (collision.tag == "Monster")
        {
            // Barrier 아이템을 있을 경우
            if (this.BarrierItem)
            {
                this.invincibility = true;
            }
            else
            {
                UnityEngine.Object.Destroy(base.gameObject);    // 플레이어 죽음
            }
        }
        // 장애물과 부딪혔을 때
        if (collision.tag == "obstacle")
        {
            UnityEngine.Object.Destroy(collision.gameObject);
            base.StartCoroutine(this.Stay());
        }
        // 다음 스테이지
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
