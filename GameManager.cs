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
    [HideInInspector] public bool isSheetEndLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        InitGame();
    }

    public void InitGame()
    {
        maxWave = waveData.Count;
        Wave = 1;

        PlayerData.Instance.Initialize();
        CastleManager.Instance.Initialize();
        SpawnManager.Instance.SetCurWaveData();
    }

    public string GetStringTagByEnum(Tag targetTag) => Enum.GetName(typeof(Tag), targetTag);

    public void ChangeNextWave()
    {
        Wave++;
        SpawnManager.Instance.SetCurWaveData();
        SpawnManager.Instance.isBossTime = false;
    }

    //TODO : Life 닳을 때 카메라 흔들리는 연출 
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

        // 캐릭터 사망 모션 
        List<CharacterSpawnData> characterSpawnList = SpawnManager.Instance.characterSpawnList;
        for (int i = 0; i < characterSpawnList.Count; i++)
            characterSpawnList[i].character.GetComponent<Animator>().SetTrigger("dead");
    }
}
