using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System;
using static Cinemachine.DocumentationSortingAttribute;
using Unity.Burst.CompilerServices;

public class UI : Singleton<UI>
{
    //[SerializeField] private List<GameObject> charObj;
    public Transform SpawnParent { get; set; }

    [Header("HUD")]
    [SerializeField] private Image goldIcon;
    [SerializeField] private TMP_Text goldText;

    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text lifeText;

    [Header("Character Select UI")]
    [SerializeField] private GameObject selectCharacterUI;
    private List<CharSelectSlot> charSelectSlotList;
    /*
    
    [SerializeField] private TMP_Text spawnGoldText;
    [SerializeField] private Image selectUI_goldIcon;
    */

    [Header("Character Upgrade UI")]
    [SerializeField] private GameObject updateCharacterUI;
    [SerializeField] private Button upgradeButton;
    [SerializeField] private Image upgradeUI_goldIcon;
    [SerializeField] private TMP_Text upgradePriceText;
    [SerializeField] private TMP_Text charLevelText;

    [Header("Notice UI")]
    [SerializeField] private GameObject noticeUI;
    [SerializeField] private TMP_Text bossNoticeUI;
    [SerializeField] private TMP_Text noticeText;
    private Coroutine noticeCor;

    [Header("Other UI")]
    [SerializeField] private TMP_Text damageValueText;
    [SerializeField] private Transform damageValueParent;

    [SerializeField] private GameObject goldValueText;
    [SerializeField] private Transform goldValueParent;

    void Start()
    {
        charSelectSlotList = selectCharacterUI.GetComponentsInChildren<CharSelectSlot>().ToList();
        ObjectPoolManager.Instance.Init(PoolKey.DamageValueText, damageValueParent, damageValueText.gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.GoldValueText, goldValueParent, goldValueText);

        StartCoroutine(SetGoldIcon());
    }

    IEnumerator SetGoldIcon()
    {
        if (ResourceManager.Instance == null)
            yield return null;

        goldIcon.sprite = ResourceManager.Instance.coinIcon;
    }

    public void InitCharSelectUI()
    {
        for (int i = 0; i < charSelectSlotList.Count; i++)
        {
            CharacterSheetData data = SpawnManager.Instance.charPrefabDataList[i].sheetData;
            CharSelectSlot slot = charSelectSlotList[i];

            slot.nameText.text = data.name;
            slot.priceIcon.sprite = ResourceManager.Instance.coinIcon;
            slot.price.text = $"{data.spawnPrice}";

            int index = i;
            int spawnPrice = data.spawnPrice;
            slot.GetComponent<Button>().onClick.AddListener(() => OnSelect(index, spawnPrice));
        }
    }

    // -----------------------------------------------------------------------
    void OnSelect(int index, int price)
    {
        SpawnManager.Instance.SpawnCharacter(index, SpawnParent, price);
        //SpawnManager.Instance.SpawnCharacter(SpawnParent);
    }

    public void OnUpgrade()
    {
        SpawnManager.Instance.UpgradeCharacter(SpawnParent);
    }

    // -----------------------------------------------------------------------
    public void RefreshGoldUI(int gold)
    {
        goldText.text = $"{gold.ToString("#,##0")}";
    }
    //TODO: 화면 보고 있을 때도 UPGARDE TEXT 색깔 변하도록 
    public void RefreshWaveUI(int curWave, int maxWave)
    {
        waveText.text = $"Wave {curWave} / {maxWave}";
        ShowNoticeUI($"{curWave} Wave 시작");
    }

    public void RefreshLifeUI(int remainLife, int maxLife)
    {
        lifeText.text = $"{remainLife} / {maxLife}";
    }

    // -----------------------------------------------------------------------
    public void RefreshPriceColor_SelectUI()
    {
        if (!selectCharacterUI.activeSelf)
            return;

        for (int i = 0; i < charSelectSlotList.Count; i++)
        {
            CharacterSheetData data = SpawnManager.Instance.charPrefabDataList[i].sheetData;
            CharSelectSlot slot = charSelectSlotList[i];

            int spawnPrice = data.spawnPrice;
            int curGold = PlayerData.Instance.Gold;
            slot.price.color = (curGold >= spawnPrice) ? Color.black : Color.red;
        }
    }

    public void ShowSelectCharacterUI()
    {        
        selectCharacterUI.SetActive(true);
        updateCharacterUI.SetActive(false);

        RefreshPriceColor_SelectUI();
    }

    public void RefreshPriceColor_UpgradeUI()
    {
        if (!updateCharacterUI.activeSelf)
            return;

        int upgradePrice = SpawnManager.Instance.GetUpgradePrice(SpawnParent);
        int curGold = PlayerData.Instance.Gold;

        upgradePriceText.color = (curGold >= upgradePrice) ? Color.black : Color.red;
    }

    public void ShowUpgradeCharacterUI(int level, int maxLevel, Vector3 hitPos)
    {
        updateCharacterUI.transform.position = Camera.main.WorldToScreenPoint(hitPos + new Vector3(0, -1f, 0));

        charLevelText.text = (level >= maxLevel) ? $"Max Lv" : $"Lv. {level}";
        charLevelText.color = (level >= maxLevel) ? Color.red : Color.black;
        upgradeButton.interactable = (level >= maxLevel) ? false : true;

        int upgradePrice = SpawnManager.Instance.GetUpgradePrice(SpawnParent);
        upgradePriceText.text = $"{upgradePrice}";

        updateCharacterUI.SetActive(true);
        selectCharacterUI.SetActive(false);

        RefreshPriceColor_UpgradeUI();
    }

    // -----------------------------------------------------------------------
    //TODO : UI Tween 
    public void ShowNoticeUI(string text, float showTime = 1.5f)
    {
        if (noticeCor != null)
        {
            StopCoroutine(noticeCor);
            noticeCor = null;
        }        

        noticeText.text = text;
        noticeUI.SetActive(true);

        noticeCor = StartCoroutine(EndNoticeUI(showTime));
    }

    IEnumerator EndNoticeUI(float showTime)
    {
        yield return new WaitForSecondsRealtime(showTime);
        noticeUI.SetActive(false);
    }

    // -----------------------------------------------------------------------
    public void ShowBossUI()
    {
        bossNoticeUI.rectTransform.anchoredPosition = new Vector3(-1000, 0, 0);
        bossNoticeUI.gameObject.SetActive(true);
        bossNoticeUI.transform.DOLocalMoveX(0, 1f)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                bossNoticeUI.transform.DOLocalMoveX(1200, 1f)
                .SetEase(Ease.InExpo)
                .OnComplete(() =>
                {
                    bossNoticeUI.gameObject.SetActive(false);
                });
            });
    }

    // -----------------------------------------------------------------------
    public void ShowDamageValueText(int damage, Transform targetTrans)
    {        
        Vector3 targetPos = Camera.main.WorldToScreenPoint(targetTrans.position + new Vector3(0, 1f, 0));
        GameObject valueText = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.DamageValueText, targetPos, Quaternion.identity);

        DamageValueText damageValueText = valueText.GetComponent<DamageValueText>();
        damageValueText.SetValue(damage);
        damageValueText.SetHeightOffset(false);
        damageValueText.SetTarget(targetTrans);
    }

    public void ShowGoldValueText(int addGold)
    {
        //Vector3 targetPos = goldIcon.rectTransform.position - new Vector3(60f, -50f, 0);
        GameObject valueText = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.GoldValueText, goldValueParent.position, Quaternion.identity);
        valueText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        //valueText.GetComponent<RectTransform>().position = targetPos;

        valueText.GetComponent<GoldValueText>().SetText(addGold);
    }

    public Sequence GetTextSequence(Transform target, float duration, float targetY, Action endAction, TMP_Text valueText = null, Image icon = null)
    {
        Sequence textSequence = DOTween.Sequence()
            .SetAutoKill(false)

            // 위로 이동 
            .OnStart(() =>
            {
                target.transform.DOLocalMoveY(targetY, duration)
                    .SetEase(Ease.OutQuad)
                    .SetUpdate(true);
            })

            
            .OnComplete(() => endAction());

        // 투명도 0 적용 
        if (valueText != null)
            textSequence.Join(valueText.DOFade(0f, duration));

        if (icon != null)
            textSequence.Join(icon.DOFade(0f, duration));

        return textSequence;
    }
}
