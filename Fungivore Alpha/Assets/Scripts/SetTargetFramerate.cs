using UnityEngine;

public class SetTargetFramerate : MonoBehaviour
{
    public int targetFrameRate = 240;

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
}