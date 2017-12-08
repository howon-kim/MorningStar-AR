using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creat_BD : Creat_Manager
{
    private GameObject body;
    private GameObject tail;

    public float distance = 5f;
    public float spawnDelay = 0.1f;

    public void Awake()
    {
        body = Resources.Load("DragonBody", typeof(GameObject)) as GameObject;
        tail = Resources.Load("DragonTail", typeof(GameObject)) as GameObject;
    }

    public void Creat_Body()
    {
        StartCoroutine(Creat(body));
    }

    public void Creat_tail()
    {
        StartCoroutine(Creat(tail));
    }

    private IEnumerator Creat(GameObject prefab)
    {
        yield return new WaitForSeconds(spawnDelay);

        var tempPosition = transform.position - transform.forward * distance;

        GameObject bd_Obj = Instantiate(prefab, tempPosition, body.transform.rotation, boss_Lenght[0].transform.parent);

        var newBD_Creat_BD = bd_Obj.AddComponent<Creat_BD>();
        boss_Lenght.Add(bd_Obj);

        newBD_Creat_BD.distance = this.distance;
        newBD_Creat_BD.spawnDelay = this.spawnDelay;

        var enoughCount = BodyCount - boss_Lenght.Count;
        if (enoughCount > 1)
            newBD_Creat_BD.Creat_Body();
        else if (enoughCount == 1)
            newBD_Creat_BD.Creat_tail();

        var nextBodyMove = bd_Obj.GetComponent<Body_Move>();
        if (nextBodyMove != null)
        {
            var currentMoveBase = GetComponent<MoveBase>();
            currentMoveBase.back_Cube = nextBodyMove;
            nextBodyMove.ChangeFollowTarget(currentMoveBase.transform);
        }

        Destroy(this, 0.5f);
    }
}