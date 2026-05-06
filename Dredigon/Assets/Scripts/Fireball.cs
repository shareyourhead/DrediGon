using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Fireball : MonoBehaviourPunCallbacks, IPunInstantiateMagicCallback
{
	public Transform sourcePlayer;
	
	public float castingDistance;
	public float speed = 27f;
	private Vector3 startingPos;
	public float castingHeight;
	public float travel = 1f;
	
	// Start is called before the first frame update
	void Start()
	{
		// PlayerMovement PlayerMovement = sourcePlayer.GetComponent<PlayerMovement>();
		// castingDistance = PlayerMovement.castingDistance;
		//startingPos = sourcePlayer.transform.position;
		//RaycastHit hit;
		//Ray ray = new Ray(new Vector3(Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y)*castingDistance,100f,Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y)*castingDistance)+startingPos, Vector3.down);
		//Physics.Raycast(ray, out hit, 200f, ~3);
		//castingHeight=hit.point.y-startingPos.y;
		//travel = 1f;
		
		//Debug.DrawRay(ray.origin, ray.direction*200f, Color.red, 10f);
	}

	// Update is called once per frame
	void Update()
	{
		transform.Translate(0f, 0f, speed * Time.deltaTime);
		travel += speed * Time.deltaTime;
		transform.position = new Vector3(transform.position.x, ((-Mathf.Pow(travel,2f)+(castingDistance*travel))/Mathf.Sqrt(Mathf.Pow(castingDistance,2f)+Mathf.Pow(castingHeight,2f)) + castingHeight*travel/castingDistance + 0.375f + startingPos.y), transform.position.z);
		
		if(Physics.CheckSphere(transform.position, 0.375f, 456))
		{
			//Spawn explosion
			Collider[] colliders = Physics.OverlapSphere(transform.position, (castingDistance*0.4f + 1f)/2f);
			foreach(Collider c in colliders)
			{
				if(c.tag == "Player")
				{
					PlayerMovement player = c.transform.GetComponent<PlayerMovement>();
					float damage = (1-Mathf.Min(1f,Vector3.Distance(transform.position,c.transform.position)/((castingDistance*0.4f + 1f)/2f)))*30f; //Took out sqrt at beginning
					player.health = Mathf.Max(player.healthMin,player.health-damage);
					sourcePlayer.transform.GetComponent<PlayerMovement>().Points(damage);
				}
				//Dev testing dummy
				if(c.tag == "Dummy")
				{
					Dummy dummy = c.transform.GetComponent<Dummy>();
					float damage = (1-Mathf.Min(1f,Vector3.Distance(transform.position,c.transform.position)/((castingDistance*0.4f + 1f)/2f)))*30f; //Took out sqrt at beginning
					dummy.health = Mathf.Max(0f,dummy.health-damage);
					sourcePlayer.transform.GetComponent<PlayerMovement>().Points(damage);
				}
			}
			//Don't assign damage to "Player" + playerId
			Debug.Log("Fireball exploded");
			Destroy(gameObject);
			return;
		}
		
		if(transform.position.y < 0.375f)
		{
			//Spawn smoke poof
			Debug.Log("Fireball destroyed water");
			Destroy(gameObject);
			return;
		}
	}

	public void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		object[] instantiationData = info.photonView.InstantiationData;
		castingDistance = (float)instantiationData[0];
		
		PlayerMovement PlayerMovement = sourcePlayer.GetComponent<PlayerMovement>();
		// castingDistance = PlayerMovement.castingDistance;
		startingPos = sourcePlayer.transform.position;
		RaycastHit hit;
		Ray ray = new Ray(new Vector3(Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y)*castingDistance,100f,Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y)*castingDistance)+startingPos, Vector3.down);
		Physics.Raycast(ray, out hit, 200f, ~3);
		castingHeight=hit.point.y-startingPos.y;
		travel = 1f;
		
		Debug.DrawRay(ray.origin, ray.direction*200f, Color.red, 10f);
		Debug.Log("Fireball instantiated...");
		Debug.Log("castingDistance: "+castingDistance);
		Debug.Log("startingPos "+startingPos);
		Debug.Log("ray "+ray);
	}
}
