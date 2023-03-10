using System;
using UnityEngine;
using TMPro;

namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        private BaseStats baseStats;

        private void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }


        private void Update()
        {
            GetComponent<TMP_Text>().text = String.Format("{0:0}", baseStats.GetLevel());
        }
    }
}