using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchSceneNode : SprigganNode
{
    public SceneAsset scene;
    public GameObject[] doNotDestroyList;

    public override async Awaitable Execute()
    {
        string sceneName = scene.name; 

        if (SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.Log("Scene of Name " + sceneName + " is not present in build index!");
        }

        await Awaitable.EndOfFrameAsync();
    }
}
