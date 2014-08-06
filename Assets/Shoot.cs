using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shoot : MonoBehaviour 
{
	public GameObject TrajectoryPointPrefeb;
	public GameObject BallPrefb;
	public CircleCollider2D DefSysCollider;
	AudioSource AmbientAudio;
	
	private GameObject ball;

	MeshRenderer[] helpTexts;
	bool showHelp = false;

	GameObject SafeZoneIndicator;
	private bool isPressed, isBallThrown;
	private float power = 10;
	private int numOfTrajectoryPoints = 6;
	private List<GameObject> trajectoryPoints;
	private Vector3 rockPos;
	public bool breakRocks = false;

	EasyFontTextMesh RockText;
	EasyFontTextMesh RadiusText;
	EasyFontTextMesh MissileText;
	EasyFontTextMesh MusicText;
	EasyFontTextMesh AimText;

	Collisions rockScr;

	float maxRockSpeed = 15;

	TargetingSys targetingScr;
	//---------------------------------------	
	void Start ()
	{

		SafeZoneIndicator = GameObject.Find("Earth/DefSystem/SafeZone");
		AmbientAudio = GameObject.Find("Ambient").GetComponent<AudioSource>();
		targetingScr = GameObject.Find("Earth/DefSystem").GetComponent<TargetingSys>();
		DefSysCollider = GameObject.Find("Earth/DefSystem").GetComponent<CircleCollider2D>();
		RockText = GameObject.Find("HUD/RockText").GetComponent<EasyFontTextMesh>();
		trajectoryPoints = new List<GameObject>();
		isPressed = isBallThrown =false;

		RadiusText = GameObject.Find("HUD/RadiusText").GetComponent<EasyFontTextMesh>();
		MissileText = GameObject.Find("HUD/MissileText").GetComponent<EasyFontTextMesh>();
		MusicText = GameObject.Find("HUD/MusicText").GetComponent<EasyFontTextMesh>();
		AimText = GameObject.Find("HUD/AimText").GetComponent<EasyFontTextMesh>();
		RadiusText.Text = "Safezone radius: "+targetingScr.safeZoneRadius;
		MissileText.Text = "Missile speed: "+targetingScr.missileSpeed;
		AimText.Text = "Aim delay: "+targetingScr.aimDelay+" sec.";


		helpTexts = GameObject.Find("HUD/Help").transform.GetComponentsInChildren<MeshRenderer>(true);


		for(int i=0;i<numOfTrajectoryPoints;i++)
		{
			GameObject dot= (GameObject) Instantiate(TrajectoryPointPrefeb);
			dot.renderer.enabled = false;
			trajectoryPoints.Insert(i,dot);
		}
	}
	//---------------------------------------	
	void Update () 
	{
		if(isBallThrown){
			isPressed = false;
			isBallThrown = false;
			return;
		}
		if(Input.GetMouseButtonDown(0))
		{
			isPressed = true;
		//	if(!ball)
			if(!isBallThrown)
				createBall();
		}
		else if(Input.GetMouseButtonUp(0))
		{
			isPressed = false;
			if(!isBallThrown)
			{
				throwBall();
				isBallThrown = false;
				for (int i = 0 ; i < numOfTrajectoryPoints ; i++)
				{
					
					trajectoryPoints[i].renderer.enabled = false;
					//trajectoryPoints[i].transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude)*fTime,pVelocity.x)*Mathf.Rad2Deg);
					
				}
			}
		}
		if(isPressed)
		{
			Vector3 vel = GetForceFrom(rockPos,Camera.main.ScreenToWorldPoint(Input.mousePosition));
			float angle = Mathf.Atan2(vel.y,vel.x)* Mathf.Rad2Deg;
			transform.eulerAngles = new Vector3(0,0,angle);
		//	setTrajectoryPoints(transform.position, vel/ball.rigidbody2D.mass);
			setTrajectoryPoints(rockPos, vel);
		}

		if(Input.GetKeyDown(KeyCode.LeftArrow))
		{

			Vector3 tempScale = SafeZoneIndicator.transform.localScale;

			if(tempScale.x>0&& targetingScr.safeZoneRadius>0)
			{
			tempScale.x = tempScale.x - 0.5f;
			tempScale.y = tempScale.y - 0.5f;
			SafeZoneIndicator.transform.localScale = tempScale;

				float tempRadius = targetingScr.safeZoneRadius;
				tempRadius = tempRadius - 0.5f;
				targetingScr.safeZoneRadius = tempRadius;
				DefSysCollider.radius = tempRadius;
				RadiusText.Text = "Safezone radius: "+targetingScr.safeZoneRadius;

			}
		}

		if(Input.GetKeyDown(KeyCode.RightArrow))
		{
			Vector3 tempScale = SafeZoneIndicator.transform.localScale;

				tempScale.x = tempScale.x + 0.5f;
				tempScale.y = tempScale.y + 0.5f;
				SafeZoneIndicator.transform.localScale = tempScale;

				float tempRadius = targetingScr.safeZoneRadius;
				tempRadius = tempRadius + 0.5f;
				targetingScr.safeZoneRadius = tempRadius;
				DefSysCollider.radius = tempRadius;
				RadiusText.Text = "Safezone radius: "+targetingScr.safeZoneRadius;


		}

		if(Input.GetKeyDown(KeyCode.UpArrow))
		{
			
			float tempMisSpeed = targetingScr.missileSpeed;
			tempMisSpeed = tempMisSpeed + 5f;
			targetingScr.missileSpeed = tempMisSpeed;
			MissileText.Text = "Missile speed: "+targetingScr.missileSpeed;
			
			
		}

		if(Input.GetKeyDown(KeyCode.DownArrow))
		{
			
			float tempMisSpeed = targetingScr.missileSpeed;

			if(tempMisSpeed>=5f)
			tempMisSpeed = tempMisSpeed - 5f;
			targetingScr.missileSpeed = tempMisSpeed;
			MissileText.Text = "Missile speed: "+targetingScr.missileSpeed;
			
			
		}

		if(Input.GetKeyDown(KeyCode.PageUp))
		{
			
			float tempAimDelay = targetingScr.aimDelay;
			

			tempAimDelay = tempAimDelay + 0.05f;
			targetingScr.aimDelay = tempAimDelay;
			targetingScr.aimDelayAtStart = tempAimDelay;
			AimText.Text = "Aim delay: "+targetingScr.aimDelay.ToString("F2")+" sec.";
			
			
		}

		if(Input.GetKeyDown(KeyCode.PageDown))
		{
			
			float tempAimDelay = targetingScr.aimDelay;
			
			if(tempAimDelay>=0.05f)
				tempAimDelay = tempAimDelay - 0.05f;
			targetingScr.aimDelay = tempAimDelay;
			targetingScr.aimDelayAtStart = tempAimDelay;
			AimText.Text = "Aim delay: "+targetingScr.aimDelay.ToString("F2")+" sec.";
			
			
		}



		if(Input.GetKeyDown(KeyCode.Space)) //press space to shoot a missele towards all rocks in scene
		{
			GameObject[] allRocks;
			allRocks = GameObject.FindGameObjectsWithTag("Rocks");

			for(int i=0; i<allRocks.Length; i++)
			{
				targetingScr.Rocks.Add(allRocks[i]);

			}
			
		}

		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
			if(breakRocks==false){
				breakRocks=true;
				RockText.Text = "Break rocks: ON";
			}
			else if(breakRocks==true){
				breakRocks=false;
				RockText.Text = "Break rocks: OFF";
			}
		}
		if(Input.GetKeyDown(KeyCode.M)){
			if(AmbientAudio.enabled){
				AmbientAudio.enabled = false;
				MusicText.Text = "Music: OFF";
			}
			else if(!AmbientAudio.enabled){
				AmbientAudio.enabled = true;
				MusicText.Text = "Music: ON";
			}

		}
		if(Input.GetKeyDown(KeyCode.F1)){
			if(showHelp == false){
				foreach(MeshRenderer textRend in helpTexts){
					textRend.enabled = true;
				}
				showHelp = true;

			}
			else if (showHelp==true){
				foreach(MeshRenderer textRend in helpTexts){
					textRend.enabled = false;
					GameObject.Find("HUD/Help").GetComponent<MeshRenderer>().enabled = true;
				}
				showHelp = false;

			}
		}

	}
	//---------------------------------------	
	// When ball is thrown, it will create new ball
	//---------------------------------------	
	private void createBall()
	{
		ball = (GameObject) Instantiate(BallPrefb);
		//Vector3 pos = transform.position;
		rockPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		rockPos.z = 0.0f;
		//pos.z=1;
		ball.transform.position = rockPos;
		//print (pos);
		//ball.SetActive(false);
		ball.SetActive(true);

		if(Vector3.Distance(rockPos, Vector3.zero)<=targetingScr.safeZoneRadius){//to check if spawns in safezone, to not add to dangerous rocks twice
			rockScr = ball.gameObject.GetComponent<Collisions>();
			rockScr.spawnedInSafeZone = true;
		}
	}
	//---------------------------------------	
	private void throwBall()
	{
		if(ball!=null){
		Vector3 forcePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		forcePos.z = 0.0f;
	//	ball.SetActive(true);	
		//ball.rigidbody.useGravity = true;

		Vector3 tempForce = (GetForceFrom(forcePos,rockPos)); //for maxRockSpeed
		               
		   if(tempForce.magnitude>maxRockSpeed){
			tempForce.Normalize();
				tempForce =tempForce * maxRockSpeed;
		}

		ball.rigidbody2D.AddForce(tempForce,ForceMode2D.Impulse);
//		print (ball.rigidbody2D.velocity.magnitude);
		isBallThrown = true;
		targetingScr.CheckForDanger(ball); //pass rock to targeting system to see if it's coming to safe zone
		}
	}
	//---------------------------------------	
	private Vector2 GetForceFrom(Vector3 fromPos, Vector3 toPos)
	{
		return (new Vector2(toPos.x, toPos.y) - new Vector2(fromPos.x, fromPos.y))*power;//*ball.rigidbody.mass;
	}
	//---------------------------------------	
	// It displays projectile trajectory path
	//---------------------------------------	
	void setTrajectoryPoints(Vector3 pStartPosition , Vector3 pVelocity )
	{
		float velocity = Mathf.Sqrt((pVelocity.x * pVelocity.x) + (pVelocity.y * pVelocity.y));
		float angle = Mathf.Rad2Deg*(Mathf.Atan2(pVelocity.y , pVelocity.x));
		float fTime = 0;


		fTime += 0.1f;
		for (int i = 0 ; i < numOfTrajectoryPoints ; i++)
		{
		//	float dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
		//	float dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad) - (Physics2D.gravity.magnitude * fTime * fTime / 2.0f);

			float dx;
			float dy;

			if(velocity<=maxRockSpeed){
			 dx = velocity * fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			 dy = velocity * fTime * Mathf.Sin(angle * Mathf.Deg2Rad);
			}
			else{
			dx = maxRockSpeed* fTime * Mathf.Cos(angle * Mathf.Deg2Rad);
			dy = maxRockSpeed* fTime * Mathf.Sin(angle * Mathf.Deg2Rad);
			}

			Vector3 pos = new Vector3(pStartPosition.x - dx , pStartPosition.y - dy ,1);
			trajectoryPoints[i].transform.position = pos;
			trajectoryPoints[i].renderer.enabled = true;
			//trajectoryPoints[i].transform.eulerAngles = new Vector3(0,0,Mathf.Atan2(pVelocity.y - (Physics.gravity.magnitude)*fTime,pVelocity.x)*Mathf.Rad2Deg);

			fTime += 0.1f;
			
		}
	}
}