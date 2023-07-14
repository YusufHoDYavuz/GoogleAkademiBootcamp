using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chestController : MonoBehaviour
{
    Animation animation;
    public  bool isOpen = false;
    [SerializeField] private BoxCollider boxCollider;
    void Start()
    {
        animation = GetComponent<Animation>();
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void openChest()
    {
        if (isOpen == false)
        {
            animation.Play("ChestAnim");
            isOpen = true;
            boxCollider.enabled = false;
        }

    }
}
