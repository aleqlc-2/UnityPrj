using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;


public class DataHandler : MonoBehaviour
{
    private GameObject furniture; // Table

    [SerializeField] private ButtonManager buttonPrefab;
    [SerializeField] private GameObject buttonContainer; // Canvas하위계층의 Content오브젝트
    [SerializeField] private List<Item> items;
    [SerializeField] private string label;

    private int current_id = 0;

    private static DataHandler instance; // i, static
    public static DataHandler Instance // I, static
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<DataHandler>(); // instance = this; 랑 같음
            }
            return instance;
        }
    }

    private async void Start()
    {
        items = new List<Item>();
        //LoadItems();
        await Get(label);
        CreateButton();
    }

    // void LoadItems()
    // {
    //     var items_obj = Resources.LoadAll("Items", typeof(Item));
    //     foreach (var item in items_obj)
    //     {
    //         // item가 var자료형이므로 as Item으로 언박싱해서 넣어야함
    //         items.Add(item as Item);
    //     }
    // }

    void CreateButton()
    {
        foreach (Item i in items)
        {
            ButtonManager b = Instantiate(buttonPrefab, buttonContainer.transform);
            b.itemId = current_id;
            b.ButtonTexture = i.itemImage;
            current_id++;
        }
    }

    public void SetFurniture(int id)
    {
        furniture = items[id].itemPrefab;
    }

    public GameObject GetFurniture()
    {
        return furniture;
    }

    public async Task Get(string label)
    {
        var locations = await Addressables.LoadResourceLocationAsync(label).Task;
        foreach (var location in locations)
        {
            var obj = await Addressables.LoadAssetAsync<Item>(location).Task;
            items.Add(obj);
        }
    }
}
