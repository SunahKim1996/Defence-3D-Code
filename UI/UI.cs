using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using DG.Tweening;
using System.Linq;
using System;

public class UI : Singleton<UI>
{
    //[SerializeField] private List<GameObject> charObj;
    public Transform SpawnParent { get; set; }

    [Header("HUD")]
    [SerializeField] private Image goldIcon;
    [SerializeField] private TMP_Text goldText;

    [SerializeField] private TMP_Text waveText;

    [SerializeField] private Image lifeIcon;
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

    [Header("Effect UI")]
    [SerializeField] private TMP_Text damageValueText;
    [SerializeField] private Transform damageValueParent;
    [Space(10f)]
    [SerializeField] private GameObject goldTextEffect;
    [SerializeField] private Transform goldEffectParent;
    [Space(10f)]
    [SerializeField] private GameObject lifeTextEffect;
    [SerializeField] private Transform lifeEffectParent;

    [Header("Other UI")]
    [SerializeField] private GameObject repairUI;
    [SerializeField] private Image repairUI_GoldIcon;
    [SerializeField] private TMP_Text repairPriceText;


    void Start()
    {
        charSelectSlotList = selectCharacterUI.GetComponentsInChildren<CharSelectSlot>().ToList();
        ObjectPoolManager.Instance.Init(PoolKey.DamageValueText, damageValueParent, damageValueText.gameObject);
        ObjectPoolManager.Instance.Init(PoolKey.GoldTextEffect, goldEffectParent, goldTextEffect);
        ObjectPoolManager.Instance.Init(PoolKey.LifeTextEffect, lifeEffectParent, lifeTextEffect);

        StartCoroutine(SetInitIcon());
    }

    IEnumerator SetInitIcon()
    {
        if (ResourceManager.Instance == null)
            yield return null;

        goldIcon.sprite = ResourceManager.Instance.coinIcon;
        repairUI_GoldIcon.sprite = ResourceManager.Instance.coinIcon;
        upgradeUI_goldIcon.sprite = ResourceManager.Instance.coinIcon;

        lifeIcon.sprite = ResourceManager.Instance.lifeIcon;
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
 
    public void RefreshWaveUI(int curWave, int maxWave)
    {
        waveText.text = $"Wave {curWave} / {maxWave}";
        ShowNoticeUI($"{curWave} Wave 시작");
    }

    public void RefreshLifeUI(int remainLife, int maxLife)
    {
        lifeText.text = $"{remainLife} / {maxLife}";
    }

    // 캐릭터 선택 UI -----------------------------------------------------------------------
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

    // 캐릭터 업그레이드 UI -----------------------------------------------------------------------
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


        string upgradePriceString = (level >= maxLevel) ? "-" : $"{SpawnManager.Instance.GetUpgradePrice(SpawnParent)}";
        upgradePriceText.text = upgradePriceString;

        updateCharacterUI.SetActive(true);
        selectCharacterUI.SetActive(false);

        if (level < maxLevel)
            RefreshPriceColor_UpgradeUI();
    }

    // 성 수리 UI -----------------------------------------------------------------------
    public void ToggleRepairUI(bool state)
    {
        repairUI.SetActive(state);
    }

    public void RefreshRepairPriceText()
    {
        int repairPrice = CastleManager.Instance.repairPrice;
        int curGold = PlayerData.Instance.Gold;

        repairPriceText.text = $"{repairPrice}";
        repairPriceText.color = (curGold < repairPrice) ? Color.red : Color.black;
    }

    // -----------------------------------------------------------------------
    public void ShowNoticeUI(string text, float showTime = 1.5f)
    {
        if (noticeCor != null)
        {
            StopCoroutine(noticeCor);
            noticeCor = null;
        }        

        noticeText.text = text;

        noticeUI.GetComponent<RectTransform>().localScale = new Vector2(0, 1f);
        noticeUI.transform.DOScaleX(1f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.OutBack);

        noticeUI.SetActive(true);

        noticeCor = StartCoroutine(EndNoticeUI(showTime));
    }

    IEnumerator EndNoticeUI(float showTime)
    {
        yield return new WaitForSecondsRealtime(showTime);

        noticeUI.transform.DOScaleX(0f, 0.5f)
            .SetUpdate(true)
            .SetEase(Ease.InBack)
            .OnComplete(() => noticeUI.SetActive(false));        
    }

    // -----------------------------------------------------------------------
    public void ShowBossUI()
    {
        bossNoticeUI.rectTransform.anchoredPosition = new Vector3(-1000, -150, 0);
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

    // Text 효과 -----------------------------------------------------------------------
    public void ShowDamageValueText(int damage, Transform targetTrans)
    {        
        Vector3 targetPos = Camera.main.WorldToScreenPoint(targetTrans.position + new Vector3(0, 1f, 0));
        GameObject valueText = ObjectPoolManager.Instance.ShowObjectPool(PoolKey.DamageValueText, targetPos, Quaternion.identity);

        DamageValueText damageValueText = valueText.GetComponent<DamageValueText>();
        damageValueText.SetValue(damage);
        damageValueText.SetHeightOffset(false);
        damageValueText.SetTarget(targetTrans);
    }

    public void ShowTextEffect(PoolKey poolKey, int addValue, Sprite icon, float heightDis)
    {
        Transform targetParent = (poolKey == PoolKey.GoldTextEffect) ? goldEffectParent : lifeEffectParent;
        GameObject valueText = ObjectPoolManager.Instance.ShowObjectPool(poolKey, targetParent.position, Quaternion.identity);
        valueText.GetComponent<RectTransform>().anchoredPosition = Vector3.zero;

        TextEffect effect = valueText.GetComponent<TextEffect>();
        effect.SetText(addValue);
        effect.SetIcon(icon);
        effect.SetPoolKey(poolKey);
        effect.SetHeightDis(heightDis);

        effect.EffectStart();
    }

    public Sequence GetTextSequence(Transform target, float duration, float targetY, Action endAction, TMP_Text valueText = null, Image icon = null)
    {
        Sequence textSequence = DOTween.Sequence()

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
