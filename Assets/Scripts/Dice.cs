﻿using System.Collections;
using UnityEngine;

public class Dice : MonoBehaviour {

     private static AudioSource dice_land, dice_shake;

    private Sprite[] diceSides;
    private SpriteRenderer rend;
    private int whosTurn = 1;
    public static bool coroutineAllowed = true;

	// Use this for initialization
	private void Start () {
        dice_land = GameObject.Find("DiceLand").GetComponent<AudioSource>();
        dice_shake = GameObject.Find("DiceShake").GetComponent<AudioSource>();
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        rend.sprite = diceSides[5];
	}

    private void OnMouseDown()
    {
        if (coroutineAllowed)
            StartCoroutine("RollTheDice");
    }

    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        dice_shake.Play();
        int randomDiceSide = 0;
        for (int i = 0; i <= 25; i++) {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }
        Debug.Log("Rolled: " + (randomDiceSide + 1));
        GameControl.diceSideThrown = randomDiceSide + 1;
        GameControl.diceSideThrown = 6; //DEBUG: force the dice roll value
        dice_land.Play();
        if (!GameControl.fast_travel) {
            yield return new WaitForSeconds(1f);
        }
        GameControl.MovePlayer();
        whosTurn *= -1;
        coroutineAllowed = true;
    }
}
