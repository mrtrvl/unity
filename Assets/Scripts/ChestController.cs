using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace bananaDiver.chestController
{
    public class ChestController : MonoBehaviour
    {
        // public GameObject item;
        public Sprite emptyChestImage;
        public Sprite fullChestImage;
        public Sprite circleImage;

        private Button chestButton;
        private static Dictionary<GameObject, int> items = new Dictionary<GameObject, int>();
        private CanvasRenderer imageCanvas;
        private GameObject itemsDisplay;
        //private GameObject itemOnDisplay;
        private bool showItems = false;

        void Start()
        {
            chestButton = GameObject.Find("ChestButton").GetComponent<Button>();
            chestButton.onClick.AddListener(ShowHideItemsList);
            itemsDisplay = GameObject.Find("Items");
            itemsDisplay.SetActive(false);
        }

        void Update()
        {
            updateChestImage ();
        }

        void ShowHideItemsList ()
        {
            showItems = !showItems;
            itemsDisplay.SetActive(showItems);
            if (showItems)
            {
                int index = 0;
                foreach (var item in items)
                {
                    if (GameObject.Find(item.Key.name) == null)
                    {
                        Debug.Log(item.Key.name);
                        GameObject itemOnDisplay = Instantiate(new GameObject(item.Key.name));
                        itemOnDisplay.transform.SetParent(itemsDisplay.transform);
                        itemOnDisplay.transform.position = new Vector2(itemsDisplay.transform.position.x - (index * 110), itemsDisplay.transform.position.y);
                        Image itemCircle = itemOnDisplay.AddComponent<Image>();
                        itemCircle.sprite = circleImage;
                    }
                    //Debug.Log(item.Key);
                    //Debug.Log(item.Value);
                    index ++;

                }
            }
        }

        void updateChestImage ()
        {
            if (items.Count > 0)
            {
                chestButton.image.sprite = fullChestImage;
            }
            else
            {
                chestButton.image.sprite = emptyChestImage;
            }
        }

        public static void AddToItems (GameObject newItem)
        {
            foreach (var item in items.Keys.ToList())
            {
                if (item.name == newItem.name)
                {
                    items[item] += 1;
                    return;
                }
            }
            items.Add(newItem, 1);
        }
    }
}