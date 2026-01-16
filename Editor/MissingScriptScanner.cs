using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;

public class MissingScriptScanner
{
    public List<MissingScriptEntry> ScanProject(bool includeScenes, bool includePrefabs)
    {
        var results = new List<MissingScriptEntry>();

        if (includeScenes)
        {
            results.AddRange(ScanAllScenes());
        }

        if (includePrefabs)
        {
            results.AddRange(ScanAllPrefabs());
        }

        return results;
    }

    public List<MissingScriptEntry> ScanSelected()
    {
        var results = new List<MissingScriptEntry>();
        var selectedObjects = Selection.objects;

        foreach (var obj in selectedObjects)
        {
            if (obj is GameObject go)
            {
                results.AddRange(ScanGameObject(go, GetHierarchyPath(go), AssetDatabase.GetAssetPath(go), true, false));
            }
            else if (obj is SceneAsset sceneAsset)
            {
                results.AddRange(ScanScene(AssetDatabase.GetAssetPath(sceneAsset)));
            }
            else if (AssetDatabase.IsValidFolder(AssetDatabase.GetAssetPath(obj)))
            {
                results.AddRange(ScanFolder(AssetDatabase.GetAssetPath(obj)));
            }
        }

        return results;
    }

    private List<MissingScriptEntry> ScanAllScenes()
    {
        var results = new List<MissingScriptEntry>();
        var sceneGuids = AssetDatabase.FindAssets("t:Scene");

        foreach (var guid in sceneGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            results.AddRange(ScanScene(path));
        }

        return results;
    }

    private List<MissingScriptEntry> ScanAllPrefabs()
    {
        var results = new List<MissingScriptEntry>();
        var prefabGuids = AssetDatabase.FindAssets("t:Prefab");

        foreach (var guid in prefabGuids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                results.AddRange(ScanGameObject(prefab, prefab.name, path, false, true));
            }
        }

        return results;
    }

    private List<MissingScriptEntry> ScanScene(string scenePath)
    {
        var results = new List<MissingScriptEntry>();

        var scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);
        var rootObjects = scene.GetRootGameObjects();

        foreach (var root in rootObjects)
        {
            results.AddRange(ScanGameObjectRecursive(root, root.name, scenePath, true, false));
        }

        EditorSceneManager.CloseScene(scene, true);
        return results;
    }

    private List<MissingScriptEntry> ScanFolder(string folderPath)
    {
        var results = new List<MissingScriptEntry>();
        var guids = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab != null)
            {
                results.AddRange(ScanGameObject(prefab, prefab.name, path, false, true));
            }
        }

        return results;
    }

    private List<MissingScriptEntry> ScanGameObject(GameObject go, string path, string assetPath, bool isScene, bool isPrefab)
    {
        return ScanGameObjectRecursive(go, path, assetPath, isScene, isPrefab);
    }

    private List<MissingScriptEntry> ScanGameObjectRecursive(GameObject go, string currentPath, string assetPath, bool isScene, bool isPrefab)
    {
        var results = new List<MissingScriptEntry>();

        var components = go.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
        {
            if (components[i] == null)
            {
                results.Add(new MissingScriptEntry(go, currentPath, assetPath, i, isScene, isPrefab));
            }
        }

        foreach (Transform child in go.transform)
        {
            results.AddRange(ScanGameObjectRecursive(child.gameObject, currentPath + "/" + child.name, assetPath, isScene, isPrefab));
        }

        return results;
    }

    private string GetHierarchyPath(GameObject go)
    {
        var path = go.name;
        var parent = go.transform.parent;

        while (parent != null)
        {
            path = parent.name + "/" + path;
            parent = parent.parent;
        }

        return path;
    }
}
