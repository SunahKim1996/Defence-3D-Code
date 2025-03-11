using UnityEngine;

// Monster 위치를 따라다니는 오브젝트들의 부모 스크립트 
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

    /// <param name="isUseHeight"></param> target 의 y 위치를 따라갈지 자신의 y 위치를 사용할지 여부 
    /// <param name="heightOffset"></param> isUseHeight 가 true 인 경우, y 위치 Offset 설정 
    public void SetHeightOffset(bool isUseHeight, Vector3 heightOffset = default)
    {
        this.isUseHeight = isUseHeight;
        this.heightOffset = heightOffset;
    }
}
