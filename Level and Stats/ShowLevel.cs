using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class ShowLevel : MonoBehaviour
    {
        BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        private void Update()
        {
            GetComponent<TMP_Text>().text = String.Format("{0:0}", baseStats.GetLevel());//Remember TMP_Text is different than Text
        }
    }
}
