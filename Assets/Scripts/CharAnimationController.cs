using UnityEngine;
using UnityEngine.Events;

public class CharAnimationController : MonoBehaviour
{
    [SerializeField] private Transform lFoot = null;
    [SerializeField] private Transform rFoot = null;
    [SerializeField] private GameObject footEffect = null;

    [SerializeField] private UnityEvent onFootGround;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void FootL()
    {
        onFootGround.Invoke();
        if (lFoot != null)
        {
            Instantiate(footEffect, lFoot.position, transform.rotation);
            
        }
    }

    public void FootR()
    {
        onFootGround.Invoke();
        if (rFoot != null)
        {
            Instantiate(footEffect, rFoot.position, transform.rotation);
            
        }
    }
}
