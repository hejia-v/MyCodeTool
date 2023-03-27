using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public partial class ShaderEditTool : Editor
{
    private static string GetHLSLProgramForwardCode(string shaderBaseName, bool isOutline)
    {
        var temp = @"
			HLSLPROGRAM

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			// #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT

			#pragma multi_compile_fog

			#pragma vertex VertexShaderWork
			#pragma fragment ShadeFinalColor

			#define ToonShaderIsOutline

			#include ""Assets/Shader/HLSL/CRP_Character/CharacterBaseVertex.hlsl""
			#include ""Assets/Shader/HLSL/CRP_Character/CharacterBaseFragment.hlsl""

			ENDHLSL
";

        var text = temp;
        text = text.Replace("CharacterBase", shaderBaseName);
        if (!isOutline) text = text.Replace("#define ToonShaderIsOutline", "");
        return text;
    }

    private static string GetHLSLProgramDepthCode(string shaderBaseName)
    {
        var temp = @"
			HLSLPROGRAM

			#pragma vertex VertexShaderWork
			#pragma fragment BaseColorAlphaClipTest

            #define ToonShaderIsOutline

			#include ""Assets/Shader/HLSL/CRP_Character/CharacterBaseVertex.hlsl""
			#include ""Assets/Shader/HLSL/CRP_Character/CharacterBaseFragment.hlsl""

			ENDHLSL
";

        var text = temp;
        text = text.Replace("CharacterBase", shaderBaseName);
        return text;
    }

    private static string GetHLSLProgramGBufferCode(string shaderBaseName)
    {
        var temp = @"
			HLSLPROGRAM

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
			#pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
			#pragma multi_compile_fragment _ _SHADOWS_SOFT

			#pragma multi_compile_fog

			#pragma vertex VertexShaderWork
			#pragma fragment GBufferPassFragment

			#include ""Assets/Shader/HLSL/CRP_Character/CharacterBaseVertex.hlsl""
			#include ""Assets/Shader/HLSL/CRP_Character/CharacterBaseFragment.hlsl""

			ENDHLSL
";

        var text = temp;
        text = text.Replace("CharacterBase", shaderBaseName);
        return text;
    }

    [MenuItem("Res/GenShaderHLSLProgram")]
    private static void GenShaderHLSLProgram()
    {
        var shader = GetActiveShader();
        if (shader == null) return;
        var shaderBaseName = shader.name.Trim().Split("/").Last();
        var fullPath = AssetFilePathToFullPath(AssetDatabase.GetAssetPath(shader));
        var text = File.ReadAllText(fullPath);
        string lastLightMode = null;
        while (true)
        {
            var reader = new TextReader(text);
            if (!string.IsNullOrEmpty(lastLightMode))
            {
                reader.Jump(lastLightMode);
                reader.Advance(lastLightMode.Length);
            }

            var lightmodePatt = "\"LightMode\" = \"";
            reader.Jump(lightmodePatt);
            if (!reader.CanRead())
            {
                break;
            }
            var lightmodeStart = reader.Position;
            reader.Jump('\n');
            var lightmode = reader.PeekString(lightmodeStart, reader.Position).Trim();
            lastLightMode = lightmode;
            lightmode = lightmode.Substring(lightmodePatt.Length);
            lightmode = lightmode.Substring(0, lightmode.Length - 1);
            Debug.Log(lightmode);
            // 添加代码
            reader.Seek(lightmodeStart);
            reader.JumpBack("{");
            reader.JumpBack("{");
            reader.JumpBraceBracketEnd();
            reader.Rewind();
            reader.Rewind();
            string code;
            if (lightmode.Contains("GBuffer"))
            {
                code = GetHLSLProgramGBufferCode(shaderBaseName);
                // Debug.Log(reader.Peak());
                // Debug.Log(text);
            }
            else if (lightmode.Contains("ShadowCaster"))
            {
                code = GetHLSLProgramDepthCode(shaderBaseName);
            }
            else if (lightmode.Contains("Outline"))
            {
                code = GetHLSLProgramForwardCode(shaderBaseName, true);
            }
            else
            {
                code = GetHLSLProgramForwardCode(shaderBaseName, false);
            }
            code = $"\n{code}\n";
            text.Insert(reader.Position, code);
        }
        // Debug.Log(text);
    }

    //============================================================
    // 可以移动 重复的shader到一个单独的文件夹，提升性能
    [MenuItem("Res/CreateHLSLFiles")]
    private static void CreateHLSLFiles()
    {
        var shader = GetActiveShader();
        if (shader == null) return;
        var inputBasePath = "Assets/Shader/HLSL/CRP_Character/CharacterBaseInput.hlsl";
        var vertexBasePath = "Assets/Shader/HLSL/CRP_Character/CharacterBaseVertex.hlsl";
        var fragmentBasePath = "Assets/Shader/HLSL/CRP_Character/CharacterBaseFragment.hlsl";

        var arr1 = shader.name.Trim().Split("/").Where(x => x != "miHoYo");
        var shaderBasePath = string.Join("/", arr1);
        Debug.Log(shaderBasePath);

        var inputPath = $"Assets/Shader/HLSL/{shaderBasePath}Input.hlsl";
        var vertexPath = $"Assets/Shader/HLSL/{shaderBasePath}Vertex.hlsl";
        var fragmentPath = $"Assets/Shader/HLSL/{shaderBasePath}Fragment.hlsl";

        if (!IsAssetFilePathExist(inputPath)) AssetDatabase.CopyAsset(inputBasePath, inputPath);
        if (!IsAssetFilePathExist(vertexPath)) AssetDatabase.CopyAsset(vertexBasePath, vertexPath);
        if (!IsAssetFilePathExist(fragmentPath)) AssetDatabase.CopyAsset(fragmentBasePath, fragmentPath);

        var shaderBaseName = shader.name.Trim().Split("/").Last();
        var inputFileName = shaderBaseName + "Input";
        ReplaceAssetFileText(vertexPath, "CharacterBaseInput", inputFileName);
        ReplaceAssetFileText(fragmentPath, "CharacterBaseInput", inputFileName);
    }

    [MenuItem("Res/GenVariablesDefines")]
    private static void GenVariablesDefines()
    {
        var shader = GetActiveShader();
        if (shader == null) return;
        var arr1 = shader.name.Trim().Split("/").Where(x => x != "miHoYo");
        var shaderBasePath = string.Join("/", arr1);
        var inputPath = AssetFilePathToFullPath($"Assets/Shader/HLSL/{shaderBasePath}Input.hlsl");
        var lines = File.ReadAllLines(inputPath);

        var lst = new List<string>();
        bool startInject = false;
        foreach (var line in lines)
        {
            if (line.Trim().StartsWith("// CODE_GEN_START"))
            {
                startInject = true;
            }
            else if (line.Trim().StartsWith("// CODE_GEN_End"))
            {
                startInject = false;
            }
            else if (!startInject)
            {
                lst.Add(line);
            }
        }

        var indent = "    ";
        var tex_lst = new List<string>();
        var cbuffer_lst = new List<string>();
        for (int i = 0; i < shader.GetPropertyCount(); i++)
        {
            var type = shader.GetPropertyType(i);
            var flag = shader.GetPropertyFlags(i);
            var name = shader.GetPropertyName(i);
            switch (type)
            {
                case ShaderPropertyType.Color:
                    if (flag.HasFlag(ShaderPropertyFlags.HDR))
                    {
                        cbuffer_lst.Add($"{indent}float4 {name};");
                    }
                    else
                    {
                        cbuffer_lst.Add($"{indent}half4 {name};");
                    }
                    break;

                case ShaderPropertyType.Vector:
                    cbuffer_lst.Add($"{indent}float4 {name};");
                    break;

                case ShaderPropertyType.Float:
                    if (IsToggle(shader, i) || IsEnum(shader, i))
                    {
                        cbuffer_lst.Add($"{indent}half {name};");
                    }
                    else
                    {
                        cbuffer_lst.Add($"{indent}float {name};");
                    }
                    break;

                case ShaderPropertyType.Range:
                    cbuffer_lst.Add($"{indent}float {name};");
                    break;

                case ShaderPropertyType.Texture:
                    tex_lst.Add($"sampler2D {name};");
                    cbuffer_lst.Add($"{indent}float4 {name}_ST;");
                    break;

                case ShaderPropertyType.Int:
                    cbuffer_lst.Add($"{indent}float {name};");
                    break;

                default:
                    break;
            }
        }

        lst.Add("// CODE_GEN_START");
        lst.Add("");
        lst.AddRange(tex_lst);
        lst.Add("");
        lst.Add("");
        lst.Add("CBUFFER_START(UnityPerMaterial)");
        lst.AddRange(cbuffer_lst);
        lst.Add("CBUFFER_END");
        lst.Add("");
        lst.Add("// CODE_GEN_End");

        File.WriteAllLines(inputPath, lst);
    }

    private static bool IsEnum(Shader shader, int ii)
    {
        var Attrs = shader.GetPropertyAttributes(ii);
        List<string> EnumNames = new List<string>();
        List<int> EnumValues = new List<int>();
        EnumNames.Clear();
        EnumValues.Clear();
        var IsEnum = false;
        string[] patts = new string[] { "Enum" };
        foreach (var attr in Attrs)
        {
            var s0 = attr.Trim();
            foreach (var patt in patts)
            {
                if (s0.StartsWith($"{patt}("))
                {
                    var s = s0;
                    s = s.Substring(patt.Length);
                    s = s.Substring(1, s.Length - 2).Trim();
                    var arr = s.Split(",");

                    IsEnum = true;
                    for (var i = 0; i < arr.Length; i++)
                    {
                        var es = arr[i];
                        if (i % 2 == 0)
                        {
                            EnumNames.Add(es.Trim());
                        }
                        else
                        {
                            EnumValues.Add(int.Parse(es.Trim()));
                        }
                    }
                }
            }
        }
        return IsEnum;
    }

    private static bool IsToggle(Shader shader, int i)
    {
        var Attrs = shader.GetPropertyAttributes(i);
        var IsBool = false;
        var IsKeywords = false;
        string[] patts = new string[] { "Toggle", "MHYToggle" };
        foreach (var attr in Attrs)
        {
            var s0 = attr.Trim();
            foreach (var patt in patts)
            {
                if (s0.StartsWith($"{patt}"))
                {
                    var s = attr.Trim();
                    s = s.Substring(patt.Length);
                    if (s.Length > 0)
                    {
                        if (s0.StartsWith($"{patt}("))
                        {
                            s = s.Substring(1, s.Length - 2).Trim();
                            if (s.Length > 0)
                            {
                                IsKeywords = true;
                                // Keywords = s;
                            }

                            IsBool = true;
                        }
                    }
                    else
                    {
                        IsBool = true;
                    }
                }
            }
        }

        return IsBool;
    }

    private static void ReplaceAssetFileText(string assetFilePath, string oldText, string newText)
    {
        var fullPath = AssetFilePathToFullPath(assetFilePath);
        var text = File.ReadAllText(fullPath);
        text = text.Replace(oldText, newText);
        File.WriteAllText(fullPath, text);
    }

    private static bool IsAssetFilePathExist(string assetFilePath)
    {
        var fullPath = AssetFilePathToFullPath(assetFilePath);
        if (File.Exists(fullPath))
        {
            return true;
        }

        return false;
    }

    private static string AssetFilePathToFullPath(string assetFilePath)
    {
        var fp = Application.dataPath.Substring(0, Application.dataPath.Length - "Assets".Length) + assetFilePath;
        return fp;
    }

    private static Shader GetActiveShader()
    {
        if (Selection.activeObject == null) return null;
        var shader = Selection.activeObject as Shader;
        if (shader == null)
        {
            var mat = Selection.activeObject as Material;
            if (mat != null)
            {
                shader = mat.shader;
            }
        }

        return shader;
    }
}