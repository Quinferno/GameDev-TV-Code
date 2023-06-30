using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Dialogue;
using TMPro;
using UnityEngine.UI;

namespace RPG.UI
{
    public class DialogueUI : MonoBehaviour
    {
        PlayerConversant playerConversant;
        [SerializeField] TextMeshProUGUI NameText;
        [SerializeField] TextMeshProUGUI JobText;
        [SerializeField] TextMeshProUGUI AIText;
        [SerializeField] Button nextButton;
        [SerializeField] GameObject AIResponse;
        [SerializeField] Transform choiceRoot;
        [SerializeField] GameObject choicePrefab;
        [SerializeField] Button quitButton;

        void Start()
        {
            playerConversant = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConversant>();  
            playerConversant.onConversationUpdated += UpdateUI;    
            nextButton.onClick.AddListener(() => 
            {
                playerConversant.Next();//technically don't need curly braces for one line lambda functions
            });
            quitButton.onClick.AddListener(() => 
            {
                playerConversant.Quit();//I find this easier to read
            });

            UpdateUI();
        }
        void UpdateUI()
        {
            gameObject.SetActive(playerConversant.IsActive());//if playerConversant isn't active, the conversation game object isn't either

            if(!playerConversant.IsActive())
            {
                return;
            }

            AIResponse.SetActive(!playerConversant.IsChoosing());//if IsChoosing returns false, this is active
            choiceRoot.gameObject.SetActive(playerConversant.IsChoosing());//if IsChoosing returns true, this is active

            if (playerConversant.IsChoosing())
            {
                NameText.text = "Jericho";//Added by me, probably better solution
                JobText.text = "The Merc With Bravado";//Added by me, probably better solution

                BuildChoiceList();
            }
            else
            {
                NameText.text = playerConversant.GetName();//Added by me
                JobText.text = playerConversant.GetJob();//Added by me
                AIText.text = playerConversant.GetText();
                nextButton.gameObject.SetActive(playerConversant.HasNext()); //Can add and remove "End Conversation" button in a similar fashion
            }
        }

        private void BuildChoiceList()
        {
            foreach (Transform item in choiceRoot)
            {
                Destroy(item.gameObject);//Destroys default buttons in UI, to more easily replace them
            }

            foreach (DialogueNode choice in playerConversant.GetChoices())
            {
                GameObject choiceInstance = Instantiate(choicePrefab, choiceRoot);
                var textComp = choiceInstance.GetComponentInChildren<TextMeshProUGUI>();
                textComp.text = choice.GetText();
                Button button = choiceInstance.GetComponentInChildren<Button>();
                button.onClick.AddListener(() => //on a click, the code in the curly braces is called
                {
                    playerConversant.SelectChoice(choice);
                });//I believe this is called a lambda function
            }
        }
    }
}