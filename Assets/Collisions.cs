using UnityEngine;
using System.Collections;

public class Collisions : MonoBehaviour {

	float killDelay = 30.0f;
	bool startKill = false;
	public AudioClip ColClip;
	public bool detected = false;
	public bool spawnedInSafeZone = false;
	bool small = false;
	Collisions rockScr;
	Shoot shootScr;

	public GameObject CollidePart;

	// Use this for initialization
	void Start () {
		startKill = true;
		shootScr = GameObject.Find("Controls").GetComponent<Shoot>();
	}
	
	// Update is called once per frame
	void Update () {
		if(startKill){
			//killDelay -= Time.deltaTime; //replaced with onbecameinvisible
		}
		if(killDelay<0){
			startKill = false;
			Destroy(gameObject);
		}
	}

	void OnBecameInvisible()
	{
		DestroyObject(gameObject);
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.name=="Rock(Clone)" || other.gameObject.name=="Missile(Clone)"){
		//	killDelay = 3.0f;
			foreach (ContactPoint2D contact in other.contacts) {
				Instantiate (CollidePart, contact.point, Quaternion.identity);

				if(gameObject.audio&&!small){
					audio.PlayOneShot(ColClip);
				}
			}

			if(other.gameObject.name=="Missile(Clone)"&&gameObject.name=="Rock(Clone)"&&!small&&shootScr.breakRocks){ //break to smaller rocks

				Vector3 tempScale = gameObject.transform.localScale;
				tempScale.x = tempScale.x/2;
				tempScale.y = tempScale.y/2;
				Vector3 tempVel = gameObject.rigidbody2D.velocity;
				tempVel = tempVel/2;
				gameObject.rigidbody2D.velocity = tempVel;
				gameObject.transform.localScale = tempScale;
				small = true;

				GameObject clone1;
				
				clone1 = Instantiate(gameObject, transform.position, Quaternion.identity) as GameObject;

				clone1.gameObject.name = "Rock(Clone)";
				rockScr = clone1.gameObject.GetComponent<Collisions>();
				rockScr.small = true;
				rockScr.detected = false;

				GameObject clone2;
				
				clone2 = Instantiate(gameObject, transform.position, Quaternion.identity) as GameObject;

				clone2.gameObject.name = "Rock(Clone)";
				rockScr = clone2.gameObject.GetComponent<Collisions>();
				rockScr.small = true;
				rockScr.detected = false;


			}

		}
	}
}
