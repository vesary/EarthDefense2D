using UnityEngine;
using System.Collections;

public class AnimateTex : MonoBehaviour {

/*	public Sprite[] frames;
	public float fps = 12.0f;
	int index;
	SpriteRenderer rendereri;
*/
	// Use this for initialization
	public Animator globeAnim;


	void Start () {
	//	rendereri = gameObject.GetComponent<SpriteRenderer>();
	//	animation.Play();
		globeAnim = gameObject.GetComponent<Animator>();
		globeAnim.speed *= 0.08f;

	}
	
	// Update is called once per frame
	/*void Update () {
		index = (int)((Time.time * fps) % frames.Length);
		rendereri.sprite = frames[index];
	//	print ( index);
	}*/

	void Update () {
	//	globeAnim.animation.Play();

	}



}


