using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : Singleton<UI>
{
    [SerializeField] private List<GameObject> charObj;
    public Transform SpawnParent { get; set; }

    public TMP_Text goldText;
    public TMP_Text waveText;
    public TMP_Text lifeText;

    private int gold = 0;
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            goldText.text = $"{gold.ToString("#,##0")}";
        }
    }

    private int wave = 1;
    private int maxWave = 2;
    public int Wave
    {
        get { return wave; }
        set
        {
            wave = value;
            waveText.text = $"Wave {wave}/{maxWave}";
        }
    }

    private int life = 0;
    private int maxLife = 10;
    public int Life
    {
        get { return life; }
        set
        {
            life = value;
            lifeText.text = $"{life}/{maxLife}";

            if(life <= 0)
            {
                Time.timeScale = 0;
            }
        }
    }

    void Start()
    {
        Initialized();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Gold += 10;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            Wave++;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            Life--;
        }
    }

    public void Initialized()
    {
        Gold = 100;
        Wave = 1;
        Life = maxLife;

        SpawnManager.Instance.Initialize();
    }

    public void OnSelect(int index)
    {
        SpawnManager.Instance.SpawnCharacter(charObj[index], SpawnParent);     
    }

    public void OnUpgrade()
    {
        SpawnManager.Instance.UpgradeCharacter(SpawnParent); 
    }
}
