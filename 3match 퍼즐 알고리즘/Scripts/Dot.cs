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
        targetX = (int)transform.position.x;    // ���� x��ǥ
        targetY = (int)transform.position.y;    // ���� y��ǥ
        column = targetX;
        row = targetY;
        
        previousColumn = column; // ���� ��ǥ
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

        // Mathf.Abs() : ���밪
        // �¿츦 ����
         if (Mathf.Abs(targetX - transform.position.x) > 0.1f)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.04f);
            // Lerp(������ġ, ��ǥ��ġ, ����ð�)

            // Vector2 velo = Vector2.zero;
            // transform.position = Vector2.SmoothDamp(transform.position, tempPosition,ref velo, 0.05f);
            // SmoothDamp(float current, float target, ref currentVelocity, float smooothTime, float maxSpeed, Time.deltaTime)
            //            ������ġ, ��ǥ��ġ, ���� �ӵ�, �����ϱ����� �ð�, �ְ�ӵ�)
            // Lerp ���� SmoothDamp�� �� �ε巴�� ������
        }
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }

        // ���Ʒ��� ����
        if (Mathf.Abs(targetY - transform.position.y) > 0.1f)
        {
            // ��ǥ�� ���� �̵�
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, 0.04f);
        }
        else
        {
            // ��ġ�� ��������
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
            board.allDots[column, row] = this.gameObject;
        }

        // �ӽ� ����Ű
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }


    }

    // ������ ���� �ȸ������ �ٽ� �ǵ���
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
        // ����ȭ�鳻�� ���콺 ��ǥ�� ������

    }

    private void OnMouseUp()
    {
        finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        CalculateAngle();
    }

    // �巡�� ���� ���
    void CalculateAngle()
    {
        // Ŭ�������� �����̴°��� ������
        if (Mathf.Abs(finalTouchPosition.y - firstTouchPosition.y) > swipeResist || Mathf.Abs(finalTouchPosition.x - firstTouchPosition.x) > swipeResist)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x) * Mathf.Rad2Deg;
            // ������ ������(�巡��) ������ �����
            // Atan2(float y, float x)
            // Atan2�� ��ȯ���� Radian�̱� ������ Rad2Deg�� ����ؼ� ������ ��ȯ�� = 180 * PI 

            MovePieces();
        }
    }

    // �巡�� ���⿡ ���� ����ǥ�� �ٲ���
    void MovePieces()
    {
        // Right Swipe / x��ǥ + 1
        if(swipeAngle > -45 && swipeAngle <= 45 && column < board.width-1)
        {
            otherDot = board.allDots[column + 1, row];
            otherDot.GetComponent<Dot>().column -= 1;
            column += 1;
        }
        // Up Swipe / y��ǥ + 1
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height-1)
        {
            otherDot = board.allDots[column, row + 1];
            otherDot.GetComponent<Dot>().row -= 1;
            row += 1;
        }
        // Left Swipe / x��ǥ - 1
        else if (swipeAngle > 135 || swipeAngle <= -135 && column > 0)
        {
            otherDot = board.allDots[column - 1, row];
            otherDot.GetComponent<Dot>().column += 1;
            column -= 1;
        }
        // Down Swipe / y��ǥ - 1
        else if (swipeAngle < -45 && swipeAngle >= -135 && row > 0)
        {
            otherDot = board.allDots[column, row - 1];
            otherDot.GetComponent<Dot>().row += 1;
            row -= 1;
        }
        StartCoroutine(CheckMoveCo());
    }

    // ���Ʒ� �¿찡 �������� ã��
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
