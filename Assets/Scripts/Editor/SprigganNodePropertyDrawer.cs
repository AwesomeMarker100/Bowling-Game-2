using Codice.Client.Common.TreeGrouper;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SprigganNode))]
public class SprigganNodePropertyDrawer : PropertyDrawer
{
    public override VisualElement CreatePropertyGUI(SerializedProperty property)
    {
        Foldout rootElement = new Foldout();
        var reference = property.managedReferenceValue;

        if(reference is PlayMusicNode)
        {

            PropertyField audioSource = new PropertyField(property.FindPropertyRelative("audioSource"));
            rootElement.text = "Play Music Node";

            rootElement.Add(audioSource);

            PropertyField audioClip = new PropertyField(property.FindPropertyRelative("clip"));
            rootElement.Add(audioClip);

            PropertyField duration = new PropertyField(property.FindPropertyRelative("duration"));
            rootElement.Add(duration);


        } else if(reference is SwitchSceneNode)
        {
            rootElement.text = "Switch Scene Node";

            PropertyField sceneAsset = new PropertyField(property.FindPropertyRelative("scene"));
            rootElement.Add(sceneAsset);

            PropertyField doNotDestroyList = new PropertyField(property.FindPropertyRelative("doNotDestroyList"));
            rootElement.Add(doNotDestroyList);


        } else if(reference is SpawnObjectNode)
        {
            rootElement.text = "Spawn Object Node";

            PropertyField objectsToSpawn = new PropertyField(property.FindPropertyRelative("objectsToSpawn"));
            rootElement.Add(objectsToSpawn);

        } else if (reference is MoveObjectNode)
        {
            rootElement.text = "Move Object Node";

            PropertyField gameObjectToMove = new PropertyField(property.FindPropertyRelative("gameObject"));
            PropertyField position = new PropertyField(property.FindPropertyRelative("position"));

            rootElement.Add(gameObjectToMove);
            rootElement.Add(position);
        } else if (reference is RotateObjectNode)
        {
            rootElement.text = "Rotate Object Node";

            PropertyField gameObjectToRotate = new PropertyField(property.FindPropertyRelative("gameObject"));
            PropertyField rotationEulerAngles = new PropertyField(property.FindPropertyRelative("rotationEulerAngles"));

            rootElement.Add(gameObjectToRotate);
            rootElement.Add(rotationEulerAngles);
        } else if (reference is ProgrammableNode)
        {
            rootElement.text = "Programmable Node";
            PropertyField actions = new PropertyField(property.FindPropertyRelative("actions"));
            rootElement.Add(actions);
        }

        return rootElement;
    }
}


