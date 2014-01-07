using UnityEngine;
using System.Collections;
using Phidgets;
using Phidgets.Events;
using System;

public class BallController : MonoBehaviour {

	public GameObject player, platform, hitsController, timeGone, timeLeft, cam, topBar, hitsBar, timeLine;
	public static float distanceTravelled, value = 0, startTime, adjustedTime;
	public bool dir = true, shiftCamera = false, gameStart = false, gameEnd = false;
	public InterfaceKit ifKit;
	public Queue forces = new Queue();
	public static int hits = 0;
	public Vector3 newPos;
	public static Vector3 startVertex, endVertex;
	public Rect windowRect;
	public static String editString = "300"; 
	public static ArrayList uQ = new ArrayList();
	public static ArrayList vQ = new ArrayList();
	// Use this for initialization
	void Start () {
		platform = GameObject.Find ("Cube");
		hitsController = GameObject.Find ("hits");
		timeGone = GameObject.Find ("timeCompleted");
		timeLeft = GameObject.Find ("timeRemaining");
		topBar = GameObject.Find ("TopCube");
		hitsBar = GameObject.Find ("HitBar");
		timeLine = GameObject.Find ("TimeLine");
		distanceTravelled = 0;
		platform.guiTexture.color = Color.green;
		setup_Phidget ();
		//setup_Screen ();
		Debug.Log ("Screen width and height " + Screen.width + " " + Screen.height); 

		windowRect = new Rect((Screen.width - 400)/2, (Screen.height - 200)/2, 400, 200);

	}

	void OnGUI() {
		if (gameEnd) {
					windowRect = GUI.Window (0, windowRect, gameEndWindow, "Game Over!");
				}
		if (!gameStart) {
						windowRect = GUI.Window (0, windowRect, DoMyWindow, "Settings");
				} 
	}	
	void DoMyWindow(int windowID) {
		if (!gameStart) {
						if (GUI.Button (new Rect ((windowRect.width - 100) * 1 / 3, (windowRect.height - 20) * 2 / 3, 100, 20), "Ok")) {
								timeLeft.GetComponent<TextMesh> ().text = "time remaining " + Convert.ToString (Math.Floor ((Double)Convert.ToInt32 (editString) / 60)) + ":" + Convert.ToString (Math.Floor ((Double)Convert.ToInt32 (editString) % 60));
								gameStart = true;
								startTime = Time.realtimeSinceStartup;
								Debug.Log(Math.Floor ((Double)Convert.ToInt32 (editString) % 60));
						}
						GUI.Button (new Rect ((windowRect.width - 100) * 2 / 3, (windowRect.height - 20) * 2 / 3, 100, 20), "Cancel");
						editString = GUI.TextField (new Rect ((windowRect.width - 100) * 2 / 3, (windowRect.height - 20) * 1 / 3, 100, 20), editString, 3);
						GUI.Label (new Rect ((windowRect.width - 100) * 1 / 3, (windowRect.height - 20) * 1 / 3, 100, 20), "Game Time(s) :");
						//print("Got a click");
				}
	}

	void gameEndWindow(int windowID) {
		if (gameEnd) {
			if (GUI.Button (new Rect ((windowRect.width - 100) * 1 / 2, (windowRect.height - 20) * 2 / 3, 100, 20), "Ok")) {
				Application.Quit();
			}
			//print("Got a click");
		}
	}

	void onDisable(){
		Debug.Log("Phidget Simple Playground (plug and unplug devices)");
		Debug.Log("Press Enter to end anytime");
		
		Debug.Log("Closing....");
		try
		{
			ifKit.Attach -= new AttachEventHandler(ifKit_Attach);
			ifKit.Detach -= new DetachEventHandler(ifKit_Detach);
			ifKit.Error -= new ErrorEventHandler(ifKit_Error);
			ifKit.SensorChange -= new SensorChangeEventHandler(ifKit_SensorChange);
			ifKit.close();
		}
		catch (PhidgetException ex)
		{
			Debug.Log("Error on function call: " + ex.Code + " - " + ex.Description + "!");
		}
		
		ifKit = null;
	}
	// Update is called once per frame
	void Update () {
				//Debug.Log ("Update called now!! " + Time.deltaTime);
				/*if (dir) {
			transform.Translate (Time.deltaTime, Time.deltaTime * 5, 0);
			distanceTravelled += Time.deltaTime;	
			dir = false;
		} else {
			transform.Translate (Time.deltaTime, -Time.deltaTime * 5, 0);
			distanceTravelled += Time.deltaTime;	
			dir = true;
		}*/
		if (gameStart && !gameEnd) {
						adjustedTime = Time.realtimeSinceStartup - startTime;
						if ((Convert.ToInt32 (editString) - adjustedTime) < 0) {
								gameEnd = true;
						}
						timeGone.GetComponent<TextMesh> ().text = "time completed " + Convert.ToString (Math.Floor (adjustedTime / 60)) + ":" + Convert.ToString (Math.Floor (adjustedTime) % 60);
						timeLeft.GetComponent<TextMesh> ().text = "time remaining " + Convert.ToString (Math.Floor ((Convert.ToInt32 (editString) - adjustedTime) / 60)) + ":" + Convert.ToString (Math.Floor (Convert.ToInt32 (editString) - adjustedTime) % 60);
						if (forces.Count > 0) {
								if (shiftCamera) {
										Camera.main.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										platform.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										timeLine.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										topBar.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										hitsBar.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										hitsController.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										timeGone.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										timeLeft.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
								}
								if (Convert.ToDouble (forces.Peek ()) > 5) {
										if (value < 5) {
												hits++;
												hitsController.GetComponent<TextMesh> ().text = "HITS " + Convert.ToString (hits);
												lineCoordinates ();
												//Debug.DrawLine(new Vector3(transform.position.x, 6.5f, transform.position.z), new Vector3(transform.position.x, 7.5f, transform.position.z), Color.red, 33, false);
										}
										transform.Translate (Time.deltaTime, (float)(5 - value), 0);
										value = (float)5;
										forces.Dequeue ();
								} else {
										transform.Translate (Time.deltaTime, (float)(Convert.ToDouble (forces.Peek ()) - value), 0);
										value = (float)Convert.ToDouble (forces.Dequeue ());
								}
								distanceTravelled += Time.deltaTime;
						} else {
								if (shiftCamera) {
										Camera.main.transform.Translate (Time.deltaTime, 0, 0);
										platform.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										timeLine.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										topBar.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										hitsBar.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										hitsController.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										timeGone.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
										timeLeft.transform.Translate (Time.deltaTime, (float)(double)0, (float)(double)0);
								}
								transform.Translate (Time.deltaTime, 0, 0);
								distanceTravelled += Time.deltaTime;
						}
						if (distanceTravelled > 14) {
								//platform.transform.Translate((float)(double)15,(float)(double)0,(float)(double)0);
								//platform.transform.parent = transform;
								//Camera.main.transform.parent = transform;
								//Camera.main.transform.Translate((float)(double)distanceTravelled,(float)(double)0,(float)(double)0);
								distanceTravelled = 0;
								shiftCamera = true;
						}
				} 
	}

	static void ifKit_Attach(object sender, AttachEventArgs e)
	{
		int serialNumber;
		String name;
		
		try{
			serialNumber = e.Device.SerialNumber;
			name = e.Device.Name;
			Debug.Log("Hello Device " + serialNumber.ToString() + "," + "Serial Number:" + name + "!");
		}
		catch (PhidgetException ex)
		{
			Debug.Log("Error on function call: " + ex.Code + " - " + ex.Description + "!");
		}
	}
	
	static void ifKit_Detach(object sender, DetachEventArgs e)
	{
		int serialNumber;
		String name;
		
		try
		{
			serialNumber = e.Device.SerialNumber;
			name = e.Device.Name;
			Debug.Log("Goodbye Device " + serialNumber.ToString() + "," + "Serial Number:" + name + "!");
		}
		catch (PhidgetException ex)
		{
			Debug.Log("Error on function call: " + ex.Code + " - " + ex.Description + "!");
		}
	}
	
	static void ifKit_Error(object sender, ErrorEventArgs e)
	{
		int number;
		String name;
		
		number = (int)e.Code;
		name = e.Description;
		
		// The error triggered by the event
		Debug.Log("Error Event: " + number.ToString() + " - "  + name);
	}

	void ifKit_SensorChange(object sender, SensorChangeEventArgs e)
	{
		//Debug.Log ("value of sensor right now is " + e.Value);
		forces.Enqueue ((float)e.Value/(25.71*2));
	}

	public void setup_Phidget()
	{
		ifKit = new InterfaceKit();
		
		try
		{
			ifKit.Attach += new AttachEventHandler(ifKit_Attach);
			ifKit.Detach += new DetachEventHandler(ifKit_Detach);
			ifKit.Error += new ErrorEventHandler(ifKit_Error);
			ifKit.SensorChange += new SensorChangeEventHandler(ifKit_SensorChange);
		} 
		catch (PhidgetException ex)
		{
			Debug.Log("Error on function call: " + ex.Code + " - " + ex.Description + "!");
		}
		
		// No exception thrown on open
		ifKit.open();
	}

	public void setup_Screen()
	{
		newPos = Camera.main.ScreenToWorldPoint (new Vector2(Screen.width-150, Screen.height-70));
		hitsController.transform.Translate (newPos.x - hitsController.transform.position.x, newPos.y - hitsController.transform.position.y, 0);

		Debug.Log(newPos.x + " " + newPos.y + " " + newPos.z);
	}

	public void lineCoordinates()
	{
		startVertex = new Vector3(transform.position.x, 6.52f, transform.position.z);
		endVertex = new Vector3(transform.position.x, 7.0f, transform.position.z);
		uQ.Add (startVertex);
		vQ.Add (endVertex);
	}
}
