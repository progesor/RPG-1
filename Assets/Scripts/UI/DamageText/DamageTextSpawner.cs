using UnityEngine;

namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText damageTextPrefab = null;
        [SerializeField] Color damageTextColor=Color.red;
        
        public void Spawn(float damageAmount)
        {
            DamageText instance = Instantiate<DamageText>(damageTextPrefab, transform);
            instance.SetColor(damageTextColor);
            instance.SetValue(damageAmount);
        }
    }
}