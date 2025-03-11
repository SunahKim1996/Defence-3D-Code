using TMPro;
using UnityEngine;
using DG.Tweening;
using System;

public class DamageValueText : FollowMonster
{
    private void OnEnable()
    {
        // �ʱ�ȭ
        GetComponent<TMP_Text>().color = new Color(1, 1, 1, 1);
        
        // Tween ���
        float targetY = transform.localPosition.y + 30f;
        Action endAction = () =>
        {
            ObjectPoolManager.Instance.Hide(PoolKey.DamageValueText, gameObject);
        };

        Sequence sequence = UI.Instance.GetTextSequence(transform, 1f, targetY, endAction, GetComponent<TMP_Text>());
        sequence.Restart();
    }
    public void SetValue(int damage)
    {
        GetComponent<TMP_Text>().text = $"{damage}";
    }
}
