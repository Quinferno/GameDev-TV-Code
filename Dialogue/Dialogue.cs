using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace RPG.Dialogue
{
    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue", order = 0)]
    public class Dialogue : ScriptableObject, ISerializationCallbackReceiver 
    {
        [SerializeField] List<DialogueNode> nodes = new List<DialogueNode>();
        [SerializeField] Vector2 newNodeOffset = new Vector2(250, 0);
        Dictionary<string, DialogueNode> nodeLookup = new Dictionary<string, DialogueNode>();

        private void Awake()//when editing or removing Awake for tutorial, don't remove "On Validate" 
        {
            #if UNITY_EDITOR//only runs this part in editor
         
            #endif//ends what hash began, meaning later code can till run in game

            OnValidate();//needed because in a finished, exported game it otherwise isn't called "naturally"
        }
        private void OnValidate() 
        {
            if(nodeLookup!=null) nodeLookup.Clear();//This bit isn't in tutorial but is a small bug fix due to version differences
            foreach (DialogueNode node in GetAllNodes())
            {
                if(node!=null)
                {
                    nodeLookup[node.name] = node;
                }
            }
        }
        public IEnumerable<DialogueNode> GetAllNodes() //a list is IEnumerable, so this pulls from all the dialogue nodes attached
        {
            return nodes;
        }

        public DialogueNode GetRootNode()
        {
            return nodes [0];
        }

        public IEnumerable<DialogueNode> GetAllChildren(DialogueNode parentNode) 
        {
            foreach (string childID in parentNode.GetChildren())
            {
                if (nodeLookup.ContainsKey(childID))
                {
                    yield return nodeLookup[childID];
                }
            }
        }

        public IEnumerable<DialogueNode> GetPlayerChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if(node.IsPlayerSpeaking())//only returns nodes where player is speaking
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<DialogueNode>  GetAIChildren(DialogueNode currentNode)
        {
            foreach (DialogueNode node in GetAllChildren(currentNode))
            {
                if(!node.IsPlayerSpeaking())//only returns nodes where player isn't speaking
                {
                    yield return node;
                }
            }
        }

#if UNITY_EDITOR//only runs this part in editor
        public void CreateNode (DialogueNode parent)
        {
            DialogueNode newNode = MakeNode(parent);
            Undo.RegisterCreatedObjectUndo(newNode, "Created Dialogue Node");
            Undo.RecordObject(this, "Added Dialogue Node");
            AddNode(newNode);
        }

        public void DeleteNode(DialogueNode nodeToDelete)
        {
            Undo.RecordObject(this, "Deleted Dialogue Node");
            nodes.Remove(nodeToDelete);            
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete); 
        }

        private DialogueNode MakeNode(DialogueNode parent)
        {
            DialogueNode newNode = CreateInstance<DialogueNode>();
            newNode.name = Guid.NewGuid().ToString();
            if (parent != null)
            {
                parent.AddChild(newNode.name);
                newNode.SetPlayerSpeaking(!parent.IsPlayerSpeaking());//by default, a node is the opposite speaker of the previous
                newNode.SetPosition(parent.GetRect().position + newNodeOffset);
            }

            if(newNode.IsPlayerSpeaking() == true)
            {
                newNode.SetSpeakerName("Jericho");
                newNode.SetSpeakerJob("The Merc With Bravado");
            }

            if(newNode.IsPlayerSpeaking() == false)
            {
                newNode.SetSpeakerName("Stranger");
                newNode.SetSpeakerJob("???");
            }

            return newNode;
        }
        private void AddNode(DialogueNode newNode)
        {
            nodes.Add(newNode);
            OnValidate();
        }

        private void CleanDanglingChildren(DialogueNode nodeToDelete)
        {
            foreach (DialogueNode node in GetAllNodes())
            {
                node.RemoveChild(nodeToDelete.name);
            }
        }
#endif//ends what hash began, meaning later code can till run in game
        public void OnBeforeSerialize()
        {
#if UNITY_EDITOR//only runs this part in editor
            if (nodes.Count == 0)
            {
                DialogueNode newNode = MakeNode(null);
                AddNode(newNode);
            }
            
            if (AssetDatabase.GetAssetPath(this) != "")
            {
                foreach (DialogueNode node in GetAllNodes())
                {
                    if (AssetDatabase.GetAssetPath(node) == "")
                    {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
#endif//ends what hash began, meaning later code can till run in game
        }

        public void OnAfterDeserialize()
        {
        }
    }
}



