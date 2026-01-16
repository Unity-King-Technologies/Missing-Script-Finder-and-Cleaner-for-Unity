using UnityEngine;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using System.Collections.Generic;
using System.Linq;

public class MissingScriptTreeView : TreeView
{
    private List<MissingScriptEntry> data = new List<MissingScriptEntry>();

    public MissingScriptTreeView(TreeViewState state) : base(state)
    {
        Reload();
    }

    public void SetData(List<MissingScriptEntry> newData)
    {
        data = newData ?? new List<MissingScriptEntry>();
        Reload();
    }

    public List<MissingScriptEntry> GetAllEntries()
    {
        return data;
    }

    protected override TreeViewItem BuildRoot()
    {
        var root = new TreeViewItem { id = 0, depth = -1, displayName = "Root" };

        for (int i = 0; i < data.Count; i++)
        {
            var item = new MissingScriptTreeViewItem(data[i], i + 1);
            root.AddChild(item);
        }

        SetupDepthsFromParentsAndChildren(root);
        return root;
    }

    protected override void RowGUI(RowGUIArgs args)
    {
        var item = args.item as MissingScriptTreeViewItem;
        if (item == null) return;

        var entry = item.entry;

        // Icon
        Rect iconRect = args.rowRect;
        iconRect.width = 16;
        iconRect.x += GetContentIndent(item);

        Texture icon = entry.isInScene ? EditorGUIUtility.IconContent("d_SceneAsset Icon").image :
                       entry.isInPrefab ? EditorGUIUtility.IconContent("d_Prefab Icon").image :
                       EditorGUIUtility.IconContent("d_GameObject Icon").image;
        GUI.DrawTexture(iconRect, icon);

        // GameObject path
        Rect pathRect = args.rowRect;
        pathRect.x += iconRect.width + 4;
        pathRect.width -= iconRect.width + 4 + 100;

        GUI.Label(pathRect, entry.gameObjectPath);

        // Asset path
        Rect assetRect = args.rowRect;
        assetRect.x = assetRect.width - 200;
        assetRect.width = 180;
        GUI.Label(assetRect, entry.assetPath, EditorStyles.miniLabel);

        // Remove button
        Rect buttonRect = args.rowRect;
        buttonRect.x = buttonRect.width - 20;
        buttonRect.width = 18;

        if (GUI.Button(buttonRect, "×", EditorStyles.miniButton))
        {
            if (EditorUtility.DisplayDialog("Remove Missing Script",
                $"Remove missing script from {entry.gameObjectPath}?",
                "Yes", "Cancel"))
            {
                var cleaner = new MissingScriptCleaner();
                cleaner.RemoveSingle(entry);
                data.Remove(entry);
                Reload();
            }
        }
    }

    protected override float GetCustomRowHeight(int row, TreeViewItem item)
    {
        return 20;
    }
}

public class MissingScriptTreeViewItem : TreeViewItem
{
    public MissingScriptEntry entry;

    public MissingScriptTreeViewItem(MissingScriptEntry entry, int id)
    {
        this.entry = entry;
        this.id = id;
        this.displayName = entry.gameObjectPath;
    }
}
