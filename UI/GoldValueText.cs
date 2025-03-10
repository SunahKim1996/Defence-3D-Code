using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class GoldValueText : MonoBehaviour
{
    public TMP_Text valueText;
    [SerializeField] private Image goldIcon;

    void Start()
    {
        goldIcon.sprite = ResourceManager.Instance.coinIcon;
    }

    // 호출 순서 참고 : OnEnable -> Start 
    void OnEnable()
    {
        // 초기화 
        valueText.color = new Color(1, 1, 1, 1); 
        goldIcon.color = new Color(1, 1, 1, 1);

        // 트윈 재생
        float targetY = transform.localPosition.y + 30f;
        Action endAction = () =>
        {
            ObjectPoolManager.Instance.Hide(PoolKey.GoldValueText, gameObject);
        };

        Sequence sequence = UI.Instance.GetTextSequence(transform, 1f, targetY, endAction, valueText, goldIcon);
        sequence.Restart();
    }

    public void SetText(int value)
    {
        valueText.text = $"+{value}";
    }
}
