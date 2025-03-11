using UnityEngine;

public class FX : MonoBehaviour
{
    [Header("Character FX")]
    [SerializeField] private GameObject charSpawnFX;
    [SerializeField] private GameObject charUpgradeFX;

    [SerializeField] private Transform charFXParent;

    [Header("Monster FX")]
    [SerializeField] private GameObject monsterDeadFX;
    [SerializeField] private Transform monsterDeadFXParent;

    [Header("Castle Destroy FX")]
    [SerializeField] private GameObject castleDestroyFX;
    [SerializeField] private Transform castleDestroyFXParent;

    // Start is called before the first frame update
    void Start()
    {
        // FX Pool 초기 세팅 
        ObjectPoolManager.Instance.Init(PoolKey.CharSpawnFx, charFXParent, charSpawnFX);
        ObjectPoolManager.Instance.Init(PoolKey.CharUpgradeFx, charFXParent, charUpgradeFX);

        ObjectPoolManager.Instance.Init(PoolKey.CastleDestroyFX, castleDestroyFXParent, castleDestroyFX);

        ObjectPoolManager.Instance.Init(PoolKey.MonsterDeadFX, monsterDeadFXParent, monsterDeadFX);
    }
}
