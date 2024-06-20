using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : MonoBehaviour
{
    public Transform[] points;
    public GameObject monsterPrefabs;
    public float createTime = 2.0f;
    public int maxMonster = 10;
    public bool isGameOver = false;

    public static GameMgr instance = null;

    public List<GameObject> monsterPool = new List<GameObject>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        points = GameObject.Find("SpawnPoint").GetComponentsInChildren<Transform>();

        // (Pooling) 배열에다가 미리 몬스터 객체를 생성한 다음 비활성화 해서 저장
        for(int i=0; i<maxMonster; i++)
        {
            GameObject monster = (GameObject)Instantiate(monsterPrefabs);
            monster.name = "Monster_" + i.ToString();
            monster.SetActive(false);
            monsterPool.Add(monster);
        }

        if(points.Length > 0)
        {
            StartCoroutine(this.CreateMonster());
        }
    }

    IEnumerator CreateMonster()
    {
        while(!isGameOver)
        {
            /*
            int monsterCount = (int) GameObject.FindGameObjectsWithTag("MONSTER").Length;
            if(monsterCount < maxMonster)
            {
                yield return new WaitForSeconds(createTime);
                int idx = Random.Range(1, points.Length);
                //Instantiate(monsterPrefabs, points[idx].position, points[idx].rotation);
            }
            else{
                yield return null;
                // 
            }
            */

            yield return new WaitForSeconds(createTime);
            if(isGameOver) yield break;
            foreach(GameObject monster in monsterPool)
            {
                if(!monster.activeSelf)
                {
                    int idx = Random.Range(1, points.Length);
                    monster.transform.position = points[idx].position;
                    monster.SetActive(true);
                    break;
                }
            }
        }
    }
}
