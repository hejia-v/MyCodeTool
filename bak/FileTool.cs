using System.Collections.Generic;
using System.IO;

public partial class FileTool
{
    public static List<string> GetFiles(string directory, string[] patterns)
    {
        List<string> files = new List<string>();
        foreach (var pattern in patterns)
        {
            foreach (var item in Directory.GetFiles(directory, pattern))
            {
                files.Add(item);
            }
        }

        foreach (var item in Directory.GetDirectories(directory))
        {
            files.AddRange(GetFiles(item, patterns));
        }
        return files;
    }
}