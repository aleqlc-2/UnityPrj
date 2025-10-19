using TMPro;
using UnityEngine;

public class LevelSelect : MonoBehaviour
{
    public GameObject buttonPrefab;

	public string[] levelNames;
	public Transform grid;

	private void Start()
	{
		for (int i = 0; i < levelNames.Length; i++)
		{
			GameObject newButton = Instantiate(buttonPrefab, grid);
			newButton.GetComponent<MenuElements>().sceneToLoad = levelNames[i];
			newButton.GetComponentInChildren<TextMeshProUGUI>().text = (i + 1).ToString();
		}
	}
}
