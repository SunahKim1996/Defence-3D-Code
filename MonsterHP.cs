using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterHP : MonoBehaviour
{
    public Transform target;

    [SerializeField] private Image hpImage;
    private float hpWidth;
    private float hpHeight;

    // Start is called before the first frame update
    void Start()
    {
        hpWidth = hpImage.rectTransform.sizeDelta.x;
        hpHeight = hpImage.rectTransform.sizeDelta.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
        {
            transform.position = Camera.main.WorldToScreenPoint(target.position + new Vector3(0, 1f, 0));
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetSize(int hp, int maxHp)
    {
        if (hpImage == null)
            return;

        float widthSize = ((float)hp / maxHp) * hpWidth;
        hpImage.rectTransform.sizeDelta = new Vector2(widthSize, hpHeight);
    }
}
