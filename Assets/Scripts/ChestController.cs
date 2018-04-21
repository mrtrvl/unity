using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace bananaDiver.chestController
{
    public class ChestController : MonoBehaviour
    {
        public Sprite emptyChestImage;
        public Sprite fullChestImage;
        public Sprite circleImage;
        public Sprite yellowChestImage;
        public GameObject itemPrefab;

        private Button chestButton;
        private static Dictionary<GameObject, int> items = new Dictionary<GameObject, int>();
        private GameObject itemsDisplay;
        private bool showItems = false;
        private static bool listIsChanged = false;
        private bool changeColor = false;
        private bool colorChanged = true;

        private Color yellow = new Color(1, 1, 0);
        private Color white = new Color(1, 1, 1);

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
            chestColor ();
            if (listIsChanged)
            {
                changeColor = true;
                refreshListOfItems ();
                listIsChanged = false;
            }
        }

        void chestColor ()
        {
            if (changeColor)
            {
                chestButton.image.color = yellow;
                changeColor = false;
            }

            if (chestButton.image.color != white)
            {
                chestButton.image.color = chestButton.image.color + new Color(0, 0, 0.01f);
            }



        }

        void refreshListOfItems ()
        {
            if (showItems && items.Count > 0)
            {
                removeAllItemsFromItemsDisplay();
                createItemsDisplay ();
            }
            else
            {
                removeAllItemsFromItemsDisplay();
            }
        }

        void createItemsDisplay ()
        {
            int index = 0;
            foreach (var item in items)
            {
                createItemForDisplay(item.Key, item.Value, index);
                index++;
            }
        }

        void removeAllItemsFromItemsDisplay ()
        {
            foreach (Transform child in itemsDisplay.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        void ShowHideItemsList ()
        {
            showItems = !showItems;
            itemsDisplay.SetActive(showItems);
            refreshListOfItems();
        }

        GameObject createItemForDisplay (GameObject key, int value,  int index)
        {
            GameObject GO = Instantiate(itemPrefab);

            GO.transform.parent = itemsDisplay.transform;
            GO.transform.position = new Vector2(itemsDisplay.transform.position.x - (index * 110), itemsDisplay.transform.position.y);

            Text itemText = GO.GetComponentInChildren<Text>();
            itemText.text = key.name + "\n" + value.ToString();

            Button itemButton = GO.GetComponent<Button>();
            itemButton.onClick.AddListener(() => clickOnItem(key.name));

            return GO;
        }

        void clickOnItem (string name)
        {
            if (name == "tnt")
            {
                Debug.Log("Plant tnt");
            }
            else if (name == "map")
            {
                Debug.Log("Show map");
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
            if (items.Count > 0)
            {
                foreach (var item in items.Keys.ToList())
                {
                    if (item.name == newItem.name)
                    {
                        items[item] += 1;
                        listIsChanged = true;
                        return;
                    }
                }
            }
            items.Add(newItem, 1);
            listIsChanged = true;
        }

        public static void RemoveItem (string itemToRemove)
        {
            foreach (var item in items.Keys.ToList())
            {
                if (item.name == itemToRemove)
                {
                    items[item] -= 1;
                    if (items[item] <= 0)
                    {
                        items.Remove(item);
                    }
                    listIsChanged = true;
                    return;
                }
            }
        }

        public static bool DoesHaveItem (string itemToCheck)
        {
            foreach (var item in items.Keys.ToList())
            {
                if (item.name == itemToCheck)
                {
                    return true;
                }
            }

            return false;
        }
    }
}