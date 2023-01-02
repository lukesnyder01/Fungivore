using System.IO;
using System.Linq;
using UnityEditor;

// prevents materials from being saved while we are in play mode
public class PlayModeMaterials : UnityEditor.AssetModificationProcessor
{
    /*
    static string[] OnWillSaveAssets(string[] paths)
    {
        UnityEngine.Debug.Log("OnWillSaveAssetsCalled");

        if (EditorApplication.isPlaying)
        {
            UnityEngine.Debug.Log("We're not in Play Mode");
            return paths.Where(path => Path.GetExtension(path) != ".mat").ToArray();
            
        }
        else
        {
            UnityEngine.Debug.Log("We're not in Play Mode");
            return paths;
        }
    }
    */
}