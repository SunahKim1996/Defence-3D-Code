using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TouchEvent : Singleton<TouchEvent>
{
    bool isSelecting = false;
    RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.gameState != GameState.Game)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (isSelecting) // = selectCharacter.activeInHierarchy == true
            return;

        if (Physics.Raycast(ray, out hit))
        {
            string padTag = GameManager.Instance.GetStringTagByEnum(Tag.Pad);
            if (hit.collider.tag.Equals(padTag))
            {
                // 클릭 시, 캐릭터 생성 or 업그레이드 UI 표시 
                if (Input.GetMouseButtonDown(0))
                    SettingPad();
            }
        }
    }

    void ToggleOutlineMat(bool state, GameObject obj)
    {
        // 하위 오브젝트 Renderer 도 검색 
        List<Renderer> rendererList = obj.GetComponentsInChildren<Renderer>().ToList();
        Renderer objRenderer = obj.GetComponent<Renderer>();

        if (objRenderer != null)
            rendererList.Add(objRenderer);

        for (int i = 0; i < rendererList.Count; i++)
        {
            Renderer renderers = rendererList[i].GetComponent<Renderer>();
            List<Material> matList = new List<Material>();

            matList.AddRange(renderers.sharedMaterials);

            if (state)
                matList.Add(ResourceManager.Instance.outlineMat);
            else
                matList.Remove(ResourceManager.Instance.outlineMat);

            renderers.materials = matList.ToArray();
        }
    }

    void SettingPad()
    {
        CharacterSpawnData data = SpawnManager.Instance.GetTargetCharData(hit.transform);
        UI.Instance.SpawnParent = hit.transform;
        ToggleOutlineMat(true, hit.collider.gameObject);

        // 캐릭터 생성 UI
        if (data == null || data.character == null)
            UI.Instance.ShowSelectCharacterUI();

        // 캐릭터 업그레이드 UI
        else
        {            
            ToggleOutlineMat(true, data.character);

            Character targetChar = data.character.GetComponent<Character>();
            int level = targetChar.data.Level;
            int maxLevel = targetChar.data.MaxLevel;
            int index = targetChar.data.Index;

            UI.Instance.ShowUpgradeCharacterUI(level, maxLevel, hit.transform.position);
        }        

        isSelecting = true;
    }

    public void EndSelecting()
    {
        isSelecting = false;
        ToggleOutlineMat(false, hit.collider.gameObject);
                
        CharacterSpawnData data = SpawnManager.Instance.GetTargetCharData(hit.transform);

        if (data != null)
            ToggleOutlineMat(false, data.character);
    }
}
