using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WizardCharacter : Character
{
    public override void Upgrade()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Power = 1;
        data.AttackSpeed = 1.5f;
        data.AttackRange = 6;
        data.Cost = 1;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
