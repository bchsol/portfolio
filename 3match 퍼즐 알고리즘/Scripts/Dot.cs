using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour
{
    [Header("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;
    public bool isMatched = false;

    private Board board;
    private GameObject otherDot;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;
    private Vector2 tempPosition;
    public float swipeAngle = 0;
    public float swipeResist = 1f;

    void Start()
    {
        board = FindObjectOfType<Board>();
        targetX = (int)transform.position.x;    // 현재 x좌표
        targetY = (int)transform.position.y;    // 현재 y좌표
        column = targetX;
        row = targetY;
        
        previousColumn = column; // 이전 좌표
        previousRow = row;
    }

    void Update()
    {
        FindMatches();
        if(isMatched)
        {
            SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
            mySprite.color = new Color(0f, 0f, 0f, .2f);
        }

        targetX = column;
        targetY = row;

        // Mathf.Abs() : 절대값
        // 좌우를 스왑
         if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.04f);
            // Lerp(최초위치, 목표위치, 경과시간)

            // Vector2 velo = Vector2.zero;
            // transform.position = Vector2.SmoothDamp(transform.position, tempPosition,ref velo, 0.05f);
            // SmoothDamp(float current, float target, ref currentVelocity, float smooothTime, float maxSpeed, Time.deltaTime)
            //            최초위치, 목표위치, 현재 속도, 도달하기위한 시간, 최고속도)
            // Lerp 보다 SmoothDamp가 더 부드럽게 움직임
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }

        // 위아래를 스왑
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            // 목표를 향해 이동
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.04f);
        }
        else
        {
            // 위치를 직접설정
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }

        // 임시 종료키
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


    }

    // 움직인 블럭이 안맞을경우 다시 되돌림
    public IEnumerator CheckMoveCo()
    {
        yield return new WaitForSeconds(.7f);

        if(otherDot != null)
        {
            if(!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;
            }
            otherDot = null;
        }
        
    }

    private void OnMouseDown()
    {
        firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 게임화면내의 마우스 좌표를 가져옴

    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    // 드래그 각도 계산
    void CalculateAngle()
    {
        // 클릭했을때 움직이는것을 방지함
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            // 눌렀다 땠을때(드래그) 각도를 계산함
            // Atan2(float y, float x)
            // Atan2의 반환값은 Radian이기 때문에 Rad2Deg을 사용해서 각도로 변환함 = 180 * PI 

            MovePieces();
        }
    }

    // 드래그 방향에 따라 블럭좌표를 바꿔줌
    void MovePieces()
    {
        // Right Swipe / x좌표 + 1
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1)
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        // Up Swipe / y좌표 + 1
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        // Left Swipe / x좌표 - 1
        else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        // Down Swipe / y좌표 - 1
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    // 위아래 좌우가 같은색을 찾음
    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];
            if (leftDot1 != this.gameObject && rightDot1 != this.gameObject)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;

                    isMatched = true;
                }
            }
        }
        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];

            if (upDot1 != this.gameObject && downDot1 != this.gameObject)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;

                    isMatched = true;
                }
            }
        }
    }

}
