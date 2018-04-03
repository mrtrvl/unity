using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace bananaDiver.chestController
{
    public class ChestController : MonoBehaviour
    {
        public GameObject item;

        private Button chestButton;
        private static Dictionary<GameObject, int> items = new Dictionary<GameObject, int>();

        void Start()
        {
            chestButton = GameObject.Find("ChestButton").GetComponent<Button>();
            chestButton.onClick.AddListener(ShowHideList);
        }

        void Update()
        {

        }

        void ShowHideList()
        {
            foreach (var item in items)
            {
                Debug.Log(item.Key);
                Debug.Log(item.Value);
            }
        }

        public static void AddToItems(GameObject newItem)
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