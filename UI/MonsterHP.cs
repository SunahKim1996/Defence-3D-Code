using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHP : FollowMonster
{
    [SerializeField] private Image hpImage;
    private float hpWidth;
    private float hpHeight;

    // Start is called before the first frame update
    void Start()
    {
        hpWidth = hpImage.rectTransform.sizeDelta.x;
        hpHeight = hpImage.rectTransform.sizeDelta.y;
    }

    public void SetSize(int hp, int maxHp)
    {
        if (hpImage == null)
            return;

        float widthSize = ((float)hp / maxHp) * hpWidth;
        hpImage.rectTransform.sizeDelta = new Vector2(widthSize, hpHeight);
    }
}
