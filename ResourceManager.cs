using System.Collections.Generic;
using UnityEngine;


public class ResourceManager : Singleton<ResourceManager>
{
    [Header("Icon")]
    public Sprite coinIcon;
    public Sprite lifeIcon;

    [Header("Material")]
    public List<Material> rankMatList;
    [Space(10f)]
    [HideInInspector] public Material outlineMat;

    [Header("Castle")]
    public List<GameObject> castleList;

    void Start()
    {
        outlineMat = new Material(Shader.Find("Draw/OutlineShader"));
    }
}
