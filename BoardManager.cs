using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;

        public Count (int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }

    // 이 변수들로 게임 보드를 변경 가능.
    public int colums = 8;
    public int rows = 8;
    // 레벨마다 얼마나 많은 벽을 랜덤하게 설정할 지
    // 레벨마다 최소 5개의 벽과 최대 9개의 벽이 있다.
    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] wallTiles;
    public GameObject[] foodTiles;
    public GameObject[] enemyTiles;
    public GameObject[] outerWallTiles;

    // 하이어라키를 깨끗이 해놓기 위해 사용할 변수.
    // 생성한 많은 변수들을 boardHolder에 집어넣는다. (자식으로)
    private Transform boardHolder;
    // 보드 위에 가능한 모든 위치를 기억하고,
    // 오브젝트가 해당 장소에 있는지 없는지 추적하는데도 사용.
    private List <Vector3> gridPositions = new List<Vector3>();

    // 리스트 초기화.
    void InitializeList()
    {
        gridPositions.Clear();

        // 게임 상에서 벽, 적, 아이템들이 있을 수 있는 가능한 모든 위치를 만듬.
        // 1을 뺀 이유는 floor타일의 가장자리를
        // Exit를 위해서 빼 놓음
        for(int x = 1; x < colums - 1; x++)
        {
            for(int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    // 바깥벽과 게임 보드의 바닥을 짓기 위해 사용.
    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        // 게임 보드의 활성된 부분의 가장자리를
        // 바깥 벽 타일을 이용해 짓고 있기 때문.
        for (int x = -1; x < colums + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == colums || y == -1 || y == rows)
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;

                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);

        for(int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        // 랜덤 범위를 특정화하지 않았기 때문에 최소값과 최대값이 같다.
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(colums - 1, rows - 1, 0f), Quaternion.identity);
    }
}
