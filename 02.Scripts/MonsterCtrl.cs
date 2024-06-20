using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterCtrl : MonoBehaviour
{
    public enum State
    {
        IDLE,
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }
    private State state;
    private Transform monsterTr;
    private Transform playerTr;
    private NavMeshAgent nvAgent;
    private Animator anim;

    private float traceDist = 10.0f; // 추적(따라오기 시작) 거리
    private float attackDist = 2.0f; // 공격 범위
    private bool isDie = false;

    public GameObject bloodEffect; //프리펩 : 혈흔효과
    public GameObject bloodDecal; // 프리펩 : 데칼효과    

    private int hp = 100;

    private GameUI gameUI;

    void Awake()
    {
        monsterTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();
        nvAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        gameUI = GameObject.Find("GameUI").GetComponent<GameUI>();
    }

    void OnEnable()
    {
        PlayerCtrl.OnPlayerDie += this.OnPlayerDie; // 함수 포인터를 이벤트에 등록

        // 동시에 두 가지의 함수를 수행
        StartCoroutine(CheckMonsterState());
        StartCoroutine(MonsterAction()); // Action : 애니메이션, 동작 
    }

    void OnDisable()
    {
        PlayerCtrl.OnPlayerDie -= this.OnPlayerDie; // 함수 등록을 이벤트에서 제거
    }

    void Start()
    {
        state = State.IDLE;
    }

    void Update()
    {
        //nvAgent.destination = playerTr.position;
    }

    IEnumerator CheckMonsterState()
    {
        while (!isDie) // true : 무한 루프
        {
            yield return new WaitForSeconds(0.3f); // 0.3초 간격으로 리턴
            // 거리 계산
            float distance = Vector3.Distance(playerTr.position, monsterTr.position);

            if (state == State.DIE) yield break;

            if (distance <= attackDist) // 공격 범위 안에 있으면
            {
                // 공격 상태로 전환
                state = State.ATTACK;
            }
            else if (distance <= traceDist) // 추적 범위 안에 있으면
            {
                // 추적 상태로 전환
                state = State.TRACE;
            }
            else
            {
                // 아이들 상태로 전환  
                state = State.IDLE;
            }
        }
    }

    IEnumerator MonsterAction()
    {
        while (!isDie) // true : 무한 루프
        {
            yield return new WaitForSeconds(0.3f);
            switch (state)
            {
                case State.IDLE:
                    nvAgent.isStopped = true;            // 따라가기를 멈춤
                    anim.SetBool("IsTrace", false);      // 추적 애니메이션 정지
                    break;
                case State.TRACE:
                    nvAgent.destination = playerTr.position; // 실제 따라가기
                    nvAgent.isStopped = false;           // 따라가기를 시작
                    anim.SetBool("IsTrace", true);       // 추적 애니메이션 시작
                    anim.SetBool("IsAttack", false);     // 공격 애니메이션 정지
                    break;
                case State.ATTACK:
                    anim.SetBool("IsAttack", true);      // 공격 애니메이션 시작
                    break;
                case State.DIE: // 몬스터가 죽었을 때 
                    isDie = true;
                    nvAgent.isStopped = true;
                    anim.SetTrigger("Die");
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (state == State.TRACE)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, traceDist); // 반지름이 10인 원
        }
        if (state == State.ATTACK)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackDist); // 반지름이 2인 원
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            Destroy(collision.gameObject);
            anim.SetTrigger("IsHit"); // 히트 애니메이션

            // 혈흔 효과(총알이 맞은 위치)
            Vector3 pos = collision.GetContact(0).point;
            Quaternion rot = Quaternion.LookRotation(-collision.GetContact(0).normal);
            ShowBloodEffect(pos, rot); // 혈흔 효과
            CreateBloodEffect(pos); // 데칼 효과 

            hp -= 50;
            if (hp <= 0)
            {
                state = State.DIE;
            }

            gameUI.DispScore(50);
        }
    }

    // OnDamage 메서드를 public으로 수정
    public void OnDamage(Vector3 pos, Vector3 normal)
    {
        anim.SetTrigger("Hit");
        Quaternion rot = Quaternion.LookRotation(normal);
        Debug.Log("몬스터에 맞았습니다. 혈흔을 흘립니다.");
    }

    // 몸
    void ShowBloodEffect(Vector3 pos, Quaternion rot)
    {
        GameObject blood = Instantiate(bloodEffect, pos, rot) as GameObject;
        Destroy(blood, 1.0f);
    }

    // 바닥
    void CreateBloodEffect(Vector3 pos)
    {
        Vector3 decalPos = monsterTr.position + (Vector3.up * 0.05f);
        Quaternion decalRot = Quaternion.Euler(90, 0, Random.Range(0, 360));
        GameObject blood = Instantiate(bloodDecal, decalPos, decalRot) as GameObject;
        float scale = Random.Range(1.5f, 3.5f);
        blood.transform.localScale = Vector3.one * scale; // (1, 1, 1) 
        Destroy(blood, 2.0f);
    }

    // 플레이어가 죽었을 때 몬스터가 하는 행동
    void OnPlayerDie()
    {
        Debug.Log("플레이어 사망");
        StopAllCoroutines();
        nvAgent.isStopped = true;
        // 죽었다라는 애니메이션 연출
        anim.SetFloat("Speed", Random.Range(0.8f, 1.2f));
        anim.SetTrigger("IsDie");
    }
}
