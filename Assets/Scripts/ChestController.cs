﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using bananaDiver.mapController;
using bananaDiver.cameraController;

namespace bananaDiver.chestController
{
    public class ChestController : MonoBehaviour
    {
        public Sprite TNTSprite;
        public Sprite MapSprite;
        public Sprite CoinSprite;
        public Sprite DiamondSprite;
        public Sprite KeySprite;

        public Sprite emptyChestImage;
        public Sprite fullChestImage;
        public Sprite circleImage;
        public GameObject itemPrefab;
        public static Dictionary<string, int> items = new Dictionary<string, int>();

        private Button chestButton;
        private GameObject itemsDisplay;
        private bool showItems = false;
        private static bool listIsChanged = false;
        private bool changeColor = false;
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
            UpdateChestImage ();
            ChestColor ();
            if (listIsChanged)
            {
                changeColor = true;
                RefreshListOfItems ();
                listIsChanged = false;
            }
        }

        void ChestColor()
        {
            if (changeColor)
            {
                chestButton.image.color = yellow;
                changeColor = false;
            }

            if (chestButton.image.color != white)
                chestButton.image.color = chestButton.image.color + new Color(0, 0, 0.01f);
        }

        void RefreshListOfItems()
        {
            if (showItems && items.Count > 0)
            {
                RemoveAllItemsFromItemsDisplay();
                CreateItemsDisplay ();
            }
            else
                RemoveAllItemsFromItemsDisplay();
        }

        void CreateItemsDisplay()
        {
            int index = 0;
            foreach (var item in items)
            {
                CreateItemForDisplay(item.Key, item.Value, index);
                index++;
            }
        }

        void RemoveAllItemsFromItemsDisplay ()
        {
            foreach (Transform child in itemsDisplay.transform)
                Destroy(child.gameObject);
        }

        void ShowHideItemsList ()
        {
            showItems = !showItems;
            itemsDisplay.SetActive(showItems);
            RefreshListOfItems();
        }

        GameObject CreateItemForDisplay(string key, int value,  int index)
        {
            GameObject GO = Instantiate(itemPrefab);
            GO.transform.SetParent(itemsDisplay.transform);
            GO.transform.position = new Vector2(itemsDisplay.transform.position.x - (index * 110), itemsDisplay.transform.position.y);

            Text itemText = GO.GetComponentInChildren<Text>();
            itemText.text = value.ToString();

            Image contentImage = GO.GetComponentInChildren<Image>();

            Sprite content;
            switch (key)
            {
                case "TNT":
                    content = TNTSprite;
                    break;
                case "Key":
                    content = KeySprite;
                    break;
                case "Diamond":
                    content = DiamondSprite;
                    break;
                case "Coin":
                    content = CoinSprite;
                    break;
                case "Map":
                    content = MapSprite;
                    break;
                default:
                    content = CoinSprite;
                    break;
            }
            contentImage.sprite = content;

            Button itemButton = GO.GetComponent<Button>();
            itemButton.onClick.AddListener(() => ClickOnItem(key));

            return GO;
        }

        void ClickOnItem(string name)
        {
            if (name == ItemTag.Tnt)
            {
                Debug.Log("Plant tnt");
            }
            else if (name == ItemTag.Map)
            {
                //MapController.showMap();
                CameraController.ShowMap();
                RemoveItem(ItemTag.Map);
            }
        }

        void UpdateChestImage()
        {
          if (items.Count > 0 && !showItems)
          {
              chestButton.image.sprite = fullChestImage;
          }
          else if (items.Count < 1 || showItems)
          {
              chestButton.image.sprite = emptyChestImage;
          }
        }

        public static void AddToItems(string newItem)
        {
            int itemCount;
            items.TryGetValue(newItem, out itemCount);
            if (itemCount < 1)
                items[newItem] = 1;
            else
                items[newItem] = items[newItem] + 1;
            listIsChanged = true;
        }

        public static void RemoveItem(string itemToRemove)
        {
            if (ItemCount(itemToRemove) >= 0)
                items[itemToRemove] = items[itemToRemove] - 1;

            if (ItemCount(itemToRemove) <= 0)
                items.Remove(itemToRemove);

            listIsChanged = true;
        }

        public static bool DoesHaveItem(string itemToCheck)
        {
            int itemCountInChest;
            items.TryGetValue(itemToCheck, out itemCountInChest);
            return itemCountInChest > 0;
        }

        public static int ItemCount(string itemToCount)
        {
            int itemCountInChest;
            items.TryGetValue(itemToCount, out itemCountInChest);
            if (itemCountInChest > 0)
                return items[itemToCount];
            else
                return 0;
        }
    }
}