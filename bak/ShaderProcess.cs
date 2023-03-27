using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace RPG
{
    namespace Editor
    {
    }
}

public class ShaderProcess : Editor
{
    [MenuItem("Res/Shader把本该是Color的Vector属性修复成Color类型")]
    private static void ShaderFixPropertyVectorToColor()
    {
        ShaderFixPropertyVectorToColor("Assets/test1/character_base.shader");
        return;
        var shaders = AssetDatabase.FindAssets("t: Shader");
        foreach (var guid in shaders)
        {
            var path=AssetDatabase.GUIDToAssetPath(guid);
            ShaderFixPropertyVectorToColor(path);
        }
        Debug.Log("done");
    }

    private static void ShaderFixPropertyVectorToColor(string path)
    {
        var shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
        List<string> vectorList = new List<string>();
        for (int i = 0; i < shader.GetPropertyCount(); i++)
        {
            var type = shader.GetPropertyType(i);
            if (type != ShaderPropertyType.Vector) continue;
            var desc = shader.GetPropertyDescription(i);
            // Debug.Log(type);
            var found = false;
            var arr = desc.Split(" ");
            foreach (var s in arr)
            {
                if (s.Trim().ToLower().EndsWith("color"))
                {
                    found = true;
                    break;
                }
            }

            if (!found) continue;
            // Debug.Log(desc);
            vectorList.Add(desc);
        }

        var fullFilePath = GetFullPath(path);
        var lines = File.ReadAllLines(fullFilePath);
        for (var i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            var found = false;
            foreach (var desc in vectorList)
            {
                if (line.Contains($"\"{desc}\""))
                {
                    found = true;
                    break;
                }
            }
            if (!found) continue;
            line = line.Replace(", Vector)", ", Color)");
            lines[i] = line;
            // Debug.Log(line);
        }
        File.WriteAllLines(fullFilePath, lines);
    }

    private static string GetFullPath(string assetPath)
    {
        var fp = Application.dataPath + "" + assetPath.Substring("Assets".Length);
        return fp;
    }
}