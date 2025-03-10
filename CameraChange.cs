using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChange : MonoBehaviour
{
    [SerializeField] private CinemachineBrain brainCam;
    [SerializeField] private Camera[] camList;
    private int camIndex, preCamIndex = 0;

    private Coroutine changingCor;

    [HideInInspector] public bool isChanging;

    public void OnChangeCameraRight()
    {
        preCamIndex = camIndex;
        camIndex++;

        if (camIndex >= camList.Length)
            camIndex = 0;

        ChangeCamera();
    }

    public void OnChangeCameraLeft()
    {
        preCamIndex = camIndex;
        camIndex--;

        if (camIndex < 0)
            camIndex = camList.Length - 1;

        ChangeCamera();
    }

    void ChangeCamera()
    {
        isChanging = true;

        if (changingCor != null) 
        { 
            StopCoroutine(changingCor);
            changingCor = null;
        }

        for (int i = 0; i < camList.Length; i++)
        {
            bool state = (i == camIndex) ? true : false;
            camList[i].gameObject.SetActive(state);
        }

        changingCor = StartCoroutine(Changing());
    }

    IEnumerator Changing()
    {        
        CinemachineVirtualCamera vCam = camList[preCamIndex].GetComponent<CinemachineVirtualCamera>();

        while (CinemachineCore.Instance.IsLive(vCam))
            yield return null;

        isChanging = false;
    }
}
