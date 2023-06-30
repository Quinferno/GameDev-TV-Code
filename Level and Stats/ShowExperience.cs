using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ShowExperience : MonoBehaviour
    {
        Experience experience;

        private void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            GetComponent<TMP_Text>().text = String.Format("{0:0}", experience.GetExperience());//Remember TMP_Text is different than Text
        }
    }
}
