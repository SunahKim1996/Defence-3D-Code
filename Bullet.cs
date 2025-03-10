using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletType
{
    Wizard,
    Gunner,
}

public class Bullet : MonoBehaviour
{
    public Transform target;
    public int power;
    public BulletType bulletType;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Game || target == null)
            return;

        transform.position = Vector3.Lerp(transform.position, target.position, 0.1f);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetPower(int power)
    {
        this.power = power;
    }

    public void ClearTrailRenderer()
    {
        GetComponent<TrailRenderer>().Clear();
    }
}
