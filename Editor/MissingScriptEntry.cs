using UnityEngine;
using System;

[Serializable]
public class MissingScriptEntry
{
    public GameObject gameObject;
    public string gameObjectPath;
    public string assetPath;
    public string sceneName;
    public int componentIndex;
    public bool isInScene;
    public bool isInPrefab;

    public MissingScriptEntry(GameObject go, string path, string asset, int index, bool scene, bool prefab)
    {
        gameObject = go;
        gameObjectPath = path;
        assetPath = asset;
        sceneName = scene ? UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().name : "";
        componentIndex = index;
        isInScene = scene;
        isInPrefab = prefab;
    }
}
