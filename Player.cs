using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    public float restartLevelDelay = 1f;

    private Animator animator;
    private int food;

    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        // 부모 클래스의 start() 호출.
        base.Start();
	}

    // 게임 오브젝트가 비활성화 되는 순간 호출
    private void OnDisable()
    {
        GameManager.instance.playerFoodPoints = food;
    }

    // Update is called once per frame
    void Update () {
        if (!GameManager.instance.playerTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        // 대각선으로 이동하지 못 하게
        if (horizontal != 0)
            vertical = 0;

        // 0이 아니면 플레이어가 움직이려 한다는 뜻이므로,
        // AttempMove 함수를 호출한다.
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
	}

    // 제네릭 T = 움직이는 오브젝트가 마주칠 대상의 컴포넌트의 타입
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // 움직일때마다 음식 감소
        food--;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playerTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Exit")
        {
            Invoke("Restart", restartLevelDelay);
            enabled = false;
        }
        else if(collision.tag == "Food")
        {
            food += pointsPerFood;
            collision.gameObject.SetActive(false);
        }
        else if(collision.tag == "Soda")
        {
            food += pointsPerSoda;
            collision.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        // 입력으로 받은 컴포넌트를 wall로
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("PlayeChop");
    }

    // 플레이어가 출구와 충돌할 때
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    // 적이 플레이어를 공격할 때 호출
    public void LoseFood(int loss)
    {
        animator.SetTrigger("PlayerHit");
        food -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            GameManager.instance.GameOver();
        }
    }
}
