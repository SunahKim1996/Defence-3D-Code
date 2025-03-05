using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemCharacter : Character
{
    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Power = 10;
        data.AttackSpeed = 2;
        data.AttackRange = 3;
        data.Cost = 1;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
