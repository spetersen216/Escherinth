using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

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

	public int curLevel;
	public MazeGame curGame;
	private Action callback;
	private Action nextFrame;

	void Start () {
		// initialize vars
		main = canvas.Find("Main").gameObject;
		pause = canvas.Find("Pause").gameObject;
		levelSelect = canvas.Find("LevelSelect").gameObject;
		levelEnd = canvas.Find("LevelEnd").gameObject;
		credits = canvas.Find("Credits").gameObject;
		menus = new GameObject[]{main, pause, levelSelect, levelEnd, credits};

		// initialize scrollbar
		scrollbar = levelSelect.transform.Find("ScrollArea").Find("Scrollbar").GetComponent<Scrollbar>();
		RectTransform tmp = ((RectTransform)scrollbar.transform);
		tmp.sizeDelta = new Vector2(20, Screen.height);

		// initialize menus
		//Play();
		MainMenu();
		RectTransform template = (RectTransform)levelSelect.transform.Find("Template");
		RectTransform scrollArea = (RectTransform)levelSelect.transform.Find("ScrollArea");
		for (int i=0; i<levels.Length; ++i) {
			RectTransform button = (RectTransform)Instantiate(template);
			button.parent = scrollArea;
			Text text = button.GetChild(0).GetComponent<Text>();
			text.text = "Level "+i+" ("+(levels[i].isScary?"scary":"plain")+") ("+(levels[i].is3D?"3D":"2D")+")";
			RectTransform rect = (RectTransform)button;
			button.anchoredPosition = new Vector2(template.anchoredPosition.x, template.anchoredPosition.y-40*(i+1));
			Button.ButtonClickedEvent onClick = new Button.ButtonClickedEvent();
			int tmpi = i;
			onClick.AddListener(()=>PlayLevel(tmpi));
			button.GetComponent<Button>().onClick = onClick;
		}//*/
	}

	public void Update() {
		if (nextFrame!=null)
			nextFrame();
		nextFrame = null;
	}

	/// <summary>
	/// Plays the same level again.
	/// </summary>
	public void Restart() {
		PlayLevel(curLevel);
	}

	/// <summary>
	/// Plays the next level.
	/// </summary>
	public void Play() {
		PlayLevel(curLevel+1);
	}

	/// <summary>
	/// Plays the selected level.
	/// </summary>
	public void PlayLevel(int level) {
		Hide();
		print("level: "+level);

		// initialize the level
		curLevel = level;
		curGame = (MazeGame)Instantiate(levels[level]);
		curGame.gameObject.SetActive(true);
		nextFrame = ()=>curGame.Init(this);
	}

	/// <summary>
	/// Disables the camera, hides all menus.
	/// </summary>
	public void Hide() {
		HideMenus();
		camera.enabled = false;
	}

	/// <summary>
	/// Enables the camera, shows the given menu.
	/// </summary>
	public void Show(Action menu) {
		camera.enabled = true;
		menu();
	}

	/// <summary>
	/// Exits the game.
	/// </summary>
	public void Quit() {
		Application.Quit();
	}

	/// <summary>
	/// Hides all menus.
	/// </summary>
	public void HideMenus() {
		for (int i=0; i<menus.Length; ++i)
			menus[i].SetActive(false);
	}

	/// <summary>
	/// Shows main menu, resets the current level
	/// </summary>
	public void MainMenu() {
		HideMenus();
		main.SetActive(true);
		curLevel = -1;
		if (curGame!=null)
			Destroy(curGame);
	}

	/// <summary>
	/// Shows the pause menu. callback is called when unpaused.
	/// </summary>
	public void PauseMenu(Action callback) {
		HideMenus();
		pause.SetActive(true);
		this.callback = callback;
	}
	public void UnPause() { callback(); }

	/// <summary>
	/// Shows the level select menu, which is dynamically generated.
	/// </summary>
	public void LevelSelectMenu() {
		HideMenus();
		levelSelect.SetActive(true);
	}

	/// <summary>
	/// Shown when a level is completed.
	/// </summary>
	public void LevelEndMenu(bool victory) {
		// handle total victory
		if (victory && curLevel==levels.Length-1) {
			CreditsMenu();
			return;
		}

		HideMenus();
		levelEnd.SetActive(true);
		levelEnd.transform.Find("NextLevel").gameObject.SetActive(victory);
	}

	/// <summary>
	/// Shows the credits.
	/// </summary>
	public void CreditsMenu() {
		HideMenus();
		credits.SetActive(true);
	}

	public void GameOverMenu() {
		throw new NotImplementedException("GameOverMenu isn't currently implemented");
	}
}
