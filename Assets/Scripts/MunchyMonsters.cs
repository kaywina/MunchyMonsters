using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class MunchyMonsters : MonoBehaviour {

	private Ray ray;
	private RaycastHit hit;

	public SpriteRenderer monsterSprite;
	public List<Sprite> monsterSprites;
	public Transform spawnPoint;
	public List<GameObject> foods;
	public Monster monster;
	private bool gameOver = false;
	private bool onStart = true;
	public GameObject gameOverDialog;
	public GameObject startDialog;
	private float spawnRate = 2f;
	private float originalSpawnRate;
	private float waveRate = 3f;
	private float spawnIncrement = 0.1f;
	private string playerName;
	public InputField nameField;
	public List<TextMesh> scoreFields;
	public GameObject monsterButtons;
	public GameObject tapAnywhere;
	public GameObject gameOverInstructions;

	public dreamloLeaderBoard dl;
	private int countdown = 3;
	public TextMesh countdownText;


	// Use this for initialization
	void Start () {
		if (PlayerPrefs.GetString ("Name") == "") { playerName = "UncleScotty"; }
		else { playerName = PlayerPrefs.GetString ("Name"); }
		nameField.text = playerName;
		originalSpawnRate = spawnRate;
		startDialog.SetActive (true);
		monster.gameObject.SetActive (false);
		gameOverDialog.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {

		// START DIALOG
		if (onStart && !nameField.isFocused) {
			#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
			if (Input.GetKeyUp (KeyCode.Space)) {
				PlayGame();
			}
			#endif

			#if UNITY_ANDROID || UNITY_IOS
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
				ray = Camera.main.ScreenPointToRay(Input.GetTouch (0).position);
				if (Physics.Raycast(ray, out hit)) {
					if (hit.transform.gameObject.name == "TapAnywhere") {
						PlayGame ();
					}
				}
			}
			#endif

			if (Input.GetKeyUp (KeyCode.Escape)) {
				Application.Quit ();
			}

		}



		// GAME SCREEN
		if (!onStart && !gameOver) {
			#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
			if (Input.GetKeyUp (KeyCode.Alpha1)) {
				monsterSprite.sprite = monsterSprites [0];
				monster.SetMonsterType (0);
			}

			if (Input.GetKeyUp (KeyCode.Alpha2)) {
				monsterSprite.sprite = monsterSprites [1];
				monster.SetMonsterType (1);
			}

			if (Input.GetKeyUp (KeyCode.Alpha3)) {
				monsterSprite.sprite = monsterSprites [2];
				monster.SetMonsterType (2);
			}
			#endif

			#if UNITY_ANDROID || UNITY_IOS
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
				ray = Camera.main.ScreenPointToRay(Input.GetTouch (0).position);
				if (Physics.Raycast(ray, out hit)) {
					if (hit.transform.gameObject.name == "Monster1Button") {
						monsterSprite.sprite = monsterSprites [0];
						monster.SetMonsterType (0);
					}
					if (hit.transform.gameObject.name == "Monster2Button") {
						monsterSprite.sprite = monsterSprites [1];
						monster.SetMonsterType (1);
					}
					if (hit.transform.gameObject.name == "Monster3Button") {
						monsterSprite.sprite = monsterSprites [2];
						monster.SetMonsterType (2);
					}
				}
			}
			#endif

			if (Input.GetKeyUp (KeyCode.Escape)) {
				ReturnToStart();
			}
		}

		// GAME OVER DIALOG
		if (gameOver) {
			#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE
			if (Input.GetKeyUp (KeyCode.Space)) {
				PlayAgain();
			}
			#endif

			#if UNITY_ANDROID || UNITY_IOS
			if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
				ray = Camera.main.ScreenPointToRay(Input.GetTouch (0).position);
				if (Physics.Raycast(ray, out hit)) {
					if (hit.transform.gameObject.name == "TapAnywhere") {
						PlayAgain ();
					}
				}
			}
			#endif

			if (Input.GetKeyUp (KeyCode.Escape)) {
				ReturnToStart();
			}
		}
	}

	public void ReturnToStart() {
		CancelInvoke ("SpawnFood");
		CancelInvoke ("ChangeSpawnRate");
		monster.DestroyFood ();
		onStart = true;
		startDialog.SetActive (true);
		monster.gameObject.SetActive (false);
		gameOverDialog.SetActive (false);
		monster.ResetScore ();
		nameField.gameObject.SetActive (true);
		gameOverInstructions.SetActive (false);
		tapAnywhere.SetActive (true);
	}

	public void PlayGame() {
		tapAnywhere.SetActive (false);
		nameField.gameObject.SetActive (false);
		gameOver = false;
		onStart = false;
		startDialog.SetActive (false);
		monster.gameObject.SetActive (true);
		InvokeRepeating ("SpawnFood", spawnRate, spawnRate);
		InvokeRepeating ("ChangeSpawnRate", 0, waveRate);
		monsterButtons.SetActive (true);
	}

	public void SpawnFood() {
		//Debug.Log ("Spawn food");
		int random = Random.Range (0, 3);
		Instantiate (foods [random], spawnPoint.transform.position, foods[0].transform.rotation);
	}

	public void ChangeSpawnRate() {
		if (spawnRate > 0.1f) {
			CancelInvoke ("SpawnFood");
			spawnRate -= spawnIncrement;
			InvokeRepeating ("SpawnFood", spawnRate, spawnRate);
		}
	}

	public void GameOver() {
		monsterButtons.SetActive (false);
		gameOver = true;
		CancelInvoke ("SpawnFood");
		CancelInvoke ("ChangeSpawnRate");
		gameOverDialog.SetActive (true);
		int score = monster.GetScore ();
		dl.AddScore (playerName, score);
		dl.LoadScores ();
		InvokeRepeating ("Countdown", 0f, 1f);
		Invoke ("SetScores", 4f);
		Invoke ("EnableTapAnywhere", 4f);
	}

	public void EnableTapAnywhere() {
		tapAnywhere.SetActive (true);
		gameOverInstructions.SetActive (true);
	}

	public void Countdown() {
		countdownText.text = "Scores ready in ... " + countdown.ToString ();
		countdown = countdown - 1;
		if (countdown == -1) {
			CancelInvoke ("Countdown");
			countdown = 3;
			countdownText.text = "High Scores";
		}
	}

	public void SetScores() {
		if (dl.GetGotScores ()) {
			//string[] rows = this.highScores.Split(new char[] {'\n'}, System.StringSplitOptions.RemoveEmptyEntries);
			string[] scores = dl.ToStringArray ();
			//Debug.Log (scores [0]);
			for (int n=0; n < 10; n++) {
				if (scores [n] != null) {
					string[] oneScore = scores [n].Split (new char[] {'|'});
					scoreFields [n].text = oneScore [1] + " " + oneScore [0];
				}
			}
		}
	}

	public void PlayAgain() {
		tapAnywhere.SetActive (false);
		gameOverInstructions.SetActive (false);
		spawnRate = originalSpawnRate;
		gameOverDialog.SetActive (false);
		monster.ResetScore ();
		monsterSprite.sprite = monsterSprites [monster.GetMonsterType ()];
		PlayGame ();
	}

	public void SetPlayerName() {
		playerName = nameField.text;
		Debug.Log (playerName.ToString());
		PlayerPrefs.SetString ("Name", playerName);
	}
}
