using System;
using UnityEngine;
using TMPro;

namespace RPG.Atributes
{
    public class HealthDisplay : MonoBehaviour
    {
        private Health _health;

        private void Awake()
        {
            _health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }


        private void Update()
        {
            //GetComponent<TMP_Text>().text = String.Format("{0:0}%", _health.GetPercentage());
            GetComponent<TMP_Text>().text = String.Format("{0:0}/{1:0}", _health.GetHealthPoints(), _health.GetMaxHealtPoint());
        }

        
    }
}