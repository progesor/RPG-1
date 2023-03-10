using System;
using UnityEngine;
using TMPro;
using RPG.Atributes;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        private Fighter fighter;

        private void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }


        private void Update()
        {
            if (fighter.GetTarget() == null)
            {
                GetComponent<TMP_Text>().text = "N/A";
                return;
            }
            Health health = fighter.GetTarget();
            //GetComponent<TMP_Text>().text = String.Format("{0:0}%", health.GetPercentage());
            GetComponent<TMP_Text>().text = String.Format("{0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealtPoint());
        }
    }
}