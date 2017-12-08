using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitManager : MonoBehaviour
{
    private int hp = 100;
    public int deathScore = 500;
    public int hitScore = 20;
    public GameObject chnage_prefab_Head;
    public GameObject bossNuclearParticlePrefab;
    private Creat_HD createHD;
    // Bullet 레이어 에 총알에 맞으면 발동하는 메소드

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
    //    {
    //        //총알 없어짐
    //        Destroy(other.gameObject);
    //        if (invincible == false)
    //        {
    //            ///// 데미지 계산
    //            var damage = 10;

    //            CollisionProcess(damage);
    //        }
    //    }
    //}

    private void Start()
    {
        createHD = GameObject.FindGameObjectWithTag("Gate").GetComponent<Creat_HD>();
    }

    public void CollisionProcess(int damage)
    {
        if (createHD.isInvincible != true)
        {
            if (hp > 0)
            {
                Debug.Log(hitScore);
                ScoreManager.instance.AddTechnicalScore(hitScore);
                StartCoroutine(EffectManager.instance.LaunchEffect(gameObject.transform.position, ParticleEffect.BossHit));
                hp -= damage;
                Debug.Log(hp);
                if (hp <= 0)
                {
                    Debug.Log(deathScore);
                    ScoreManager.instance.AddKillScore(deathScore);
                    hp = 0;

                    Change_Head();
                }
            }
        }
    }

    private void Change_Head()
    {
        #region MoveBase들 가져오기

        MoveBase first_move = GetComponent<MoveBase>();
        MoveBase second_move = null;
        if (first_move != null && first_move.back_Cube != null)
            second_move = first_move.back_Cube.GetComponent<MoveBase>();

        MoveBase third_move = null;
        if (second_move != null && second_move.back_Cube != null)
        {
            Debug.Log("세번째 머리");
            third_move = second_move.back_Cube.GetComponent<MoveBase>();
        }

        #endregion MoveBase들 가져오기

        #region 피격당한 객체의 다음 객체를 머리로 교체

        // 피격당한 객체의 다음 객체의 자리에 머리를 만들기
        GameObject newHeadGO = null;

        if (second_move != null)
        {
            newHeadGO = Instantiate(chnage_prefab_Head, second_move.transform.position, Quaternion.identity);

            // 바뀐 머리의 다음 객체를 다음/다음 객체로 연결
            var newHeadMove = newHeadGO.GetComponent<MoveBase>();
            newHeadMove.back_Cube = third_move;

            // 경로를 만들고 따라서 이동
            newHeadGO.transform.DOTweenPathFollow();

            // 피격당한 객체의 다음 객체를 제거하기
            Destroy(second_move.gameObject);
        }

        #endregion 피격당한 객체의 다음 객체를 머리로 교체

        // 피격당한 객체의 다음/다음 객체가 머리로 바뀐 객체를 따라가도록 변경
        if (third_move != null)
        {
            Debug.Log("피격당한 객체의 다음/다음 객체가 머리로 바뀐 객체를 따라가도록 변경");
            third_move.ChangeFollowTarget(newHeadGO.transform);
        }

        #region 피격당한 객체를 떨어뜨리기

        // 피격당한 객체에 강체 추가
        var rb = gameObject.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(Vector3.down * 100f);
        // 강체 옵션 조절

        // 피격당한 객체가 머리이면, 머리 움직임 정지
        if (gameObject.tag == "Head")
            DOTween.Kill(transform);

        // 폭발 파티클 실행
        var newclearParticleGO = Instantiate(bossNuclearParticlePrefab, transform.position, transform.rotation);
        newclearParticleGO.GetComponent<TargetFollow>().FollowTarget(transform);

        // 피격당한 객체를 일정시간뒤에 삭제
        Destroy(gameObject, 2f);

        // 피격당한 객체의 스크립트 지우기
        var c = gameObject.GetComponent<Collider>();
        if (c != null) Destroy(c);
        var mb = GetComponent<MoveBase>();
        if (mb != null) Destroy(mb);
        var hm = GetComponent<HitManager>();
        if (hm != null) Destroy(hm);
        var da = gameObject.GetComponent<DragonAttack>();
        if (da != null) Destroy(da);

        #endregion 피격당한 객체를 떨어뜨리기
    }
}