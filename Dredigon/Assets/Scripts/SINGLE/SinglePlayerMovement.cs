using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
//uncomment this in mp build
//using Photon.Pun

public class SinglePlayerMovement : MonoBehaviour
{
	//Special thanks to Chase Beadles for jumpstarting me early on, and google search for keeping the momentum going
	// -Travis "ShareYourHead" Newbry
	
	public CharacterController controller;
	public Transform Fireball;
	public Transform Snowball;
	public Transform ShadowField;
	public Transform Spike;
	public Transform Trap;
	public Transform Grapple;
	public Transform dummy;
	public Transform mySnowball;
	public Transform myShadowField;
	public List<Transform> myTraps;
	public GameObject cam;
	public Transform Capsule;
	public Transform Sphere;
	public GameObject topLight;
	public GameObject bottomLight;
	
	public float health = 100f;
	public float healthMin = 0f;
	public float speed = 4.5f;
	public float speedBoost = 0f;
	
	public Transform Gui;
	public Transform pauseMenu;
	public bool isPaused;
	
	public Transform UiHealth;
	public Transform UiHealthShield;
	public Transform UiUlt;
	public Transform UiUltColor;
	public Transform UiMeleeToast;
	public Transform UiMagicToast;
	public Transform UiTechToast;
	public Transform UiMelee1;
	public Transform UiMagic1;
	public Transform UiTech1;
	public Transform UiCooldown1;
	public Transform UiMelee2;
	public Transform UiMagic2;
	public Transform UiTech2;
	public Transform UiCooldown2;
	public Transform UiMelee3;
	public Transform UiMagic3;
	public Transform UiTech3;
	
	public Vector3 movementVector = Vector3.zero;
	
	private float jumpHeight = 1.0f;
	public float gravity = 9.81f;
	public float yVel = 0f;
	public Vector3 gravityCoef = Vector3.up;
	
	public bool isGrounded;
	private bool controllerIsGrounded;
	private bool hitAngleIsGrounded;
	private Vector3 hitNormal = Vector3.zero;
	private Vector3 hitHorizontal = Vector3.zero;
	private float hitHorizontalMagnitude = 0f;
	private float hitAngle = 0f;
	private float angleLimit = 40f;
	
	public float castingDistance = 18f;
	public float meleeUlt = 0f;
	public float damageBoost = 1f;
	public bool techAiming = false;
	
	public bool devMode = true;
	
	public float cooldownLeft = 0f;
	public float cooldownRight = 0f;
	public float points = 0f;
	public float ultUsed = 0f;
	public float ultMax = 400f;
	public float classChange = 0f;
	public Vector2[] classCooldowns = {
				  //   L    R
	    new Vector2( .65f, 13f ),  //6
		new Vector2( 1.1f, 15f ),  //7
		new Vector2(   1f,  10f )}; //8
	
	//Comment this out in mp build \/
	//public PhotonView view;
	
	// Start is called before the first frame update
	void Start()
	{
		
		//view = GetComponent<PhotonView>();
		//if (view.IsMine) {
			gravity = Physics.gravity.y;
			Fireball.GetComponent<SingleFireball>().sourcePlayer = transform;
			Snowball.GetComponent<Singlesnowball>().sourcePlayer = transform;
			ShadowField.GetComponent<SingleShadowField>().sourcePlayer = transform;
			Spike.GetComponent<SingleSpike>().sourcePlayer = transform;
			Trap.GetComponent<SingleTrap>().sourcePlayer = transform;
			Grapple.GetComponent<SingleGrapple>().sourcePlayer = transform;
			dummy.GetComponent<SingleDummy>().sourcePlayer = transform;
			Sphere.GetComponent<Renderer>().material.color = new Color(Random.value,Random.value,Random.value);
			Capsule.GetComponent<Renderer>().material.color = new Color(1f,0f,1f);
			topLight.GetComponent<Light>().enabled = (gameObject.layer == 7);
			bottomLight.GetComponent<Light>().enabled = (gameObject.layer == 7);
			
			pauseMenu.GetComponent<Canvas>().enabled = false;
			UiHealthShield.GetComponent<Image>().enabled = false;
			UiMeleeToast.GetComponent<Image>().enabled = false;
			UiMagicToast.GetComponent<Image>().enabled = false;
			UiTechToast.GetComponent<Image>().enabled = false;
			UiMelee1.GetComponent<Image>().enabled = (gameObject.layer == 6);
			UiMagic1.GetComponent<Image>().enabled = (gameObject.layer == 7);
			UiTech1.GetComponent<Image>().enabled = (gameObject.layer == 8);
			UiMelee2.GetComponent<Image>().enabled = (gameObject.layer == 6);
			UiMagic2.GetComponent<Image>().enabled = (gameObject.layer == 7);
			UiTech2.GetComponent<Image>().enabled = (gameObject.layer == 8);
			UiMelee3.GetComponent<Image>().enabled = (gameObject.layer == 6);
			UiMagic3.GetComponent<Image>().enabled = (gameObject.layer == 7);
			UiTech3.GetComponent<Image>().enabled = (gameObject.layer == 8);
		//}
	}
	
	// Update is called once per frame
	void Update()
	{
	//if (view.IsMine) 
	//{
	  //Pause unpause
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame();
			SceneManager.LoadScene("MainMenu");
		}
		
	  //UI CONTROL (others sprinkled into script. CTRL+F "Ui")
		//if (view.IsMine)
		//{
			UiHealth.GetComponent<Slider>().value = health;
			if(meleeUlt == 0f && myShadowField == null && !techAiming)
			{
				UiUlt.GetComponent<Slider>().value = Mathf.Min(points-ultUsed,ultMax);
				UiUltColor.GetComponent<Image>().color = new Color(0f,1f,1f);
			}
			else
			{
				UiUlt.GetComponent<Slider>().value = 400f;
				UiUltColor.GetComponent<Image>().color = new Color(1f-Random.value/3f,1f-Random.value/3f,1f-Random.value/3f);
			}
		//}
	  
		if(Input.GetKeyDown(KeyCode.H) && !isPaused && devMode)
			health = Mathf.Min(health,healthMin);
	  //Dead? (temporary for dev testing)
		if(health == 0f || health < 0f)
		{
			if(transform.parent != null)
				Destroy(transform.parent);
			if(health < 0f)
				Debug.Log("(Player Movement) ERROR: Health is below 0");
			if(meleeUlt != 0f)
			{
				Debug.Log("(Player Movement) ERROR: Player died and Melee Ult was active");
				meleeUlt = 0f;
				damageBoost = 1f;
			}
			gravityCoef = Vector3.up;
			yVel = Mathf.Sqrt(Mathf.Max(0f,200f-transform.position.y) * -2f * gravity);
			speedBoost = 45f;
			techAiming = false;
			classChange = Mathf.Min(classChange, 10f);
		}
		if(transform.position.y > 199f && health != 100f)
			health = 100f;
	  
	  //APPLY MOVEMENT
		if(transform.parent == null)
			controller.Move((movementVector*(speed+speedBoost) + gravityCoef*yVel + Vector3.down*0.01f) * Time.deltaTime);
		
	  //Health color
		Capsule.GetComponent<Renderer>().material.color = new Color(Mathf.Clamp01(2f-0.02f*health),Mathf.Clamp01(0.02f*health),0f,1f);
		
	  //Grounded
		controllerIsGrounded = controller.isGrounded;
		hitAngleIsGrounded = (hitAngle <= angleLimit);
		isGrounded = (controller.isGrounded && hitAngle <= angleLimit);
		if(transform.parent != null)
			isGrounded = true;
		
	  //Gravitational Acceleration
		yVel += gravity*Time.deltaTime;
		//was just vector3.up
		gravityCoef = gravityCoef*(1f-2f*Time.deltaTime)+Vector3.up*2f*Time.deltaTime;
		if(isGrounded)
		{
			yVel = Mathf.Max(yVel, -Mathf.Tan(Mathf.Min(hitAngle,angleLimit)*Mathf.Deg2Rad)*speed);
			gravityCoef = Vector3.up;
		}
		else
		{
			if(controller.isGrounded)
			{
				//There's GOTTA be a way to get the perpendicular normal more simply
				gravityCoef = new Vector3(hitHorizontal.x*Mathf.Cos(Mathf.Acos(hitHorizontalMagnitude)+Mathf.PI/2f),Mathf.Sin(Mathf.Asin(hitNormal.y)+Mathf.PI/2f),hitHorizontal.z*Mathf.Cos(Mathf.Acos(hitHorizontalMagnitude)+Mathf.PI/2f));
			}
		}
		
	  //Movement Input
		if(cam.transform.GetComponent<SingleMouseMovement>().transition == 0f && !techAiming && !isPaused)
		{
			movementVector = Vector3.ClampMagnitude(transform.right*Input.GetAxis("Horizontal") + transform.forward*Input.GetAxis("Vertical"),1f);
			if(Input.GetButtonDown("Jump") && isGrounded)
				yVel = Mathf.Sqrt(jumpHeight * -2f * gravity);
		}
		else
		{
			movementVector = Vector3.zero;
		}
		
	  //Left Click
		if(Input.GetMouseButtonDown(0) && cam.transform.GetComponent<SingleMouseMovement>().transition == 0f && !isPaused)
		{
			if(techAiming)
				TechUlt();
			else
			{
				if(cooldownLeft == 0f)
				{
					if(gameObject.layer == 6)
						MeleeLeft();
					if(gameObject.layer == 7)
						MagicLeft();
					if(gameObject.layer == 8)
						TechLeft();
				}
			}
		}
		
	  //Right Click
		if(Input.GetMouseButtonDown(1) && cam.transform.GetComponent<SingleMouseMovement>().transition == 0f && !isPaused)
		{
			if(gameObject.layer == 7)
				MagicRight();
			if(cooldownRight == 0f)
			{
				if(gameObject.layer == 6)
					MeleeRight();
				if(gameObject.layer == 8 && !techAiming)
					TechRight();
			}
		}
		
	  //Middle Click
		if((Input.GetMouseButtonDown(2) || Input.GetKeyDown(KeyCode.X)) && cam.transform.GetComponent<SingleMouseMovement>().transition == 0f && !isPaused)
		{
			if(points-ultUsed >= ultMax)
			{
				ultUsed = points;
				if(gameObject.layer == 6)
					MeleeUlt();
				if(gameObject.layer == 7)
					MagicUlt();
				if(gameObject.layer == 8)
					TechUlt();
			}
		}
		
	  //Scroll Aim (always active, but only matters for magic class)
		castingDistance = Mathf.Clamp(castingDistance + Input.mouseScrollDelta.y, 2f, 40f);
		if(Input.mouseScrollDelta.y != 0f)
		{
			topLight.transform.localPosition = Vector3.forward * castingDistance + Vector3.up * 100f;
			bottomLight.transform.localPosition = Vector3.forward * castingDistance + Vector3.up * -100f;
			topLight.GetComponent<Light>().cookieSize = (castingDistance*0.4f + 1f)*1.75f;
			bottomLight.GetComponent<Light>().cookieSize = (castingDistance*0.4f + 1f)*1.75f;
		}
		
	  //Cooldown Decay
		cooldownLeft  =  Mathf.Max(0f, cooldownLeft-1f*Time.deltaTime);
		cooldownRight = Mathf.Max(0f, cooldownRight-1f*Time.deltaTime);
		//if (view.IsMine)
		//{
			UiCooldown1.GetComponent<Image>().enabled = (cooldownLeft != 0f);
			UiCooldown2.GetComponent<Image>().enabled = (cooldownRight != 0f);
		//}
		
		speedBoost = Mathf.Max(0f, speedBoost-5f*damageBoost*Time.deltaTime);
		
	  //Water?
		if(transform.position.y <= 0.8f && health > 0f)
			health = Mathf.Max(0f, health - 15f * Time.deltaTime);
		
	  //Melee Ult Handling
		if(meleeUlt != 0f)
		{
			//if (view.IsMine)
				UiHealthShield.GetComponent<Image>().enabled = true;
			damageBoost = 1f+(1f-(health/100f))*2f;
			meleeUlt -= Time.deltaTime;
			if(meleeUlt <= 0f)
			{
				//if (view.IsMine)
					UiHealthShield.GetComponent<Image>().enabled = false;
				damageBoost = 1f;
				healthMin = 0f;
				meleeUlt = 0f;
				classChange = 10f;
			}
		}
	  
	  //Tech ult handling
	  
	  //ult class switch
		if(classChange > 0f)
		{
			classChange = 0f;
			gameObject.layer = (gameObject.layer+1)%3+6;
			topLight.GetComponent<Light>().enabled = (gameObject.layer == 7);
			bottomLight.GetComponent<Light>().enabled = (gameObject.layer == 7);
			//if (view.IsMine)
			//{
				UiMeleeToast.GetComponent<Image>().enabled = (classChange > 0f && gameObject.layer == 6);
				UiMagicToast.GetComponent<Image>().enabled = (classChange > 0f && gameObject.layer == 7);
				UiTechToast.GetComponent<Image>().enabled = (classChange > 0f && gameObject.layer == 8);
				UiMelee1.GetComponent<Image>().enabled = (gameObject.layer == 6);
				UiMagic1.GetComponent<Image>().enabled = (gameObject.layer == 7);
				UiTech1.GetComponent<Image>().enabled = (gameObject.layer == 8);
				UiMelee2.GetComponent<Image>().enabled = (gameObject.layer == 6);
				UiMagic2.GetComponent<Image>().enabled = (gameObject.layer == 7);
				UiTech2.GetComponent<Image>().enabled = (gameObject.layer == 8);
				UiMelee3.GetComponent<Image>().enabled = (gameObject.layer == 6);
				UiMagic3.GetComponent<Image>().enabled = (gameObject.layer == 7);
				UiTech3.GetComponent<Image>().enabled = (gameObject.layer == 8);
			//}
		}
		
	  //Dev summon dummy
		if(Input.GetKeyDown(KeyCode.R) && !isPaused && devMode)
		{
			Transform myDummy;
			Vector3 looking = cam.transform.GetChild(0).position-cam.transform.position;
			myDummy = Instantiate(dummy, transform.position+new Vector3(looking.x,0f,looking.z)*3f, transform.rotation);
			myDummy.GetComponent<SingleDummy>().movementVector = movementVector;
		}
	  //Dev Charge Ult
		if(Input.GetKeyDown(KeyCode.U) && !isPaused && devMode)
			points = Mathf.Max(points, ultUsed+ultMax);
		
	  //Debug player path
		Debug.DrawRay(transform.position, Vector3.Normalize(gravityCoef), Color.blue, 0.0f, false);
		Debug.DrawRay(cam.transform.position, cam.transform.GetChild(0).position-cam.transform.position, Color.red, 0.0f, false);
	  //Independent Error Logging
		if((transform.localRotation.x != 0f) || (transform.localRotation.z != 0f))
			Debug.Log("(Player Movement) ERROR: X and/or Z Rot is not 0");
		if( health<0f  )
			Debug.Log("(Player Movement) ERROR: Health is below 0");
		if( float.IsNaN(health) )
			Debug.Log("(Player Movement) ERROR: Health is NaN");
	//}
	}
	
	void OnControllerColliderHit(ControllerColliderHit hit)
	{
		hitNormal = hit.normal;
		hitHorizontal = Vector3.Normalize(new Vector3(hitNormal.x,0f,hitNormal.z));
		hitHorizontalMagnitude = new Vector3(hitNormal.x, 0f, hitNormal.z).magnitude;
		hitAngle = Mathf.Atan(hitHorizontalMagnitude/hitNormal.y)*Mathf.Rad2Deg;
	}
	
	public void PauseGame()
    {
        //if (view.IsMine)
			Gui.GetComponent<Canvas>().enabled = false;
        pauseMenu.GetComponent<Canvas>().enabled = true;
        isPaused = true;
		Cursor.lockState = CursorLockMode.None;
    }

    public void ResumeGame()
    {
		//if (view.IsMine)
			Gui.GetComponent<Canvas>().enabled = true;
        pauseMenu.GetComponent<Canvas>().enabled = false;
        isPaused = false;
		Cursor.lockState = CursorLockMode.Locked;
    }
	
	//CLASS ABILITIES
	void MeleeLeft()
	{
	  //Shiny Sword
		speedBoost = 2.5f*damageBoost;
		Vector3 looking = cam.transform.GetChild(0).position-cam.transform.position;
		Collider[] colliders = Physics.OverlapBox(cam.transform.position+1.65f*looking, new Vector3(1.7f/2f,1f/2f,2.3f/2f), Quaternion.LookRotation(cam.transform.position+2f*looking), 448);
		foreach(Collider c in colliders)
		{
			if(c.tag == "Player" && c.transform != gameObject.transform)
			{
			//Vector3.Angle(looking,c.transform.position-cam.transform.position) <= 45f/2f
			//Add a coeff that reduces damage based off angle
				SinglePlayerMovement player = c.transform.GetComponent<SinglePlayerMovement>();
				float damage = 22f*damageBoost;
				player.health = Mathf.Clamp(player.health-damage,0f,player.health);
				Points(damage);
			}
			//Dev testing dummy
			if(c.tag == "Dummy")
			{
				SingleDummy dummy = c.transform.GetComponent<SingleDummy>();
				float damage = 22f*damageBoost;
				dummy.health = Mathf.Clamp(dummy.health-damage,0f,dummy.health);
				Points(damage);
			}
		}
		
		cooldownLeft = classCooldowns[0].x; //%cooldown
	}
	void MeleeRight()
	{
	  //Sho'gun
		Vector3 looking = cam.transform.GetChild(0).position-cam.transform.position;
		gravityCoef = 1.5f*new Vector3( looking.x, Mathf.Min(-0.5f, looking.y), looking.z );
		yVel = -15f;
		
		Collider[] colliders = Physics.OverlapCapsule(cam.transform.position+2f*looking-0.85f*Vector3.up, cam.transform.position+5f*looking-0.85f*Vector3.up, 2f, 448);
		foreach(Collider c in colliders)
		{
			if(c.tag == "Player" && c.transform != gameObject.transform)
			{
			//Vector3.Angle(looking,c.transform.position-cam.transform.position) <= 45f/2f
			//Add a coeff that reduces damage based off angle
				SinglePlayerMovement player = c.transform.GetComponent<SinglePlayerMovement>();
				float damage = Mathf.Sqrt(1-Mathf.Min(1f,Vector3.Distance(cam.transform.position,c.transform.position)/5f))*Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad*Vector3.Angle(looking,c.transform.position-cam.transform.position)),2f)*63f*damageBoost;
				player.health = Mathf.Clamp(player.health-damage,0f,player.health);
				Points(damage);
			}
			//Dev testing dummy
			if(c.tag == "Dummy")
			{
				SingleDummy dummy = c.transform.GetComponent<SingleDummy>();
				float damage = Mathf.Sqrt(1-Mathf.Min(1f,Vector3.Distance(cam.transform.position,c.transform.position)/5f))*Mathf.Pow(Mathf.Cos(Mathf.Deg2Rad*Vector3.Angle(looking,c.transform.position-cam.transform.position)),2f)*63f*damageBoost;
				dummy.health = Mathf.Clamp(dummy.health-damage,0f,dummy.health);
				Points(damage);
			}
		}
		
		cooldownRight = classCooldowns[0].y; //%cooldown
	}
	void MeleeUlt()
	{
	  //Coal Health
		meleeUlt = 30f;
		healthMin = Mathf.Min(health,15f);
	}
	void MagicLeft()
	{
	  //FIIIIREBAALLLL!!!
	  //Fireball.cs
		Instantiate(Fireball, transform.position+new Vector3(Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y),0.375f,Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y)), transform.rotation);
		cooldownLeft = classCooldowns[1].x; //%cooldown
	}
	void MagicRight()
	{
	  //Snowball
	  //snowball.cs
		if(mySnowball == null)
		{
			if(cooldownRight == 0f)
			{
				mySnowball = Instantiate(Snowball, transform.position+new Vector3(Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y),-0.25f,Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y))*2f, transform.rotation);
				cooldownRight = classCooldowns[1].y; //%cooldown
			}
		}
		else
		{
			mySnowball.GetComponent<Singlesnowball>().DestroyBall();
			mySnowball = null;
		}
	}
	void MagicUlt()
	{
	  //Shadow Field
	  //ShadowField.cs
		myShadowField = Instantiate(ShadowField, transform.position-Vector3.up, transform.rotation);
		classChange = 30f;
	}
	void TechLeft()
	{
	  //Spike Ballista
		Vector3 looking = cam.transform.GetChild(0).position-cam.transform.position;
		Instantiate(Spike, cam.transform.position+new Vector3(Mathf.Sin(Mathf.Deg2Rad*transform.eulerAngles.y),0f,Mathf.Cos(Mathf.Deg2Rad*transform.eulerAngles.y)), Quaternion.LookRotation(looking+Vector3.up*0.3f));
		cooldownLeft = classCooldowns[2].x; //%cooldown
		
	}
	void TechRight()
	{
	  //Bear Trap
		Vector3 looking = cam.transform.GetChild(0).position-cam.transform.position;
		RaycastHit hit;
		if(Physics.Raycast(transform.position+2f*new Vector3(looking.x, 0f, looking.z), Vector3.down, out hit, 27f, 8))
		{
			foreach(Transform c in myTraps)
			{
				c.transform.GetComponent<SingleTrap>().health -= 50f;
			}
			myTraps.Add(Instantiate(Trap, hit.point, transform.rotation) as Transform);
		}
		cooldownRight = classCooldowns[2].y; //%cooldown
	}
	void TechUlt()
	{
	  //Grapple Stake
		if(techAiming)
		{
			Instantiate(Grapple, transform.position, transform.rotation);
			cam.GetComponent<SingleMouseMovement>().Unaim();
			techAiming = false;
			classChange = 10f;
		}
		else
		{
			cam.GetComponent<SingleMouseMovement>().Aim();
			techAiming = true;
		}
	}
	
	public void Points(float a)
	{
		points += a;
		if(meleeUlt > 0f || myShadowField != null || techAiming == true) //Eventually replace with ult active
			ultUsed = points;
	}
}
