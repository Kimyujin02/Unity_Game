using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    private float h = 0.0f;
    private float v = 0.0f;

    private Transform tr;
    public float moveSped = 10.0f;
    public float rotSpeed = 100.0f;
    // 애니메이션
    private Animation anim;

    private float turnSpeed = 80.0f;
    private float initHp = 100.0f;
    public Image imgHpbar;
    public float currHp = 100.0f;
    
    // 델리게이트 객체 선언
    public delegate void PlayerDieHandler();
    // 이벤트 객체 선언
    public static event PlayerDieHandler OnPlayerDie; // 이벤트 선언 , 반환 타입은 반드시 델리게이트 타입 

    //private GameMgr gameMgr;

    //void Start() 
    IEnumerator Start() // 코르틴 형식의 Start() 함수를 만든 것 -> 게임 엔진에서 호출 자동으로 함
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>(); // 애니메이션 컴포넌트 초기화
        anim.Play("Idle"); // 플레이어의 애니메이션을 아이들로 실행 (0.3초)
        initHp = currHp;

        //gameMgr = GameObject.Find("GameManager").GetComponent<GameMgr>();

        turnSpeed = 0.0f;
        yield return new WaitForSeconds(0.3f); // 0.3초 듸에
        turnSpeed = 80.0f; 
    }

    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        //Debug.Log("H=" + h.ToString());
        //Debug.Log("v=" + v.ToString());

        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //tr.Translate(Vector3.forward * moveSpeed * v * Time.deltaTime, Space.Self);
        tr.Translate(moveDir.normalized * Time.deltaTime *10.0f, Space.Self);
        //tr.Rotate(Vector3.up * Time.deltaTime * rotSpeed * Input.GetAxis("Mouse X")); // 100
        tr.Rotate(Vector3.up * Time.deltaTime * turnSpeed * Input.GetAxis("Mouse X"));  // 회전 속도 : 80 .....0.3초 뒤에 회전값이 반영
        
        PlayerAnim(h, v);
    }

    void PlayerAnim(float h, float v)
    {
        if (v >= 0.1f) // 전진 키가 눌려 졌을 경우
        {
            // (변경할 애니메이션 클립 이름, 페이드 아웃 시간)
            anim.CrossFade("RunF", 0.25f);
        }
        else if (v <= -0.1f)
        {
            anim.CrossFade("RunB", 0.25f);
        }
        else if (h >= 0.1f) // 오른쪽 키
        {
            anim.CrossFade("RunR", 0.25f);
        }
        else if (h <= -0.1f)
        {
            anim.CrossFade("RunL", 0.25f);
        }
        else
        {
            anim.CrossFade("Idle", 0.25f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(currHp >= 0.0f && other.CompareTag("PUNCH"))
        {
            currHp -= 10.0f;
            imgHpbar.fillAmount = (float)currHp / (float)initHp;

            if(currHp <= 0.0f)
            {
                PlayerDie();
            }
        }
    }

    void PlayerDie()
    {
        //Debug.Log("Die ---------------!!!");
        // 게임 오브젝트 자체를 가져옴 -> 덩어리가 큽니다. 
        //GameObject[] monsters = GameObject.FindGameObjectsWithTag("MONSTER"); // 성능 자체를 다운 , 직관적인 코딩
        //foreach(GameObject monster in monsters)
        //{
            //메세지를 뿌리는 역활 (반환 값은 받지 않는다) 
        //    monster.SendMessage("OnPlayerDie", SendMessageOptions.DontRequireReceiver);
        //}
        // 이벤트 발생
        OnPlayerDie(); // -> 이베트 발생 ------ 델리게이트 객체 (함수 포인터)
        //gameMgr.isGameOver = true;
        GameMgr.instance.isGameOver = true;
    }
}
