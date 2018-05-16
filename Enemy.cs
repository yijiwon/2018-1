using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject {

    public int playerDamage;

    private Animator animator;
    // 플레이어 위치를 저장하고,
    // 적이 어디로 향할지 알려줌.
    private Transform target;
    private bool skipMove;

	// Use this for initialization
	protected override void Start () {
        GameManager.instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
	}

    // T는 플레이어.
    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;

        // x좌표가 대충 같은지 체크
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
        {
            // 같은 열에 있다면 target의 y좌표가 transform의 y좌표보다 큰지 체크.
            // 그렇다면 플레이어를 향해 위로 이동.
            // 아니면 플레이어를 향해 아래로 이동.
            yDir = target.position.y > transform.position.y ? 1 : -1;
        }
        else { 
            // 플레이어를 향해 수평으로 이동.
            // 양수 1을 사용해서 오른쪽으로 이동,
            // 음수 1을 사용해서 왼쪽으로 이동.
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }

        AttemptMove<Player>(xDir, yDir);
    }

    // 플레이어가 점거중인 공간으로 적이 이동하려고 시도 할 때 호출.
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;

        animator.SetTrigger("enemyAttack");

        hitPlayer.LoseFood(playerDamage);
    }
}
