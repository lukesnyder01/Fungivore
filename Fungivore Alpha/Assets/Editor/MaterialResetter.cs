using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



// ensure class initializer is called whenever scripts recompile
[InitializeOnLoadAttribute]
public static class MaterialColorResetter
{
    public static Material[] materials;


    // register an event handler when the class is initialized
    static MaterialColorResetter()
    {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    private static void LogPlayModeState(PlayModeStateChange state)
    {
        Debug.Log(state);
    }
}
