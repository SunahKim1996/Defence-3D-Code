using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Init,
    Game,
    Finish,
}

public enum Tag
{
    Pad,
    Monster,
}

public class GameManager : Singleton<GameManager>
{
    [HideInInspector] public GameState gameState = GameState.Init; 

    //Wave
    private int maxWave;
    private int wave = 1;
    public int Wave
    {
        get { return wave; }
        set
        {
            wave = value;

            //TODO: maxWave 넘었을 때 처리 필요 
            if (wave > maxWave)
                wave = 1; 

            UI.Instance.RefreshWaveUI(wave, maxWave);
        }
    }

    public List<Wave> waveData = new List<Wave>();

    //FX
    [SerializeField] private Transform charUpgradeFXParent;
    [SerializeField] private Transform castleDestroyFXParent;

    [HideInInspector] public bool isSheetEndLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        // FX Pool 초기 세팅 
        GameObject charUpgradeFX = ResourceManager.Instance.charUpgradeFX;
        ObjectPoolManager.Instance.Init(PoolKey.CharUpgradeFx, charUpgradeFXParent, charUpgradeFX);
        
        GameObject castleDestroyFX = ResourceManager.Instance.castleDestroyFX;
        ObjectPoolManager.Instance.Init(PoolKey.CastleDestroyFX, castleDestroyFXParent, castleDestroyFX);

        InitGame();
    }

    public void InitGame()
    {
        maxWave = waveData.Count;
        Wave = 1;

        PlayerData.Instance.Initialize();
        SpawnManager.Instance.SetCurWaveData();

        RefreshCastle(0);
    }

    public string GetStringTagByEnum(Tag targetTag) => Enum.GetName(typeof(Tag), targetTag);

    public void RefreshCastle(int castleLevel, bool isAllFalse = false)
    {
        List<GameObject> castleList = ResourceManager.Instance.castleList;

        for (int i = 0; i < castleList.Count; i++)
        {
            bool state = isAllFalse ? false : (i == castleLevel) ? true : false;
            castleList[i].SetActive(state);
        }

        if (castleLevel <= 0)
            return;

        Vector3 pos = castleList[0].transform.position + new Vector3(0, 1.8f, 0);
        Quaternion rot = castleList[0].transform.rotation;
        ObjectPoolManager.Instance.ShowObjectPool(PoolKey.CastleDestroyFX, pos, rot, true, 1.5f);
    }

    public void CameraShake(float ShakeAmount, float ShakeTime)
    {
        StartCoroutine(StartCameraShake(ShakeAmount, ShakeTime));
    }

    IEnumerator StartCameraShake(float ShakeAmount, float ShakeTime)
    {
        float timer = 0;
        while (timer <= ShakeTime)
        {
            Camera.main.transform.position =
                (Vector3)UnityEngine.Random.insideUnitCircle * ShakeAmount;
            timer += Time.deltaTime;
            yield return null;
        }
        Camera.main.transform.position = new Vector3(0f, 0f, 0f);
    }

    public void EndGame()
    {
        gameState = GameState.Finish;

        //TODO : 패배 시 처리 (캐릭터 죽는 모션) 
        GameManager.Instance.RefreshCastle(3, true);

        List<CharacterSpawnData> characterSpawnList = SpawnManager.Instance.characterSpawnList;

        for (int i = 0; i < characterSpawnList.Count; i++)
        {
            characterSpawnList[i].character.GetComponent<Animator>().SetTrigger("dead");
        }
    }
}
