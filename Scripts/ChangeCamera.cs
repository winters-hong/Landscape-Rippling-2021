using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCamera : MonoBehaviour
{
	public GameController gameController;
	public AnimationClip[] clips;
	public GuidingAnime guidAnime;
	private int currentNum;//当前是第几个camera
	private int length;
	private int speed = 1;
	float waitTime = 2;
	bool ani_running;

	// Start is called before the first frame update
	void Start()
	{
		gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
		guidAnime = GameObject.FindGameObjectWithTag("GameController").GetComponent<GuidingAnime>();
		currentNum = 3;
		length = clips.Length;
		ani_running = false;
	}

	// Update is called once per frame
	void Update()
	{
		waitTime -= Time.deltaTime;
		if (Input.GetKeyDown(KeyCode.V))
		{
			currentNum++;
			if (currentNum >= length)
				currentNum = 0;
			this.GetComponent<Animation>().clip = clips[currentNum];
			this.GetComponent<Animation>().Play();
			//Debug.Log("input V!");
		}
		// if (gameController.isWaveRight && waitTime <= 0)
		// {
		// 	waitTime = 2.0f;
		// 	currentNum++;
		// 	if (currentNum == length)
		// 		currentNum = 0;
		// 	this.GetComponent<Animation>().clip = clips[currentNum];
		// 	this.GetComponent<Animation>().Play();
		// }
		// //Debug.Log(Time.deltaTime);
	}

	public void VisitScene(int aim)
	{
		if(ani_running)
		{
			return;
		}
		else
		{
			ani_running = true;
			guidAnime.PlayAnime(false);
			switch (aim)
			{
				case 1: currentNum=0; break;
				case 2: currentNum=1; break;
				case 3: currentNum=2; break;
				default: currentNum=0; break;
			}
			this.GetComponent<Animation>().clip = clips[currentNum];
			this.GetComponent<Animation>().Play();
			StartCoroutine(DelayToInvoke.DelayToInvokeDo(() =>{   
				this.GetComponent<Animation>().clip = clips[currentNum+3];
				guidAnime.PlayAnime(false);
				this.GetComponent<Animation>().Play();
				//ani_running = false;
				StartCoroutine(DelayToInvoke.DelayToInvokeDo(() =>{ ani_running = false;}, 2f));
            	}, 6f));
		}

	}

	public bool IsAnimRunning()
	{
		return ani_running;
	}
}
