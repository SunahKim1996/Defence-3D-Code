using UnityEngine;

// Monster ��ġ�� ����ٴϴ� ������Ʈ���� �θ� ��ũ��Ʈ 
public class FollowMonster : MonoBehaviour
{
    Transform target;
    Vector3 heightOffset;
    bool isUseHeight;

    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Game)
            return;

        if (target != null)
        {
            Vector3 newPos = Camera.main.WorldToScreenPoint(target.position + heightOffset);
            Vector3 targetPos = isUseHeight ? newPos : new Vector3(newPos.x, transform.position.y, newPos.z);

            transform.position = targetPos;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    /// <param name="isUseHeight"></param> target �� y ��ġ�� ������ �ڽ��� y ��ġ�� ������� ���� 
    /// <param name="heightOffset"></param> isUseHeight �� true �� ���, y ��ġ Offset ���� 
    public void SetHeightOffset(bool isUseHeight, Vector3 heightOffset = default)
    {
        this.isUseHeight = isUseHeight;
        this.heightOffset = heightOffset;
    }
}
