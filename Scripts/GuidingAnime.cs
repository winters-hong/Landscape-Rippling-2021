using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GuidingAnime : MonoBehaviour 
{
	public float time;
	public ChangeCamera camChanger;
	public GameObject GuidAniObj,preGuidAni;
	public GameObject[] Cloud;
	// Use this for initialization
	void Start () 
	{
		camChanger = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ChangeCamera>();
		time = 0.0f;
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		TimeCounting();
	}

	private void TimeCounting()
	{
		
		time = time>=10.0f ? 0.0f : time+Time.deltaTime;
		if(time >= 4.0f &&time <= 10.0f&& !camChanger.IsAnimRunning())
		{
			PlayAnime();
		}
		else
		GuidAniObj.SetActive(false);

	}
	
	public void PlayAnime(bool state=true)
	{
		if(state)
		{
			GuidAniObj.SetActive(true);
			StartCoroutine(DelayToInvoke.DelayToInvokeDo(() =>{ 
				GuidAniObj.SetActive(false);
				Debug.Log("xx");
			}, 3.1f));
		}
		else
		{
			GuidAniObj.SetActive(false);
		}
	}
}
