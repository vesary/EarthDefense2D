using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetingSys : MonoBehaviour {

	Transform target;
	GameObject Ring;
	GameObject LaunchPoint;
	public GameObject LaunchEffect;
	GameObject LaunchEffectPoint;
	public GameObject Missile;
	public AudioClip ShootClip;
	public float missileSpeed = 25.0f, aimDelay = 0.15f, aimDelayAtStart;



	Collisions rockScr;
	//[HideInInspector]
	public float safeZoneRadius = 3.0f;

	[HideInInspector]
	public List<GameObject> Rocks;

	public int rockCount;
	Vector3 aimPoint;
	Vector3 projection;





	// Use this for initialization
	void Start () {
		Rocks = new List<GameObject>();
		aimDelayAtStart = aimDelay;
		Ring = transform.FindChild("RingTurret").gameObject;
		LaunchPoint = Ring.transform.FindChild("LaunchPoint").gameObject;
		LaunchEffectPoint = LaunchPoint.transform.FindChild("PartEmitterPos").gameObject;


	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rockCount = Rocks.Count;
		SearchNewTarget();
		if(target != null){
			
			aimDelay -= Time.deltaTime;
			
			AimAtTarget();

			if(aimDelay<=0){
				Fire ();
				aimDelay = aimDelayAtStart;
			}
			
		}
	}

	public void CheckForDanger(GameObject rockToCheck){


		if(DoesTrajectoryEnterSafetyZone(rockToCheck.transform.position, rockToCheck.rigidbody2D.velocity)){

			rockScr = rockToCheck.GetComponent<Collisions>();
			rockScr.detected = true;
			Rocks.Add(rockToCheck);
		}

	}

	bool DoesTrajectoryEnterSafetyZone(Vector3 objectPosition,Vector3 objectVelocity){
		
		Vector3 point2origin  = objectPosition - transform.position;

		float dotProduct = Vector3.Dot(point2origin,objectVelocity.normalized);

		projection = point2origin - dotProduct * objectVelocity.normalized;
	

	//	print ("mag: "+projection.magnitude);
	//	print ("dot: "+Vector3.Dot(point2origin,objectVelocity.normalized));

		

		if(projection.magnitude<=safeZoneRadius&&dotProduct<0){ //dot has to be negative or else the rock is going away
			return true;
		}
		else{
			return false;
		}
	}

	Transform GetClosestRock()
	{
		GameObject closestRock = null;
		foreach (GameObject hit in Rocks) {

			if(!closestRock)
			{
				closestRock = hit;
			}
			//compares distances
			if(hit!=null)
			{
				if(Vector3.Distance(transform.position, hit.transform.position) <= Vector3.Distance(transform.position, closestRock.transform.position))
				{	
					closestRock = hit;
				}
			}

		}
		if(closestRock!=null)
			return closestRock.transform;
		else
			return null;
	}

	void SearchNewTarget()
	{
		if(rockCount>0)
		{
			
			target = GetClosestRock();

		}
		else
			target = null;
	}
	/*void AimAtTarget()
	{
		if(target != null){

			aimPoint = CalculateMissileVelocity(LaunchPoint.transform.position,LaunchPoint.rigidbody2D.velocity,target.position,target.rigidbody2D.velocity);
			
			LookAtTarget();
		}
	}*/
	void AimAtTarget() {
		
		if(target != null){

			aimPoint = CalculateMissileVelocity(LaunchPoint.transform.position,LaunchPoint.rigidbody2D.velocity,target.position,target.rigidbody2D.velocity);
			
			Vector3 dir = aimPoint-Ring.transform.position;
			Quaternion newQuaternion = Quaternion.LookRotation(dir,-Ring.transform.forward);
			
			newQuaternion.x = 0.0f;
			newQuaternion.y = 0.0f;
			
			Ring.transform.rotation = newQuaternion;
			
			Quaternion baseRotation = transform.rotation; 
			
			Ring.transform.rotation = baseRotation*newQuaternion;
			
		}
		
	}

	void Fire () {

		GameObject clone;
		
		clone = Instantiate(Missile, LaunchPoint.transform.position, LaunchPoint.transform.rotation) as GameObject;
	//	print (clone.transform.position);
	//	clone.transform.LookAt(aimPoint);
		//aimPoint = CalculateMissileVelocity(LaunchPoint.transform.position,LaunchPoint.rigidbody2D.velocity,target.position,target.rigidbody2D.velocity); //recalc aimpoint
		//Lookat2D
		Quaternion rotation = Quaternion.LookRotation
			(aimPoint - LaunchPoint.transform.position, LaunchPoint.transform.TransformDirection(-Vector3.forward));
		clone.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
		//end lookat

		Vector3 dir = clone.transform.up;
		dir.Normalize();
		clone.rigidbody2D.AddForce(LaunchPoint.transform.TransformDirection(Vector3.up) * missileSpeed, ForceMode2D.Impulse);

		Instantiate (LaunchEffect, LaunchEffectPoint.transform.position, LaunchEffectPoint.transform.rotation);
		audio.PlayOneShot(ShootClip);

		
		
		dir += clone.transform.position;		

		//Lookat2D
		Quaternion rotation2 = Quaternion.LookRotation
			(dir - LaunchPoint.transform.position, LaunchPoint.transform.TransformDirection(-Vector3.forward));
		clone.transform.rotation = new Quaternion(0, 0, rotation.z, rotation.w);
		//end lookat

		Rocks.Remove(target.gameObject); //remove target from array after being shot at
		
		
		
	}
	Vector3 CalculateMissileVelocity(Vector3 launchPosition,Vector3 launchVelocity,Vector3 rockPosition,Vector3 rockVelocity)
	{

		Vector3 targetRelativePosition  = rockPosition - launchPosition;
		Vector3 targetRelativeVelocity  = rockVelocity - launchVelocity;
		float t  = InterceptTime (missileSpeed,targetRelativePosition,targetRelativeVelocity);
		return rockPosition + t*(targetRelativeVelocity);
	}
	
	float InterceptTime(float misSpeed,Vector3 targetRelativePosition,Vector3 targetRelativeVelocity)	
	{


		float velocitySquared = targetRelativeVelocity.sqrMagnitude;
		if(velocitySquared < 0.001f)
			return 0f;
		
		float a = velocitySquared - misSpeed*misSpeed;
		
		//similar velocities
		if (Mathf.Abs(a) < 0.001f)
		{
			float t  = -targetRelativePosition.sqrMagnitude/
				(2f*Vector3.Dot(targetRelativeVelocity,targetRelativePosition));
			return Mathf.Max(t, 0f);
		}
		
		float b  = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
		float c  = targetRelativePosition.sqrMagnitude;

		float discriminant = b*b - 4f*a*c; 		// a quadratic equation to solve intercept time, usually has two real roots(two intercept paths)
												// a negative root is in the past (negative time) so it should be ignored
		
		if (discriminant > 0f) { // two roots
			float t1  = (-b + Mathf.Sqrt(discriminant))/(2f*a);
			float t2  = (-b - Mathf.Sqrt(discriminant))/(2f*a);
			if (t1 > 0f) {
				if (t2 > 0f)
					return Mathf.Min(t1, t2); //both are positive, find minimum time to intercept
				else
					return t1; //only t1 is positive
			} else
				return Mathf.Max(t2, 0f); //don't return negative root
		} else if (discriminant < 0f) //discriminant < 0; no roots, no intercept path
			return 0f;
		else //discriminant = 0; one solution, very rare
			return Mathf.Max(-b/(2f*a), 0f); //don't return negative root
		}

	void OnTriggerEnter2D(Collider2D other){

			if(other.gameObject.name=="Rock(Clone)"){
			rockScr = other.gameObject.GetComponent<Collisions>();
			if(!rockScr.detected&&!rockScr.spawnedInSafeZone)
			Rocks.Add(other.gameObject);
		}

	
	}

}
