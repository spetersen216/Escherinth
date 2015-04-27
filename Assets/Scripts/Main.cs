using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Main : MonoBehaviour {

	public MazeGame[] levels;
	public Transform canvas;
	public Transform eventSystem;

	private GameObject main;
	private GameObject pause;
	private GameObject levelSelect;
	private GameObject levelEnd;
	private GameObject credits;
	private GameObject[] menus;
	private Scrollbar scrollbar;

	void Start () {
		// initialize vars
		main = canvas.Find("Main").gameObject;
		pause = canvas.Find("Pause").gameObject;
		levelSelect = canvas.Find("LevelSelect").gameObject;
		levelEnd = canvas.Find("LevelEnd").gameObject;
		credits = canvas.Find("Credits").gameObject;
		menus = new GameObject[]{main, pause, levelSelect, levelEnd, credits};

		// initialize scrollbar
		scrollbar = levelSelect.transform.Find("Scrollbar").GetComponent<Scrollbar>();
		RectTransform tmp = ((RectTransform)scrollbar.transform);
		tmp.sizeDelta = new Vector2(20, Screen.height);

		// initialize menus
		MainMenu();
		for (int i=0; i<levels.Length; ++i) {
			RectTransform orig = (RectTransform)levelSelect.transform.Find("Template");
			RectTransform button = (RectTransform)Instantiate(orig);
			button.parent = levelSelect.transform;
			Text text = button.GetChild(0).GetComponent<Text>();
			text.text = "Level "+i+" ("+(levels[i].isScary?"scary":"plain")+") ("+(levels[i].is3D?"3D":"2D")+")";
			RectTransform rect = (RectTransform)button;
			button.anchoredPosition = new Vector2(orig.anchoredPosition.x, orig.anchoredPosition.y-40*i);
		}
		Destroy(levelSelect.transform.Find("Template").gameObject);
	}

	public void HideMenus() {
		for (int i=0; i<menus.Length; ++i)
			menus[i].SetActive(false);
	}

	public void MainMenu() {
		HideMenus();
		main.SetActive(true);
	}

	public void PauseMenu() {
		HideMenus();
		pause.SetActive(true);
	}

	public void LevelSelectMenu() {
		HideMenus();
		levelSelect.SetActive(true);
	}

	public void LevelEndMenu() {
		HideMenus();
		levelEnd.SetActive(true);
	}

	public void CreditsMenu() {
		HideMenus();
		credits.SetActive(true);
	}
}
