using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEvent : Singleton<TouchEvent>
{
    [SerializeField] private GameObject selectCharacter;
    [SerializeField] private GameObject updateCharacter;

    bool isSelecting = false;

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (isSelecting) // UI On Off 여부로 판단하는 것도 가능 : selectCharacter.activeInHierarchy == true
            return;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.tag.Equals("Pad"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    UI.Instance.SpawnParent = hit.transform;

                    // 캐릭터 생성
                    if (hit.transform.childCount == 0)
                    {
                        selectCharacter.transform.position = Camera.main.WorldToScreenPoint(
                            hit.transform.position + new Vector3(0, 1f, 0));
                        selectCharacter.SetActive(true);
                    }

                    // 캐릭터 업그레이드 
                    else
                    {
                        updateCharacter.transform.position = Camera.main.WorldToScreenPoint(
                            hit.transform.position + new Vector3(0, 1f, 0));
                        updateCharacter.SetActive(true);
                    }

                    isSelecting = true;
                }
            }
        }
    }

    public void EndSelecting()
    {
        isSelecting = false;
    }
}
