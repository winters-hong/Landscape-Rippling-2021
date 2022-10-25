using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Video;
using UnityEngine.UI;

public class GameController : MonoBehaviour, InteractionListenerInterface
{
	[Tooltip("Camera used for screen-to-world calculations. This is usually the main camera.")]
	public Camera screenCamera;

	private bool isMoving;
	private bool colored;
	private MyGestureListener gestureListener;
	private Quaternion initialRotation;

//===============================================================	
	[Tooltip("Interaction manager instance, used to detect hand interactions. If left empty, it will be the first interaction manager found in the scene.")]
	public InteractionManager interactionManager;
	private InteractionManager.HandEventType lastHandEvent = InteractionManager.HandEventType.None;

	private Vector3 screenNormalPos = Vector3.zero;
	private Vector3 screenPixelPos = Vector3.zero;

	private int waved;
	private float[] cameraPos = {-10f, 0f, 10f};
	private int camX = 1;
	private ChangeCamera camChanger;

	public SensorReader sensorReader;
	public GameObject paint;
	public GameObject scanplane;
	public Material grayPaint;
	public Material colorChanging;
	public Material colorPaint;
	public GameObject anime;
	public VideoPlayer videoPlayer;
	public VideoPlayer scan_videoPlayer;
	public Canvas info;

	public Text pos;

	public GameObject preGuidAni;

	void Start() 
	{
		//paint2.SetActive(false);
		//By default set the main-camera to be screen-camera
		if (screenCamera == null) {
			screenCamera = Camera.main;
		}
		waved=0;
		//Initialize vars
		AnimeActive(false);
		interactionManager = InteractionManager.Instance;
		camChanger = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ChangeCamera>();
		sensorReader = GameObject.FindGameObjectWithTag("Arduino").GetComponent<SensorReader>();
		isMoving = false;
		colored = false;
		gestureListener = MyGestureListener.Instance;
		GrayMatSet();
		videoPlayer = paint.GetComponent<VideoPlayer>();
		scan_videoPlayer= scanplane.GetComponent<VideoPlayer>();
		info.GetComponent<CanvasGroup>().alpha=0;

		//preGuidAni=GameObject.FindGameObjectWithTag("preGuidAni");
	}
	
	void Update() 
	{
		
		if(Input.GetKeyDown(KeyCode.Space)||sensorReader.GetSensorState())
		{
			preGuidAni.SetActive(false);
			colored = true;
			Coloring();
			
			
		}
		else if(Input.GetKeyDown(KeyCode.P))
		{
			ColorPause();
		}

		if(!colored&&!preGuidAni.activeSelf)//倒水引导
		{
			preGuidAni.SetActive(true);
			StartCoroutine(DelayToInvoke.DelayToInvokeDo(() =>{ 
				preGuidAni.SetActive(false);
			}, 4.9f));
			//preGuidAni.SetActive(false);
		}
		if(colored&&!videoPlayer.enabled)//整体浏览
		{
			after_coloring();
		}
		if (colored && interactionManager != null && interactionManager.IsInteractionInited())
		{
			DetectGestures();
		}
		if(!isMoving)
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
				camChanger.VisitScene(1);
			else if(Input.GetKeyDown(KeyCode.Alpha2))
				camChanger.VisitScene(2);
			else if(Input.GetKeyDown(KeyCode.Alpha3))
				camChanger.VisitScene(3);
		}
	}

	//Detect user gestures to visit detail scenes
	public void DetectGestures()
	{
		screenNormalPos = interactionManager.IsLeftHandPrimary() ? interactionManager.GetLeftHandScreenPos() : interactionManager.GetRightHandScreenPos();
		screenPixelPos.x = (int)(screenNormalPos.x * (screenCamera ? screenCamera.pixelWidth : Screen.width));
		screenPixelPos.y = (int)(screenNormalPos.y * (screenCamera ? screenCamera.pixelHeight : Screen.height));

		//if(gestureListener && (gestureListener.IsSwipeLeft() || gestureListener.IsSwipeRight()))
		//if(lastHandEvent == InteractionManager.HandEventType.Grip)
		if(gestureListener.IsZoomingIn() || gestureListener.IsZoomingOut())
		{
			//Debug.Log(screenPixelPos.x);
			pos.text = screenPixelPos.x.ToString();
			isMoving = true;
			if(screenPixelPos.x<400)
			{
				if(waved == 1)
				{
					camChanger.VisitScene(1);
					waved = 0;
				}
				else waved=1;
			}
			else if(screenPixelPos.x>400 && screenPixelPos.x<650)
			{
				if(waved == 2)
				{
					camChanger.VisitScene(2);
					waved = 0;
				}
				else waved=2;
			}
			else if(screenPixelPos.x>650 && screenPixelPos.x<1100)
			{
				if(waved == 3)
				{
					camChanger.VisitScene(3);
					waved = 0;
				}
				else waved=3;				
			}
		}
		else
		{
			isMoving = false;
		}
	}

	//Set gray paint as material of board
	private void GrayMatSet()
	{
		paint.GetComponent<Renderer>().material= grayPaint;
	}

	//Color the board
	private void Coloring()
	{
		preGuidAni.SetActive(false);
		videoPlayer.Play();
		info.GetComponent<Animation>().Play();
				StartCoroutine(DelayToInvoke.DelayToInvokeDo(() =>{ 	//Switch material after 3s
			//AnimeActive(true);（浏览后再激活）
			paint.GetComponent<Renderer>().material = colorPaint;
			videoPlayer.enabled=false;
			colored = true;
		}, 3f));

		
	}
	private void after_coloring()
	{
		scan_videoPlayer.Play();
		scanplane.transform.localPosition = new Vector3(scanplane.transform.localPosition.x, scanplane.transform.localPosition.y, -2);
		StartCoroutine(DelayToInvoke.DelayToInvokeDo(() => {
			scanplane.SetActive(false);
			AnimeActive(true);
		}, 15f));
	}
	//Pause coloring video
	private void ColorPause()
	{
		videoPlayer.Pause();
	}
	
	//Activate animation objects
	public void AnimeActive(bool status)
	{
		anime.SetActive(status);
	}
	
	//Get state of isMoving
	public bool IsMoving()
	{
		return isMoving;
	}
	//===============================================================
	//      Hand Gectures Detecting Functions (Unused extended funcs)
	//===============================================================
	public void HandGripDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
	{
		if (!isHandInteracting || !interactionManager)
			return;
		if (userId != interactionManager.GetUserID())
			return;

		lastHandEvent = InteractionManager.HandEventType.Grip;
		//isLeftHandDrag = !isRightHand;
		screenNormalPos = handScreenPos;
	}

	public void HandReleaseDetected(long userId, int userIndex, bool isRightHand, bool isHandInteracting, Vector3 handScreenPos)
	{
		if (!isHandInteracting || !interactionManager)
			return;
		if (userId != interactionManager.GetUserID())
			return;

		lastHandEvent = InteractionManager.HandEventType.Release;
		//isLeftHandDrag = !isRightHand;
		screenNormalPos = handScreenPos;
	}

	public bool HandClickDetected(long userId, int userIndex, bool isRightHand, Vector3 handScreenPos)
	{
		return true;
	}
}