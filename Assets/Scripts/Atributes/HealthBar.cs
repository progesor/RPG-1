using UnityEngine;

namespace RPG.Atributes
{
    public class HealthBar : MonoBehaviour
    {

        [SerializeField] private Health health = null;
        [SerializeField] private RectTransform foreground = null;
        [SerializeField] private Canvas rootCanvas = null;
        private void Update()
        {
            if (Mathf.Approximately(health.GetFraction(), 0) || Mathf.Approximately(health.GetFraction(), 1))
            {
                rootCanvas.enabled = false;
                return;
            }

            rootCanvas.enabled = true;
            
            foreground.localScale = new Vector3(health.GetFraction(), 1, 1);
        }
    }
}