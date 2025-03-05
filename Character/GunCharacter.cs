using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunCharacter : Character
{
    
    public override void Upgrade()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Power = 10;
        data.AttackSpeed = 0.2f;
        data.AttackRange = 8;
        data.Cost = 1;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
