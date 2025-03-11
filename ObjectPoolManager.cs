using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PoolKey
{
    CharSpawnFx,
    CharUpgradeFx,
    CastleDestroyFX,
    MonsterDeadFX,

    DamageValueText,
    GoldTextEffect,
    LifeTextEffect,

    WizardBullet,
    GunnerBullet,
    GolemCrack,
    SwordEffect,
}

public class ObjectPoolManager : Singleton<ObjectPoolManager>
{
    public struct PoolInfo
    {
        public Transform parent;
        public GameObject originObj;
        public Queue<GameObject> pool;
    }

    public Dictionary<PoolKey, PoolInfo> poolList = new Dictionary<PoolKey, PoolInfo>();

    public void Init(PoolKey poolKey, Transform parent, GameObject originObj)
    {
        PoolInfo poolInfo = new PoolInfo();
        poolInfo.parent = parent;
        poolInfo.originObj = originObj;
        poolInfo.pool = new Queue<GameObject>();

        poolInfo.originObj.SetActive(false);
        poolList.Add(poolKey, poolInfo);
    }

    //AutoHide 사용하려면, origin 프리팹에 PoolHideCoroutine 스크립트 추가 필요 
    public GameObject ShowObjectPool(PoolKey poolKey, Vector3 targetPos, Quaternion targetRot, bool isAutoHide = false, float hideTime = 2f)
    {
        GameObject obj;
        PoolInfo poolInfo = poolList[poolKey];

        if (poolInfo.pool.Count == 0)
        {
            obj = Instantiate(poolInfo.originObj, targetPos, targetRot, poolInfo.parent);
        }
        else
        {
            obj = poolInfo.pool.Dequeue();
            obj.transform.position = targetPos;
            obj.transform.rotation = targetRot;
        }

        obj.SetActive(true);

        if (isAutoHide)
        {
            PoolHideCoroutine gb = obj.GetComponent<PoolHideCoroutine>();

            if (gb.autoHideCor != null)
            {
                StopCoroutine(gb.autoHideCor);
                gb.autoHideCor = null;
            }

            gb.autoHideCor = StartCoroutine(AutoHide(poolKey, obj, hideTime));
        }

        return obj;
    }

    public void Hide(PoolKey poolKey, GameObject obj)
    {
        PoolInfo poolInfo = poolList[poolKey];

        obj.SetActive(false);
        poolInfo.pool.Enqueue(obj);
    }

    IEnumerator AutoHide(PoolKey poolKey, GameObject obj, float hideTime)
    {
        yield return new WaitForSeconds(hideTime);
        Hide(poolKey, obj);
    }
}
