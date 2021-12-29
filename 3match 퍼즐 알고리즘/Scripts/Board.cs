using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width;
    public int height;
    public GameObject tilePrefabs;  // 보드판 프리팹
    public GameObject[] dots;   // 색 배열
    private BackgroundTile[,] allTiles; // 보드판 배열
    public GameObject[,] allDots;

    void Start()
    {
        allTiles = new BackgroundTile[width, height];   // 가로 세로 길이만큼 보드판 생성
        allDots = new GameObject[width, height];
        SetUp();
    }

    // 보드판 그리기
    private void SetUp()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                // 보드판
                Vector2 tempPosition = new Vector2(i, j);
                GameObject backgroundTile = Instantiate(tilePrefabs, tempPosition, Quaternion.identity);
                // 보드 오브젝트 생성 2d 이기때문에 회전값x = Quaternion.identity
                // Instantiate(오브젝트나 프리팹 객체, 위치, 회전값)
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "( " + i + ", " + j + " )";
                int dotToUse = Random.Range(0, dots.Length);    // 0 ~ 색 갯수 랜덤값

                int maxIterations = 0;
                while(MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                // 색 오브젝트 생성
                dot.transform.parent = transform;
                dot.name = "( " + i + ", " + j + " )";

                allDots[i, j] = dot;
            }
        }
    }

    // 처음 보드를 생성할때 매치되지않고 생성
    private bool MatchesAt(int column, int row, GameObject piece)
    {
        if(column > 1 && row > 1)
        {
            if(allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
            {
                return true;
            }
            if (allDots[column , row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
            {
                return true;
            }
        }
        else if(column <= 1 || row <= 1)
        {
            if(row > 1)
            {
                if(allDots[column, row - 1].tag == piece.tag && allDots[column, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }
            if (column > 1)
            {
                if (allDots[column - 1, row].tag == piece.tag && allDots[column - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }
        }
        return false;
    }

}
