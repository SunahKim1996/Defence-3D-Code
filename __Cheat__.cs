using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class __Cheat__ : MonoBehaviour
{
    //TODO: ġƮ ����� 
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            PlayerData.Instance.Gold += 1000;
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            GameManager.Instance.Wave++;
        }

        if (Input.GetKeyDown(KeyCode.F3))
        {
            PlayerData.Instance.Life--;
        }
    }
}
