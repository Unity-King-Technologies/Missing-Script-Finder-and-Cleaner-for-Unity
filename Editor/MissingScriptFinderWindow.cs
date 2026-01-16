using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.IMGUI.Controls;

public class MissingScriptFinderWindow : EditorWindow
{
    [SerializeField] private TreeViewState treeViewState;
    private MissingScriptTreeView treeView;
    private SearchField searchField;
    private string searchString = "";

    private MissingScriptScanner scanner;
    private MissingScriptCleaner cleaner;

    private Vector2 scrollPos;
    private bool showScenes = true;
    private bool showPrefabs = true;
    private bool showSelectedOnly = false;

    [MenuItem("Tools/Missing Script Finder & Cleaner")]
    static void ShowWindow()
    {
        var window = GetWindow<MissingScriptFinderWindow>();
        window.titleContent = new GUIContent("Missing Script Cleaner", EditorGUIUtility.IconContent("d_console.warnicon").image);
        window.Show();
    }

    void OnEnable()
    {
        scanner = new MissingScriptScanner();
        cleaner = new MissingScriptCleaner();

        if (treeViewState == null)
            treeViewState = new TreeViewState();

        treeView = new MissingScriptTreeView(treeViewState);
        searchField = new SearchField();
    }

    void OnGUI()
    {
        DrawToolbar();

        EditorGUILayout.Space();

        if (GUILayout.Button("Scan Project", GUILayout.Height(30)))
        {
            ScanProject();
        }

        EditorGUILayout.Space();

        DrawFilters();

        EditorGUILayout.Space();

        DrawResults();
    }

    void DrawToolbar()
    {
        EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

        if (GUILayout.Button("Scan Selected", EditorStyles.toolbarButton, GUILayout.Width(100)))
        {
            ScanSelected();
        }

        searchString = searchField.OnToolbarGUI(searchString);

        EditorGUILayout.EndHorizontal();
    }

    void DrawFilters()
    {
        EditorGUILayout.LabelField("Filters", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        showScenes = EditorGUILayout.Toggle("Scenes", showScenes);
        showPrefabs = EditorGUILayout.Toggle("Prefabs", showPrefabs);
        showSelectedOnly = EditorGUILayout.Toggle("Selected Only", showSelectedOnly);
        EditorGUILayout.EndHorizontal();
    }

    void DrawResults()
    {
        EditorGUILayout.LabelField($"Missing Scripts Found: {treeView.GetRowCount()}", EditorStyles.boldLabel);

        if (treeView.GetRowCount() > 0)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Remove All Missing Scripts", GUILayout.Height(25)))
            {
                if (EditorUtility.DisplayDialog("Confirm Removal",
                    $"Remove all {treeView.GetRowCount()} missing script references?",
                    "Yes", "Cancel"))
                {
                    RemoveAllMissingScripts();
                }
            }

            if (GUILayout.Button("Find Suggestions", GUILayout.Height(25)))
            {
                ShowSuggestions();
            }

            if (GUILayout.Button("Export Results", GUILayout.Height(25)))
            {
                ExportResults();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();
        }

        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        Rect rect = EditorGUILayout.GetControlRect(false, treeView.totalHeight);
        treeView.OnGUI(rect);
        EditorGUILayout.EndScrollView();
    }

    void ScanProject()
    {
        var results = scanner.ScanProject(showScenes, showPrefabs);
        treeView.SetData(FilterResults(results));
        Repaint();
    }

    void ScanSelected()
    {
        var results = scanner.ScanSelected();
        treeView.SetData(FilterResults(results));
        Repaint();
    }

    List<MissingScriptEntry> FilterResults(List<MissingScriptEntry> results)
    {
        var filtered = results.Where(entry =>
        {
            if (!string.IsNullOrEmpty(searchString) &&
                !entry.gameObjectPath.Contains(searchString) &&
                !entry.assetPath.Contains(searchString))
                return false;

            if (showSelectedOnly && !Selection.Contains(entry.gameObject))
                return false;

            return true;
        }).ToList();

        return filtered;
    }

    void RemoveAllMissingScripts()
    {
        cleaner.RemoveAll(treeView.GetAllEntries());
        ScanProject(); // Refresh
    }

    void ExportResults()
    {
        string path = EditorUtility.SaveFilePanel("Export Missing Scripts", "", "missing_scripts.json", "json");
        if (!string.IsNullOrEmpty(path))
        {
            System.IO.File.WriteAllText(path, JsonUtility.ToJson(treeView.GetAllEntries()));
            EditorUtility.RevealInFinder(path);
        }
    }

    void ShowSuggestions()
    {
        var entries = treeView.GetAllEntries();
        if (entries.Count == 0) return;

        var suggestionWindow = CreateInstance<MissingScriptSuggestionWindow>();
        suggestionWindow.SetEntries(entries);
        suggestionWindow.ShowUtility();
    }
}
