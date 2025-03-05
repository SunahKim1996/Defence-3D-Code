using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WarrirorCharacter : Character
{
    [SerializeField] private List<GameObject> upgradeObj;
    public override void Upgrade()
    {
        for (int i = 0; i < upgradeObj.Count; i++) 
        {
            upgradeObj[i].SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        data.Power = 10;
        data.AttackSpeed = 1;
        data.AttackRange = 3;
        data.Cost = 1;
        data.Level = 1;

        animator = GetComponent<Animator>();
    }
}
