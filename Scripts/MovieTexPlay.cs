using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovieTexPlay : MonoBehaviour 
{
	public MovieTexture movieTexture;
	void Start ()
	{
       movieTexture.loop = true;
       movieTexture.Play();
   	}
   // Update is called once per frame
   void Update ()
   {
	   
   }
   public void SetLoop(bool status=true)
   {
       movieTexture.loop = status;
   }
}
