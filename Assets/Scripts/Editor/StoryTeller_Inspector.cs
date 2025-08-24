using System.CodeDom;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(StoryTeller))]
public class StoryTeller_Inspector : Editor
{

    private DropdownField field;
    private StoryTeller storyTeller;

    int previousCount;

    public override VisualElement CreateInspectorGUI()
    {

        storyTeller = target as StoryTeller;
        VisualElement rootElement = new VisualElement();

        Box box = new Box();
        Button createNewNode = new Button();
        createNewNode.text = "Create New Node";

        field = new DropdownField("Type Of Node", new List<string> { "SwitchSceneNode", "SpawnObjectNode", "PlayMusicNode", "MoveObjectNode", "RotateObjectNode", "ProgrammableNode"}, 0);
       

        createNewNode.RegisterCallback<ClickEvent>(OnClick);

        box.Add(new Label("Hello I am under the water, pleaes help me!"));
        box.Add(createNewNode);

        SerializedProperty list = serializedObject.FindProperty("nodes");

        PropertyField listField = new PropertyField(list);

        
        rootElement.Add(listField);
        

        rootElement.Add(box);
        rootElement.Add(new VisualElement());


        rootElement.Add(field);

        return rootElement;
    }

    private void OnClick(ClickEvent evt)
    {
        switch(field.value)
        {

            case "SwitchSceneNode":
                storyTeller.CreateNewNode(typeof(SwitchSceneNode));
                break;

            case "SpawnObjectNode":
                storyTeller.CreateNewNode(typeof(SpawnObjectNode));
                break;
            case "PlayMusicNode":
                storyTeller.CreateNewNode(typeof(PlayMusicNode));
                break;
            case "MoveObjectNode":
                storyTeller.CreateNewNode(typeof(MoveObjectNode));
                break;
            case "RotateObjectNode":
                storyTeller.CreateNewNode(typeof(RotateObjectNode));
                break;
            case "ProgrammableNode":
                storyTeller.CreateNewNode(typeof(ProgrammableNode));
                break;
        }
    }

}
