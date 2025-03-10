using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class UpgradeData
{
    public int price;
    public int powerUpgradeValue;
    public float attackRangeUpgradeValue;
    public float attackSpeedUpgradeValue;
}

[System.Serializable]
public class CharacterSheetData
{
    public int index;
    public string name;
    public int spawnPrice;

    public List<UpgradeData> upgradeDatas;
}

public class CharacterSheetLoader : Singleton<CharacterSheetLoader>
{
    List<CharacterSheetData> characterSheetData;

    void Start()
    {
        characterSheetData = new List<CharacterSheetData>();
        const string url = "https://docs.google.com/spreadsheets/d/1EaPjYw6SizggF6OpLSIdTuM6HJVJobzcN9hclPmktdY/export?format=tsv&gid=0";
        StartCoroutine(LoadData(url));
    }

    IEnumerator LoadData(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.isDone)
            {
                string data = www.downloadHandler.text;
                string[] rows = data.Split('\n');

                //0번째는 타이틀이므로 1부터 시작
                for (int i = 1; i < rows.Length; i++)
                {
                    string[] columns = rows[i].Split('\t');

                    CharacterSheetData sheetData = new CharacterSheetData();

                    UpgradeData upgradeData1 = new UpgradeData();
                    UpgradeData upgradeData2 = new UpgradeData();

                    for (int j = 1; j < columns.Length; j++) 
                    {                        
                        string value = columns[j];
                        value = value.Replace("\r", "");
                        
                        switch (j)
                        {
                            case 0: // ID
                                sheetData.index = int.Parse(columns[j]);
                                break;
                            case 1: // Name 
                                sheetData.name = columns[j];
                                break;
                            case 2: // SpawnPrice
                                sheetData.spawnPrice = int.Parse(value);
                                break;
                            case 3: // UpgradePrice1
                                upgradeData1.price = int.Parse(value);
                                break;
                            case 4: // PowerUpgradeValue1
                                upgradeData1.powerUpgradeValue = int.Parse(value);
                                break;
                            case 5: // AttackSpeedUpgradeValue1
                                upgradeData1.attackSpeedUpgradeValue = float.Parse(value);
                                break;
                            case 6: // AttackRangeUpgradeValue1
                                upgradeData1.attackRangeUpgradeValue = float.Parse(value);
                                break;
                            case 7: // UpgradePrice2
                                upgradeData2.price = int.Parse(value);
                                break;
                            case 8: // PowerUpgradeValue2
                                upgradeData2.powerUpgradeValue = int.Parse(value);
                                break;
                            case 9: // AttackSpeedUpgradeValue2
                                upgradeData2.attackSpeedUpgradeValue = float.Parse(value);
                                break;
                            case 10: // AttackRangeUpgradeValue2
                                upgradeData2.attackRangeUpgradeValue = float.Parse(value);
                                break;
                        }
                    }

                    sheetData.upgradeDatas = new List<UpgradeData>();
                    sheetData.upgradeDatas.Add(upgradeData1);
                    sheetData.upgradeDatas.Add(upgradeData2);

                    characterSheetData.Add(sheetData);
                }
            }
        }

        GameManager.Instance.isSheetEndLoading = true;
        GameManager.Instance.gameState = GameState.Game;

        SpawnManager.Instance.InitCharPrefabData(characterSheetData);

        Debug.Log("Shee Load Complete =================================");
    }
}
