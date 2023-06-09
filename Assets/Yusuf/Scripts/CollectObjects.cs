using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CollectObjects : MonoBehaviour
{
    public float raycastDistance;

    [SerializeField] private List<Sprite> parchmentUIList = new List<Sprite>();
    private int parchmentValue;

    [SerializeField] private GameObject parchmentUI;
    private bool isActiveParchmentUI;

    [Header("Keypad")]
    private string currentPassword = "";
    private string correctPassword = "1328";
    [SerializeField] private Transform[] doors;

    private DragAndDropController dragAndDropController;

    private bool isActiveCar;

    [SerializeField] private GameObject mainPlayerObject;
    
    public void openDoor(int doorIndex)
    {
        doors[doorIndex].DOLocalMoveY(doors[doorIndex].transform.localPosition.y+8- doorIndex*7, 1f);
    }

    private void Start()
    {
        dragAndDropController = GetComponent<DragAndDropController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, raycastDistance))
            {
                if (hit.collider.CompareTag("Collactable"))
                {
                    Destroy(hit.collider.gameObject);
                    Debug.Log("Collactable Object: " + hit.collider.name);
                }
                else if (hit.collider.CompareTag("Show"))
                {
                    if (!isActiveParchmentUI)
                    {
                        Sprite matchingSprite = FindSpriteByRaycastObjectName(parchmentUIList, hit.collider.name);

                        parchmentUI.transform.DOLocalMove(Vector3.zero, 0.25f);
                        isActiveParchmentUI = true;

                        if (matchingSprite != null)
                        {
                            parchmentUI.GetComponent<Image>().sprite = parchmentUIList[parchmentValue];
                        }
                    }
                }
                else if (hit.collider.CompareTag("Gem"))
                {
                    if (hit.collider.name == "PastGem")
                    {
                        Singleton.Instance.CallGemAction(5,0);
                        Singleton.Instance.RaiseGemAmount(1);
                    }
                    else if (hit.collider.name == "PresentGem")
                    {
                        Singleton.Instance.CallGemAction(5,1);
                        Singleton.Instance.RaiseGemAmount(1);
                    }
                    else if (hit.collider.name == "FutureGem")
                    {
                        Singleton.Instance.CallGemAction(5,2);
                        Singleton.Instance.RaiseGemAmount(1);
                        openDoor(1);
                    }

                    if (Singleton.Instance.gems == 3)
                        Debug.Log("FINISH GAME");

                    Debug.Log("Gem count: " + Singleton.Instance.gems);
                    Destroy(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Purchaseable"))
                {
                    string itemName = hit.collider.name;
                    switch (itemName)
                    {
                        case "Turret":
                            Debug.Log("Turret purchased");
                            Singleton.Instance.purchasedItems[0] = true;
                            break;
                                
                        case "microchip":
                            Debug.Log("Microchip purchased");
                            Singleton.Instance.purchasedItems[1] = true;
                            break;
                        case "Cape":
                            Debug.Log("Cape purchased");
                            Singleton.Instance.purchasedItems[2] = true;
                            break;


                        default:
                            Debug.Log("No item purchased");
                            break;
                    }
                    
                }else if (hit.collider.CompareTag("keypad"))
                {
                    string keypadValue = hit.collider.name;
                    if (keypadValue == "ok")
                    {
                        if (currentPassword == correctPassword)
                        {
                            Debug.Log("Password is correct");
                            openDoor(0);
                        }
                        else
                        {
                            Debug.Log("Password is wrong");
                        }
                    }
                    else if (keypadValue == "clear")
                    {
                        currentPassword = "";
                    }
                    else
                    {
                        currentPassword += keypadValue;
                    }
                }else if (hit.collider.CompareTag("Car"))
                {
                    dragAndDropController.cmFreeLook.Follow = dragAndDropController.carPov;
                    dragAndDropController.cmFreeLook.LookAt = dragAndDropController.carPov;
                    isActiveCar = true;
                    dragAndDropController.carController.enabled = true;
                    dragAndDropController.player.SetActive(false);
                    dragAndDropController.player.transform.parent = dragAndDropController.carPov.transform;
                }
            }

            Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.cyan);
        }

        if (isActiveParchmentUI && Input.GetKeyDown(KeyCode.O))
        {
            parchmentUI.transform.DOLocalMove(new Vector3(0, 1100, 0), 0.25f);
            isActiveParchmentUI = false;
        }

        if (Input.GetKeyDown(KeyCode.G) && isActiveCar)
        {
            dragAndDropController.carController.enabled = false;
            dragAndDropController.cmFreeLook.Follow = dragAndDropController.currentPov;
            dragAndDropController.cmFreeLook.LookAt = dragAndDropController.currentPov;
            dragAndDropController.player.SetActive(true);
            isActiveCar = false;
            dragAndDropController.player.transform.parent = mainPlayerObject.transform;

        }
    }

    private Sprite FindSpriteByRaycastObjectName(List<Sprite> sprites, string objectName)
    {
        int raiseValue = 0;

        foreach (Sprite sprite in sprites)
        {
            if (sprite.name == objectName)
            {
                parchmentValue = raiseValue;
                return sprite;
            }

            raiseValue++;
        }

        return null;
    }
}