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
        public GameObject itemPrefab;

        private Button chestButton;
        private static Dictionary<GameObject, int> items = new Dictionary<GameObject, int>();
        private GameObject itemsDisplay;
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
                    createItemForDisplay (item.Key, item.Value, index);
                    index ++;
                }
            }
            else
            {
                foreach (Transform child in itemsDisplay.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
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
            Debug.Log(name);
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