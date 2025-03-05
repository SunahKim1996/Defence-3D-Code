using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Direction
{
    Up = 0,
    Right = 90,
    Down = 180,
    Left = 270,
}

public class Monster : MonoBehaviour
{
    private Direction dir = Direction.Right;
    private float speed = 2f;

    private List<Flag> flags;
    private int pointIdx = 0;

    public MonsterHP HpUI { get; set; }
    private int hp;
    private int maxHp;

    // Start is called before the first frame update
    void Start()
    {
        hp = maxHp = 20;
    }
    
    // Update is called once per frame
    void Update()
    {
        // 값이 없을 때 예외처리 
        if (flags == null || flags.Count == 0)
            return;

        //transform.Translate(Vector3.right * Time.deltaTime * speed);
        transform.position = Vector3.MoveTowards(
            transform.position, 
            flags[pointIdx].transform.position, 
            Time.deltaTime * speed);

        float distance = Vector3.Distance(
            transform.position, 
            flags[pointIdx].transform.position);

        if (distance <= 0)
        {
            pointIdx++;

            if (pointIdx >= flags.Count)
            {
                UI.Instance.Life--;
                DestroyMonster();
            }                
            else
                SetDirecion(flags[pointIdx].dir);
        }
    }

    public void SetDirecion(Direction dir)
    {
        this.dir = dir;
        transform.localRotation = Quaternion.Euler(new Vector3(0f, (float)this.dir, 0f));
    }

    public void SetFlag(List<Flag> flags) 
    { 
        this.flags = flags;
    }

    public void Damage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
            DestroyMonster();

        HpUI.SetSize(hp, maxHp);
    }

    void DestroyMonster()
    {
        try
        {
            Destroy(HpUI.gameObject);
            Destroy(gameObject);                
        }
        catch (MissingReferenceException e) 
        { 
        
        }
    }
}
