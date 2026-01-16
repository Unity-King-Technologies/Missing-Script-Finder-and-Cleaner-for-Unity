using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public static class AssetUtility
{
    public static string GetAssetPath(GameObject go)
    {
        if (PrefabUtility.IsPartOfPrefabAsset(go))
        {
            return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
        }
        else if (!string.IsNullOrEmpty(go.scene.path))
        {
            return go.scene.path;
        }
        return "";
    }

    public static bool IsInScene(GameObject go)
    {
        return !string.IsNullOrEmpty(go.scene.path) && !PrefabUtility.IsPartOfPrefabAsset(go);
    }

    public static bool IsInPrefab(GameObject go)
    {
        return PrefabUtility.IsPartOfPrefabAsset(go);
    }

    public static List<MonoScript> FindSimilarScripts(string missingScriptName)
    {
        var allScripts = AssetDatabase.FindAssets("t:MonoScript")
            .Select(guid => AssetDatabase.LoadAssetAtPath<MonoScript>(AssetDatabase.GUIDToAssetPath(guid)))
            .Where(script => script != null);

        var similarScripts = allScripts.Where(script =>
        {
            var scriptName = script.name.ToLower();
            var missingName = missingScriptName.ToLower();
            return scriptName.Contains(missingName) || missingName.Contains(scriptName) ||
                   LevenshteinDistance(scriptName, missingName) <= 3;
        }).ToList();

        return similarScripts;
    }

    private static int LevenshteinDistance(string s, string t)
    {
        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        if (n == 0) return m;
        if (m == 0) return n;

        for (int i = 0; i <= n; i++)
            d[i, 0] = i;
        for (int j = 0; j <= m; j++)
            d[0, j] = j;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                d[i, j] = Mathf.Min(
                    Mathf.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[n, m];
    }
}
