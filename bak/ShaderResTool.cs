using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ShaderResTool
{
    // 查找shader片段时，用ag搜索Keywords
    // 属性 vector转color
    public static void RemoveShadersInvaildPart(string rootPath)
    {
        var shaders = FileTool.GetFiles(rootPath, new[] { "*.shader" });
        foreach (var fileName in shaders)
        {
            RemoveShaderInvaildPart(fileName);
        }
    }

    public static void ExtractShadersDecompiledCode(string rootPath, string outdir)
    {
        var shaders = FileTool.GetFiles(rootPath, new[] { "*.shader" });
        foreach (var fileName in shaders)
        {
            ExtractShaderDecompiledCode(fileName, outdir);
        }
    }

    private static string GetShaderName(TextReader reader)
    {
        reader.Seek(0);
        reader.Jump('\"');

        var p1 = reader.Position;

        reader.Jump('\"');
        var p2 = reader.Position - 1;

        var shaderName = reader.PeekString(p1, p2);

        return shaderName;
    }

    private static string GetPassName(TextReader reader, int passStart, int passEnd)
    {
        reader.Seek(passStart);
        reader.Jump("Name \"");
        if (reader.Position > passEnd) throw new Exception();
        var p1 = reader.Position;
        reader.Jump('\n');
        var passName = reader.PeekString(p1, reader.Position);
        passName = passName.Replace("Name \"", "").Replace("\"", "").Trim();
        reader.Seek(passStart);
        return passName;
    }

    private static void ExtractShaderDecompiledClip(TextReader reader, string outdir, string shaderName, string passName, string ext, int passStart, int passEnd, string programHeader)
    {
        reader.Seek(passStart);
        var clipIndex = 0;

        reader.Jump(programHeader);
        reader.Jump('{');
        reader.Rewind();
        var programStart = reader.Position;
        reader.JumpBraceBracketEnd();
        var programEnd = reader.Position;
        reader.Seek(programStart);

        while (reader.Position <= programEnd)
        {
            reader.Jump("SubProgram \"d3d11 \" {");
            if (reader.Position > programEnd) break;
            if (!reader.CanRead()) break;

            reader.Jump('{');
            reader.Rewind();
            var clipStart = reader.Position;
            reader.JumpBraceBracketEnd();
            var clipEnd = reader.Position;

            var clipCode = reader.PeekString(clipStart, clipEnd);
            clipCode = clipCode.Trim();
            clipCode = clipCode.Substring(1, clipCode.Length - 2);
            clipCode = clipCode.Trim();

            var indent = "\t\t\t\t\t";

            clipCode = indent + clipCode;

            var clipLines = clipCode.Split('\n');

            var tmp1 = new List<string>();
            foreach (var line in clipLines)
            {
                if (line.StartsWith(indent))
                {
                    tmp1.Add(line.Substring(indent.Length));
                }
                else
                {
                    tmp1.Add(line);
                }
            }

            clipCode = string.Join("\n", tmp1).Trim();

            // Console.WriteLine(clipName);

            // Console.WriteLine(clipCode);
            // return;

            reader.Seek(clipEnd);

            var filename = $"{outdir}/{shaderName}/{passName}/{clipIndex}.{ext}";
            var filedir = Path.GetDirectoryName(filename);
            if (!Directory.Exists(filedir))
            {
                Directory.CreateDirectory(filedir);
            }

            File.WriteAllText(filename, clipCode);

            clipIndex++;
        }
    }

    private static string CollectKeywords(string fileName)
    {
        var startStr = "Keywords {";
        var lines = File.ReadAllLines(fileName).Select(x => x.Trim()).Where(x => x.StartsWith(startStr));
        List<string> keys = new List<string>();
        foreach (var line in lines)
        {
            var s = line.Substring(startStr.Length);
            s = s.Substring(0, s.Length - 1).Trim();
            s = s.Replace("\"", "");
            var arr = s.Split(" ").Select(x => x.Trim());
            foreach (var key in arr)
            {
                if (!keys.Contains(key))
                {
                    keys.Add(key);
                }
            }
            // Console.WriteLine(line);
            // Console.WriteLine(s);
        }

        var test = string.Join("\n", keys);
        // Console.WriteLine(test);
        return test;
    }

    private static void ExtractShaderDecompiledCode(string fileName, string outdir)
    {
        Console.WriteLine(fileName);
        var text = File.ReadAllText(fileName);
        var reader = new TextReader(text);

        string shaderName = GetShaderName(reader);
        Console.WriteLine(shaderName);

        reader.Seek(0);
        var lastPos = reader.Position;
        while (true)
        {
            reader.Jump("Pass {");
            if (!reader.CanRead()) break;

            reader.Jump('{');
            reader.Rewind();
            var passStart = reader.Position;// pass的中括号前
            reader.JumpBraceBracketEnd();
            var passEnd = reader.Position;
            reader.Seek(passStart);

            var passName = GetPassName(reader, passStart, passEnd);
            Console.WriteLine(passName);

            reader.Seek(passStart);

            ExtractShaderDecompiledClip(reader, outdir, shaderName, passName, "vs", passStart, passEnd, "Program \"vp\" {");
            ExtractShaderDecompiledClip(reader, outdir, shaderName, passName, "fs", passStart, passEnd, "Program \"fp\" {");

            reader.Seek(passEnd);
            continue;
            // TODO: Keywords 单独收集
        }

        var Keywords = CollectKeywords(fileName);
        var filename = $"{outdir}/{shaderName}/Keywords.txt";
        File.WriteAllText(filename, Keywords);
    }

    private static void RemoveShaderInvaildPart(string fileName)
    {
        Console.WriteLine(fileName);
        var text = File.ReadAllText(fileName);
        var reader = new TextReader(text);
        reader.Jump('{');
        reader.Jump('{');
        reader.Rewind();
        reader.JumpBraceBracketEnd(); // jump to (before)SubShader
                                      // Debug.Log(nt);

        // StringBuilder codeBuilder = new StringBuilder();
        // codeBuilder.Append(reader.PeekString());
        // codeBuilder.Append("\n");
        // codeBuilder.AppendLine("}");

        var pos1 = reader.Position;

        List<string> lst1 = new List<string>();

        while (true)
        {
            // reader.Jump("\tPass {");
            reader.Jump("Program \"vp\" {");
            if (!reader.CanRead())
            {
                break;
            }

            var pos2 = reader.Position;
            reader.Jump('{');
            reader.Rewind();
            reader.JumpBraceBracketEnd();

            lst1.Add(reader.PeekString(pos2, reader.Position));

            // reader.Jump("GpuProgramID ");
        }

        reader.Seek(pos1);

        while (true)
        {
            reader.Jump("Program \"fp\" {");
            if (!reader.CanRead())
            {
                break;
            }

            var pos2 = reader.Position;
            reader.Jump('{');
            reader.Rewind();
            reader.JumpBraceBracketEnd();

            lst1.Add(reader.PeekString(pos2, reader.Position));
        }

        reader.Seek(pos1);

        while (true)
        {
            reader.Jump("GpuProgramID ");
            if (!reader.CanRead())
            {
                break;
            }

            var pos2 = reader.Position;
            reader.Jump('\n');
            reader.Rewind();

            lst1.Add(reader.PeekString(pos2, reader.Position));
        }

        foreach (var it in lst1)
        {
            text = text.Replace(it, "");
        }

        text = text.Replace("Comp Disabled", "Comp Never\n//Comp Disabled");

        // Console.WriteLine(text);
        File.WriteAllText(fileName + "", text);
    }

    public static void BackupShaders(string rootPath)
    {
        var shaders = FileTool.GetFiles(rootPath, new[] { "*.shader" });
        foreach (var fileName in shaders)
        {
            Console.WriteLine(fileName);

            var newFilePath = fileName.Replace('\\', '/').Replace("/Assets/", "/Shader/");
            var newDir = Path.GetDirectoryName(newFilePath);
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }

            File.Copy(fileName, newFilePath);
        }
    }

    public static void RestoreShaders(string rootPath)
    {
        var shaders = FileTool.GetFiles(rootPath, new[] { "*.shader" });
        foreach (var fileName in shaders)
        {
            Console.WriteLine(fileName);

            var bakFilePath = fileName.Replace('\\', '/').Replace("/Assets/", "/Shader/");

            if (File.Exists(bakFilePath))
            {
                File.Delete(fileName);
                File.Copy(bakFilePath, fileName);
            }
        }
    }
}