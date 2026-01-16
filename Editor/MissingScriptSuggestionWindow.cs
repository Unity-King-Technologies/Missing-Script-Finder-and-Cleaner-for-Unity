using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class MissingScriptSuggestionWindow : EditorWindow
{
    private List<MissingScriptEntry> entries = new List<MissingScriptEntry>();
    private Vector2 scrollPos;
    private Dictionary<MissingScriptEntry, List<MonoScript>> suggestions = new Dictionary<MissingScriptEntry, List<MonoScript>>();

    public void SetEntries(List<MissingScriptEntry> newEntries)
    {
        entries = newEntries;
        FindAllSuggestions();
    }

    void OnGUI()
    {
        EditorGUILayout.LabelField("Missing Script Suggestions", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("This tool suggests similar scripts that might replace the missing ones. Review carefully before applying!", MessageType.Warning);

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

        foreach (var entry in entries)
        {
            DrawEntrySuggestions(entry);
            EditorGUILayout.Space();
        }

        EditorGUILayout.EndScrollView();

        if (GUILayout.Button("Apply All Suggestions", GUILayout.Height(30)))
        {
            ApplyAllSuggestions();
        }
    }

    void DrawEntrySuggestions(MissingScriptEntry entry)
    {
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField($"GameObject: {entry.gameObjectPath}", EditorStyles.boldLabel);
        EditorGUILayout.LabelField($"Asset: {entry.assetPath}", EditorStyles.miniLabel);

        if (suggestions.ContainsKey(entry) && suggestions[entry].Count > 0)
        {
            EditorGUILayout.LabelField("Suggested Scripts:", EditorStyles.miniBoldLabel);

            foreach (var script in suggestions[entry])
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(script.name, GUILayout.Width(200));
                EditorGUILayout.LabelField(AssetDatabase.GetAssetPath(script), EditorStyles.miniLabel);

                if (GUILayout.Button("Apply", GUILayout.Width(60)))
                {
                    ApplySuggestion(entry, script);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No suggestions found.", EditorStyles.miniLabel);
        }

        EditorGUILayout.EndVertical();
    }

    void FindAllSuggestions()
    {
        suggestions.Clear();

        foreach (var entry in entries)
        {
            // For demonstration, we'll suggest scripts with similar names
            // In a real implementation, you might analyze the component requirements
            var similarScripts = AssetUtility.FindSimilarScripts(""); // Empty string to get all, but filter by context
            suggestions[entry] = similarScripts.Take(3).ToList(); // Limit to 3 suggestions
        }
    }

    void ApplySuggestion(MissingScriptEntry entry, MonoScript script)
    {
        if (entry.gameObject == null) return;

        Undo.RecordObject(entry.gameObject, "Replace Missing Script");

        // Add the suggested script
        var component = entry.gameObject.AddComponent(script.GetClass());
        if (component != null)
        {
            // Remove the missing script by index
            var serializedObject = new SerializedObject(entry.gameObject);
            var componentsProperty = serializedObject.FindProperty("m_Component");

            if (entry.componentIndex < componentsProperty.arraySize)
            {
                componentsProperty.DeleteArrayElementAtIndex(entry.componentIndex);
                serializedObject.ApplyModifiedProperties();
            }

            Debug.Log($"Replaced missing script with {script.name} on {entry.gameObjectPath}");
            EditorUtility.SetDirty(entry.gameObject);
        }
    }

    void ApplyAllSuggestions()
    {
        if (!EditorUtility.DisplayDialog("Apply All Suggestions",
            "This will replace all missing scripts with the first suggestion for each. Are you sure?",
            "Yes", "Cancel"))
            return;

        foreach (var entry in entries)
        {
            if (suggestions.ContainsKey(entry) && suggestions[entry].Count > 0)
            {
                ApplySuggestion(entry, suggestions[entry][0]);
            }
        }

        Close();
    }
}
