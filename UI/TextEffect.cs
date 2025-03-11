using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;

public class TextEffect : MonoBehaviour
{
    [SerializeField] private TMP_Text valueText;
    [SerializeField] private Image icon;
    private PoolKey poolKey;
    private float heightDis;

    public void EffectStart()
    {
        // 초기화 
        GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
        valueText.color = new Color(1, 1, 1, 1);
        icon.color = new Color(1, 1, 1, 1);

        // 트윈 재생
        float targetY = heightDis;
        Action endAction = () =>
        {
            ObjectPoolManager.Instance.Hide(poolKey, gameObject);
        };

        Sequence sequence = UI.Instance.GetTextSequence(transform, 1f, targetY, endAction, valueText, icon);
        sequence.Restart();
    }

    public void SetIcon(Sprite iconSprite) 
    {
        icon.sprite = iconSprite;
    }

    public void SetText(int value)
    {
        string signText = (value > 0) ? "+" : "";
        valueText.text = $"{signText}{value}";
    }

    public void SetPoolKey(PoolKey poolKey)
    {
        this.poolKey = poolKey;
    }

    /// <summary>
    /// y 이동 거리 
    /// </summary>
    public void SetHeightDis(float heightDis)
    {
        this.heightDis = heightDis;
    }
}
