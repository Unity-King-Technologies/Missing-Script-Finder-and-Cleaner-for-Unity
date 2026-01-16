using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class MissingScriptCleaner
{
    public void RemoveAll(List<MissingScriptEntry> entries)
    {
        var groupedByAsset = GroupEntriesByAsset(entries);

        foreach (var group in groupedByAsset)
        {
            RemoveFromAsset(group.Key, group.Value);
        }
    }

    public void RemoveSingle(MissingScriptEntry entry)
    {
        RemoveFromAsset(entry, new List<MissingScriptEntry> { entry });
    }

    private void RemoveFromAsset(string assetPath, List<MissingScriptEntry> entries)
    {
        if (string.IsNullOrEmpty(assetPath))
        {
            // Scene objects
            RemoveFromScene(entries);
        }
        else if (assetPath.EndsWith(".prefab"))
        {
            RemoveFromPrefab(assetPath, entries);
        }
    }

    private void RemoveFromScene(List<MissingScriptEntry> entries)
    {
        var sceneGroups = new Dictionary<string, List<MissingScriptEntry>>();

        foreach (var entry in entries)
        {
            if (!sceneGroups.ContainsKey(entry.assetPath))
                sceneGroups[entry.assetPath] = new List<MissingScriptEntry>();
            sceneGroups[entry.assetPath].Add(entry);
        }

        foreach (var sceneGroup in sceneGroups)
        {
            var scene = EditorSceneManager.OpenScene(sceneGroup.Key, OpenSceneMode.Single);
            RemoveComponentsFromGameObjects(sceneGroup.Value);
            EditorSceneManager.SaveScene(scene);
        }
    }

    private void RemoveFromPrefab(string prefabPath, List<MissingScriptEntry> entries)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
        if (prefab == null) return;

        var prefabInstance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
        if (prefabInstance == null) return;

        RemoveComponentsFromGameObjects(entries);

        PrefabUtility.ApplyPrefabInstance(prefabInstance, InteractionMode.AutomatedAction);
        Object.DestroyImmediate(prefabInstance);
    }

    private void RemoveComponentsFromGameObjects(List<MissingScriptEntry> entries)
    {
        foreach (var entry in entries)
        {
            if (entry.gameObject == null) continue;

            Undo.RecordObject(entry.gameObject, "Remove Missing Script");

            var components = entry.gameObject.GetComponents<Component>();
            if (entry.componentIndex < components.Length && components[entry.componentIndex] == null)
            {
                // Find the actual missing component by index
                var serializedObject = new SerializedObject(entry.gameObject);
                var componentsProperty = serializedObject.FindProperty("m_Component");

                if (entry.componentIndex < componentsProperty.arraySize)
                {
                    componentsProperty.DeleteArrayElementAtIndex(entry.componentIndex);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }

    private Dictionary<string, List<MissingScriptEntry>> GroupEntriesByAsset(List<MissingScriptEntry> entries)
    {
        var groups = new Dictionary<string, List<MissingScriptEntry>>();

        foreach (var entry in entries)
        {
            var key = entry.assetPath ?? "scene";
            if (!groups.ContainsKey(key))
                groups[key] = new List<MissingScriptEntry>();
            groups[key].Add(entry);
        }

        return groups;
    }
}
