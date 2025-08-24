using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

using System.Reflection;

public class StoryTeller : MonoBehaviour
{
    private enum TimeMode
    {
        Seconds, Minutes
    }

    //https://docs.unity3d.com/6000.1/Documentation/ScriptReference/SerializeReference.html
    [SerializeReference] List<SprigganNode> nodes = new List<SprigganNode>();
    [SerializeField] List<Transition> transitions = new List<Transition>();


    Coroutine nodeSequence;

    Coroutine curCoroutine;
    int curNodeIdx;
    bool transitionDone = false;

    bool runningNodeSequence = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curNodeIdx = 0;

        RunNodeSequence();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public async Awaitable RunNodeSequence()
    {

        runningNodeSequence = true;

        while (curNodeIdx < nodes.Count)
        {
            SprigganNode node = nodes[curNodeIdx];

            node.SetActive();

            print("executing");
            await node.Execute();
            print("done executing");

            curNodeIdx++;
        }

        runningNodeSequence = false;

    }

    /*
    //NODE
    private async Awaitable ExecuteNode(SprigganNode node)
    {
        switch(node)
        {
            case PlayMusicNode musicNode:
                await ExecuteMusicNodeAsync(musicNode);
                break;

            case SwitchSceneNode switchSceneNode:
                ExecuteSwitchSceneNode(switchSceneNode);
                break;
            case SpawnObjectNode spawnObjectNode:
                ExecuteSpawnObjectNode(spawnObjectNode);
                break;
        }

    }

    private async Awaitable ExecuteMusicNodeAsync(PlayMusicNode musicNode)
    {
        AudioSource audioSource = musicNode.audioSource;
        audioSource.clip = musicNode.clip;

        audioSource.Play();
        curCoroutine = StartCoroutine(SetTimer(musicNode.duration, TimeMode.Seconds));

        print("start transition");
        Transition transition = new Transition();
        await transition.func();
        print("end transition");
    }

    private void ExecuteSwitchSceneNode(SwitchSceneNode switchSceneNode)
    {
        string sceneName = switchSceneNode.scene.name;

        if(SceneManager.GetSceneByName(sceneName) != null)
        {
            SceneManager.LoadScene(sceneName);
        } else
        {
            print("Scene of Name " + sceneName + " is not present in build index!");
        }

        transitionDone = true;
    }

    private void ExecuteSpawnObjectNode(SpawnObjectNode spawnObjectNode)
    {
        GameObject[] objectsToSpawn = spawnObjectNode.objectsToSpawn;

        foreach(GameObject gameObject in objectsToSpawn)
        {
            Instantiate(gameObject, transform.position, Quaternion.identity);
          
        }

        transitionDone = true;

    }

    

    private IEnumerator SetTimer(float time, TimeMode timeUnit)
    {
        transitionDone = false;

        if(timeUnit == TimeMode.Seconds)
        {
            yield return new WaitForSeconds(time);

        } else
        {
            yield return new WaitForSeconds(time * 60);
        }

        transitionDone = true;
    }

    */
    public void CreateNewNode(Type type)
    {
        nodes.Add((SprigganNode)Activator.CreateInstance(type));
    }

}
