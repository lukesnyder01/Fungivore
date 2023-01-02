using System.IO;
using System.Linq;
using UnityEditor;

// prevents materials from being saved while we are in play mode
public class PlayModeMaterials : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        if (EditorApplication.isPlaying)
        {
            return paths.Where(path => Path.GetExtension(path) != ".mat").ToArray();
        }
        else
        {
            return paths;
        }
    }
}