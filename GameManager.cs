using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public float turnDelay = .1f;
    public static GameManager instance = null;
    public BoardManager boardScript;
    public int playerFoodPoints = 100;
    // public이지만 에디터에서 숨긴다.
    [HideInInspector] public bool playerTurn = true;

    private int level = 3;
    private List<Enemy> enemies;
    private bool enemiesMoving;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
	}
	
    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    public void GameOver()
    {
        enabled = false;
    }

    private void Update()
    {
        if( playerTurn || enemiesMoving)
        {
            return;
        }

        StartCoroutine(MoveEnemies());
    }

    // 적들이 자신을 게임 매니저에 등록하도록 해서
    // 게임 매니저가 적들을 움직일 수 있게
    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    // 연속적으로 한번에 하나씩 적을 옮기는데 사용.
    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        // 적들이 없는 지 체크, 첫 레벨을 의미.
        if(enemies.Count == 0)
        {
            // 적이 없지만 플레이어가 기다리게 한다.
            yield return new WaitForSeconds(turnDelay);
        }

        for(int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            // 적들마다 MoveEnemy()를 호출하면 다음 적을 호출하기 전에,
            // yield 키워드와 적의 moveTime 변수를 입력하여 기다린다.
            yield return new WaitForSeconds(enemies[i].moveTime);
        }

        playerTurn = true;
        enemiesMoving = false;
    }
}
