using System.ComponentModel.Design;
using System.Diagnostics.Tracing;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.ComponentModel;
namespace RPG.Dialogue.Editor
{
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;//Intellisense seems broken in this script for some reason and doesn't know the calls from these statements
using UnityEditor.Callbacks;
using UnityEngine;

    public class DialogueEditor : EditorWindow 
    {
        Dialogue selectedDialogue = null;
        [NonSerialized] GUIStyle nodeStyle;
        [NonSerialized] GUIStyle playerNodeStyle;
        [NonSerialized] DialogueNode draggingNode = null;
        [NonSerialized] Vector2 draggingOffset;
        [NonSerialized] DialogueNode creatingNode = null;
        [NonSerialized] DialogueNode deletingNode = null;
        [NonSerialized] DialogueNode linkingParentNode = null;
        Vector2 scrollPosition;
        [NonSerialized] bool draggingCanvas = false;
        [NonSerialized] Vector2 draggingCanvasOffset;

        const float canvasSize = 4000;
        const float backgroundSize = 50;


        [MenuItem("Window/Dialogue Editor")] public static void ShowEditorWindow() 
        {
            GetWindow(typeof(DialogueEditor), false, "Dialogue Editor");
        }

        [OnOpenAssetAttribute(1)] public static bool OnOpenAsset(int instanceID, int line)//needed to make double clicking on dialogue automatically open window
        {
            Dialogue dialogue = EditorUtility.InstanceIDToObject(instanceID) as Dialogue;// "as" returns null if type is wrong
            if(dialogue != null)
            {
                ShowEditorWindow();
                return true;
            }
            
            return false;
        }

        private void OnEnable() 
        {
            Selection.selectionChanged += OnSelectionChanged;

            nodeStyle = new GUIStyle();
            nodeStyle.normal.background = EditorGUIUtility.Load("node0") as Texture2D;//pulls an in-built texture from an object Unity has by default for node background
            nodeStyle.padding = new RectOffset(20,20,20,20);
            nodeStyle.border = new RectOffset(12,12,12,12);

            playerNodeStyle = new GUIStyle();
            playerNodeStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;//pulls an in-built texture from an object Unity has by default for node background
            playerNodeStyle.padding = new RectOffset(20,20,20,20);
            playerNodeStyle.border = new RectOffset(12,12,12,12);
        }

        private void OnSelectionChanged()
        {
            Dialogue newDialogue = Selection.activeObject as Dialogue;
            if(newDialogue != null)
            {
                selectedDialogue = newDialogue;
                Repaint();//forces an OnGUI call
            }
        }

        private void OnGUI() 
        {
            if(selectedDialogue == null)
            {
                EditorGUILayout.LabelField("No Dialogue Selected.");
            }
            else
            {
                ProcessEvents();
                scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
                
                
                Rect canvas = GUILayoutUtility.GetRect(canvasSize,canvasSize);

                Texture2D backgroundTex = Resources.Load("background") as Texture2D;//finds file of the name given; don't use the file's extension. This file is not default in Unity, it had to be added
                Rect texCoords = new Rect (0,0,canvasSize/backgroundSize,canvasSize/backgroundSize);//third number is number of times texture repeats horizontally, forth is the same but vertically
                GUI.DrawTextureWithTexCoords(canvas, backgroundTex, texCoords);

                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                {
                    DrawConnections(node);
                }
                foreach (DialogueNode node in selectedDialogue.GetAllNodes())
                
                {
                    DrawNode(node);                    
                }

                EditorGUILayout.EndScrollView();

                if(creatingNode != null)
                {
                    
                    selectedDialogue.CreateNode(creatingNode);
                    creatingNode = null;
                }
                if(deletingNode != null)
                {
                    
                    selectedDialogue.DeleteNode(deletingNode);
                    deletingNode = null;
                }
            }
        }

        private void ProcessEvents()
        {
            if (Event.current.type == EventType.MouseDown && draggingNode == null)
            {
                draggingNode = GetNodeAtPoint(Event.current.mousePosition + scrollPosition);
                if(draggingNode != null)
                {
                    draggingOffset = draggingNode.GetRect().position - Event.current.mousePosition;
                    Selection.activeObject = draggingNode;
                }
                else
                {
                    draggingCanvas = true;
                    draggingCanvasOffset = Event.current.mousePosition + scrollPosition;
                    Selection.activeObject = selectedDialogue;
                }
            }
            else if (Event.current.type == EventType.MouseDrag && draggingNode != null)
            {
                
                draggingNode.SetPosition(Event.current.mousePosition + draggingOffset);
                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseDrag && draggingCanvas)
            {
                scrollPosition = draggingCanvasOffset - Event.current.mousePosition;

                GUI.changed = true;
            }
            else if (Event.current.type == EventType.MouseUp && draggingNode != null)
            {
                draggingNode = null;
            }
            else if (Event.current.type == EventType.MouseUp && draggingCanvas)
            {
                draggingCanvas = false;
            }
        }
        private void DrawNode(DialogueNode node)//if dialogue option results in different text than stated, will need to add optional dialogue field for what the option says that is overridden
                                                //by the dialogue that option results in if the optional field is blank
        {
            GUIStyle style = nodeStyle;
            if(node.IsPlayerSpeaking())
            {
                style = playerNodeStyle;
            }
            GUILayout.BeginArea(node.GetRect(), style);
            node.SetSpeakerName(EditorGUILayout.TextField(node.GetSpeakerName()));//Added by me, sets speaker name
            node.SetSpeakerJob(EditorGUILayout.TextField(node.GetSpeakerJob()));//Added by me, sets speaker name
            node.SetText(EditorGUILayout.TextField(node.GetText()));
            
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Del"))
            {
                deletingNode = node;
            }

            DrawLinkButtons(node);

            if (GUILayout.Button("Add"))
            {
                creatingNode = node;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();

        }

        private void DrawLinkButtons(DialogueNode node)
        {
            if (linkingParentNode == null)
            {
                if (GUILayout.Button("Link"))
                {
                    linkingParentNode = node;
                }
            }
            else if (linkingParentNode == node)
            {
                if (GUILayout.Button("Cancel"))
                {
                    linkingParentNode = null;
                }
            }
            else if (linkingParentNode.GetChildren().Contains(node.name))
            {
                if (GUILayout.Button("Break"))
                {    
                    linkingParentNode.RemoveChild(node.name);
                    linkingParentNode = null;
                }
            }
            else 
            {
                if (GUILayout.Button("Child"))
                {
                    linkingParentNode.AddChild(node.name);
                    linkingParentNode = null;
                }
            }
        }

        private void DrawConnections(DialogueNode node)//responsible for lines connecting nodes
        {
            Vector3 startPosition = new Vector2(node.GetRect().xMax, node.GetRect().center.y);
            foreach(DialogueNode childNode in selectedDialogue.GetAllChildren(node))
            {
                Vector3 endPosition = new Vector2(childNode.GetRect().xMin, childNode.GetRect().center.y); 
                Vector3 controlPointOffset = endPosition - startPosition;
                controlPointOffset.y = 0;
                controlPointOffset.x *= 0.8f;

                Handles.DrawBezier(startPosition, endPosition, startPosition + controlPointOffset, endPosition - controlPointOffset, Color.white, null, 4f);
            }          
        }

        private DialogueNode GetNodeAtPoint(Vector2 point)
        {
            DialogueNode foundNode = null;
            foreach (DialogueNode node in selectedDialogue.GetAllNodes())
            {
                if(node.GetRect().Contains(point))
                {
                    foundNode = node;
                }
            }
            return foundNode;
        }
    }
}
