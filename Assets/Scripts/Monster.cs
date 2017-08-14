using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Monster : MonoBehaviour {

	public MunchyMonsters game;
	private int monsterType = 0;
	private int score = 0;
	public TextMesh scoreText;
	public SpriteRenderer monsterSprite;
	public List<Sprite> monsterSpritesIdle;
	public List<Sprite> monsterSpritesMunch;
	public AudioSource munchSound;
	public AudioSource blechSound;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int GetScore() {
		return score;
	}

	public int GetMonsterType() {
		return monsterType;
	}

	public void SetMonsterType(int toSet) {
		monsterType = toSet;
	}

	void OnTriggerEnter2D (Collider2D coll) {
		if (coll.gameObject.tag == "Food") {
			Munch ();
			if (coll.gameObject.GetComponent<Food>().GetFoodType() == monsterType) {
				munchSound.Play ();
				score++;
				scoreText.text = score.ToString ();
				Destroy (coll.gameObject);
			}
			else {
				//Debug.Log ("Game Over");
				blechSound.Play();
				CancelInvoke ("Demunch");
				Destroy (coll.gameObject);
				DestroyFood ();
				game.GameOver();
			}

		}
	}

	public void DestroyFood() {
		GameObject[] foods = GameObject.FindGameObjectsWithTag("Food");
		foreach (GameObject food in foods) {
			Destroy (food);
		}
	}

	public void ResetScore() {
		score = 0;
		scoreText.text = "0";
	}

	public void Munch() {
		monsterSprite.sprite = monsterSpritesMunch [monsterType];
		Invoke ("Demunch", 0.5f);
	}

	public void Demunch() {
		monsterSprite.sprite = monsterSpritesIdle [monsterType];
	}
}
