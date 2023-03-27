using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

public class ResEditor : Editor
{
    [MenuItem("Res/ShaderProcess 1")]
    private static void ShaderProcess1()
    {
        var shaderPath = "Assets/asbres/shader/crpshaders/character/character_base.shader";
        var filePath = Application.dataPath.Replace("Assets", "") + shaderPath;

        var text = File.ReadAllText(filePath);
        // Debug.Log(text);
        var reader = new TextReader(text);
        reader.Jump('{');
        reader.Jump('{');
        reader.Rewind();
        reader.JumpBraceBracketEnd();
        // Debug.Log(nt);

        StringBuilder codeBuilder = new StringBuilder();
        codeBuilder.Append(reader.PeekString());
        codeBuilder.Append("\n");
        codeBuilder.AppendLine("}");

        var newFilePath = Application.dataPath.Replace("Assets", "") + "character_base.shader";
        File.WriteAllText(newFilePath, codeBuilder.ToString());
    }

    [MenuItem("Res/ShaderProcess 2")]
    private static void ShaderProcess2()
    {
        var shaders = FileTool.GetFiles(Application.dataPath, new[] { "*.shader" });
        foreach (var shaderFilePath in shaders)
        {
            Debug.Log(shaderFilePath);
            var text = File.ReadAllText(shaderFilePath);
            // Debug.Log(text);
            var reader = new TextReader(text);
            reader.Jump('{');
            reader.Jump('{');
            reader.Rewind();
            reader.JumpBraceBracketEnd();
            // Debug.Log(nt);

            StringBuilder codeBuilder = new StringBuilder();
            codeBuilder.Append(reader.PeekString());
            codeBuilder.Append("\n");
            codeBuilder.AppendLine("}");

            File.WriteAllText(shaderFilePath, codeBuilder.ToString());
        }
    }

    [MenuItem("Tools/Remove Empty Folders")]
    public static void RemoveEmptFoldersy()
    {
        string[] dirs = Directory.GetDirectories(Application.dataPath, "*", SearchOption.AllDirectories);

        foreach (string dir in dirs)
        {
            if (Directory.GetFiles(dir).Length == 0 && Directory.GetDirectories(dir).Length == 0)
            {
                FileUtil.DeleteFileOrDirectory(dir);
                FileUtil.DeleteFileOrDirectory(dir + ".meta");
                Debug.Log("Removed empty folder: " + dir);
            }
        }

        AssetDatabase.Refresh();
    }

    [MenuItem("Res/Move Bundle Files")]
    private static void MoveBundleFiles()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(MoveBundleFilesCo());
    }

    private static IEnumerator MoveBundleFilesCo()
    {
        EditorUtility.ClearProgressBar();
        var pathList = AssetDatabase.GetAllAssetPaths().Where(x => x.StartsWith("Assets/")).Select(x => x.Replace('\\', '/')).ToList();

        var count = pathList.Count;
        for (var i = 0; i < pathList.Count; i++)
        {
            var oldPath = pathList[i];
            var p = ((float)i) / count;
            var cancel = EditorUtility.DisplayCancelableProgressBar("移动资源", $"资源移动中...{i + 1}/{count}", p);
            if (cancel)
            {
                EditorUtility.ClearProgressBar();
                yield break;
            }

            var arr = oldPath.Split("/");
            var dirty = false;
            var tmp = new List<string>();
            for (var index = 0; index < arr.Length; index++)
            {
                var s = arr[index];
                if (s == "AssetBundles")
                {
                    dirty = true;
                    continue;
                }
                else if (s.EndsWith(".unity") && index != arr.Length - 1)
                {
                    dirty = true;
                    continue;
                }
                tmp.Add(s);
            }

            if (!dirty) continue;

            var newPath = string.Join('/', tmp);

            var newFileName = $"{Application.dataPath}/{oldPath.Substring("Assets/".Length)}";
            var newDir = Path.GetDirectoryName(newFileName);
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            Debug.Log(newPath);
            yield return null;
            continue;

            var ret = AssetDatabase.MoveAsset(oldPath, newPath);
            if (!string.IsNullOrEmpty(ret))
            {
                Debug.Log(ret);
            }
            yield return null;
        }
    }

    [MenuItem("Res/RemoveUnusedPrefab")]
    private static void RemoveUnusedPrefab()
    {
        EditorCoroutineUtility.StartCoroutineOwnerless(CountEditorUpdates());
    }

    private static IEnumerator CountEditorUpdates()
    {
        EditorUtility.ClearProgressBar();
        var lst = AssetDatabase.FindAssets("t:Prefab");
        var count = lst.Length;
        for (var i = 0; i < lst.Length; i++)
        {
            var item = lst[i];
            var path = AssetDatabase.GUIDToAssetPath(item);
            var p = ((float)i) / count;
            var cancel = EditorUtility.DisplayCancelableProgressBar("处理资源", $"资源处理中...{i + 1}/{count}", p);
            if (cancel)
            {
                EditorUtility.ClearProgressBar();
                yield break;
            }
            if (!path.StartsWith("Assets/")) continue;
            if (path.StartsWith("Assets/asbres/ui")) continue;
            if (path.StartsWith("Assets/bak")) continue;
            var perfab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            bool keep = false;
            foreach (var child in perfab.GetComponentsInChildren<MeshFilter>())
            {
                if (child.sharedMesh != null)
                {
                    keep = true;
                    break;
                }
            }
            foreach (var child in perfab.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                if (child.sharedMesh != null)
                {
                    keep = true;
                    break;
                }
            }
            if (keep) continue;
            var newPath1 = path.Substring("Assets/".Length);
            var newPath = $"Assets/bak/{newPath1}";
            var newDir = $"{Application.dataPath}/bak/{newPath1}";
            newDir = Path.GetDirectoryName(newDir);
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            Debug.Log(newPath);
            var ret = AssetDatabase.MoveAsset(path, newPath);
            if (!string.IsNullOrEmpty(ret))
            {
                Debug.Log(ret);
            }
            yield return null;
            //Debug.Log(item);
            //return;
        }
    }
}