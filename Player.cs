using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject {

    public int wallDamage = 1;
    public int pointsPerFood = 10;
    public int pointsPerSoda = 20;
    int horizontal = 0;
    int vertical = 0;
    public float restartLevelDelay = 1f;
    public Text foodText;
    // 게임오버는 하나만 있으면 되고
    // 두 개씩 있는 녀석들은 RandomizeSfx 함수를 사용해서
    // 랜덤하게 스위칭.
    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip eatSound1;
    public AudioClip eatSound2;
    public AudioClip drinkSound1;
    public AudioClip drinkSound2;
    public AudioClip gameOverSound;

    private Animator animator;
    private int food;
    private Vector2 touchOrigin = -Vector2.one;

    // Use this for initialization
    protected override void Start () {
        animator = GetComponent<Animator>();

        food = GameManager.instance.playerFoodPoints;

        foodText.text = "Food: " + food;

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

        /*


        //#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER

        
        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");
        */
        

        Debug.Log("hozintaol: " + horizontal + ", vertical: " + vertical);

        // 대각선으로 이동하지 못 하게
        if (horizontal != 0)
            vertical = 0;

        //MoveAll();

        //#else
        /*
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];

            if(myTouch.phase == TouchPhase.Began)
            {
                touchOrigin = myTouch.position;
            }

            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                Vector2 touchEnd = myTouch.position;
                float x = touchEnd.x - touchOrigin.x;
                float y = touchEnd.y - touchOrigin.y;
                touchOrigin.x = -1;
                if(Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;
                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }
        */



        //#endif


        // 0이 아니면 플레이어가 움직이려 한다는 뜻이므로,
        // AttempMove 함수를 호출한다.
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);

        horizontal = 0;
        vertical = 0;
    }

    public void MoveAll()
    {
        LefthorizontalMoveOn();
        RighthorizontalMoveOn();
        UpVerticalMoveOn();
        DownVerticalMoveOn();
    }

    public void LefthorizontalMoveOn()
    {
        horizontal = -1;
        vertical = 0;
    }

    public void LefthorizontalMoveDown()
    {
        horizontal = 0;
        vertical = 0;
    }

    public void RighthorizontalMoveOn()
    {
        horizontal = 1;
        vertical = 0;
    }

    public void RighthorizontalMoveDown()
    {
        horizontal = 0;
        vertical = 0;
    }

    public void UpVerticalMoveOn()
    {
        horizontal = 0;
        vertical = 1;
    }

    public void UpVerticalMoveDown()
    {
        horizontal = 0;
        vertical = 0;
    }

    public void DownVerticalMoveOn()
    {
        horizontal = 0;
        vertical = -1;
    }

    public void DownVerticalMoveDown()
    {
        horizontal = 0;
        vertical = 0;
    }

    // 제네릭 T = 움직이는 오브젝트가 마주칠 대상의 컴포넌트의 타입
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        // 움직일때마다 음식 감소
        food--;
        foodText.text = "Food: " + food;

        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        if (Move(xDir, yDir, out hit))
        {
            // 2개 중 하나를 재생.
            // ,로 나눠서 입력했는데 params 키워드를 사용해서
            // 하나의 배열로 합쳐 입력됨.
            SoundManager.instance.RandomizeSfx(moveSound1, moveSound2);
        }

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
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            SoundManager.instance.RandomizeSfx(eatSound1, eatSound2);
            collision.gameObject.SetActive(false);
        }
        else if(collision.tag == "Soda")
        {
            food += pointsPerSoda;
            SoundManager.instance.RandomizeSfx(drinkSound1, drinkSound2);
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            collision.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        // 입력으로 받은 컴포넌트를 wall로
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }

    // 플레이어가 출구와 충돌할 때
    private void Restart()
    {
        Application.LoadLevel(Application.loadedLevel);
    }

    // 적이 플레이어를 공격할 때 호출
    public void LoseFood(int loss)
    {
        animator.SetTrigger("playerHit");
        food -= loss;
        foodText.text = "- " + loss + " Foods: " + food;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if(food <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            GameManager.instance.GameOver();
        }
    }
}
