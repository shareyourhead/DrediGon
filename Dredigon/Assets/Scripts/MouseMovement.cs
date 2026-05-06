using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class MouseMovement : MonoBehaviour
{
	public Transform playerBody = null;
	public PlayerMovement playerMovement = null;
	
	public float mouseSens = 4.5f;
	public bool aerialView = false;
	public float transition = 0.0f;
	private float swapTime = 1.4f;
	
	private float xRot = 0f;
	public float xRotCache;
	
	public Transform Gui;
	public Transform pauseMenu;
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
	
	//MatrixBlender
	private Matrix4x4   ortho,
						perspective,
						aiming;
	public float		fov	 = 90f,
						near	= .01f,
						far	 = 1000f,
						orthographicSize = 100f;
	private float aspect;
	public MatrixBlender blender;
	
	// Start is called before the first frame update
	void Start()
	{
		Cursor.lockState = CursorLockMode.Locked;
		//MatrixBlender
		aspect = (float) Screen.width / (float) Screen.height;
		ortho = Matrix4x4.Ortho(-orthographicSize * aspect, orthographicSize * aspect, -orthographicSize, orthographicSize, near, far);
		perspective = Matrix4x4.Perspective(fov, aspect, near, far);
		aiming = Matrix4x4.Perspective(fov/3f, aspect, near, far);
		GetComponent<Camera>().projectionMatrix = perspective;
		blender = (MatrixBlender) GetComponent(typeof(MatrixBlender));
		
		pauseMenu.GetComponent<Canvas>().enabled = false;
		UiHealthShield.GetComponent<Image>().enabled = false;
		UiMeleeToast.GetComponent<Image>().enabled = false;
		UiMagicToast.GetComponent<Image>().enabled = false;
		UiTechToast.GetComponent<Image>().enabled = false;
	}
	
	// Update is called once per frame
	void Update()
	{	
		if(transform.parent == null)
		{
			GameObject[] players;
			players = GameObject.FindGameObjectsWithTag("Player");
			foreach(GameObject p in players)
			{
				PhotonView view;
				view = p.GetComponent<PhotonView>();
				if(view.IsMine)
				{
					transform.parent = p.transform;
					playerBody = p.transform;
					playerMovement = playerBody.GetComponent<PlayerMovement>();
					transform.localPosition = Vector3.up * 0.85f;
					
					
					playerMovement.cam = transform.gameObject;
		
					playerMovement.Gui = Gui;
					playerMovement.pauseMenu = pauseMenu;
					playerMovement.UiHealth = UiHealth;
					playerMovement.UiHealthShield = UiHealthShield;
					playerMovement.UiUlt = UiUlt;
					playerMovement.UiUltColor = UiUltColor;
					playerMovement.UiMeleeToast = UiMeleeToast;
					playerMovement.UiMagicToast = UiMagicToast;
					playerMovement.UiTechToast = UiTechToast;
					playerMovement.UiMelee1 = UiMelee1;
					playerMovement.UiMagic1 = UiMagic1;
					playerMovement.UiTech1 = UiTech1;
					playerMovement.UiCooldown1 = UiCooldown1;
					playerMovement.UiMelee2 = UiMelee2;
					playerMovement.UiMagic2 = UiMagic2;
					playerMovement.UiTech2 = UiTech2;
					playerMovement.UiCooldown2 = UiCooldown2;
					playerMovement.UiMelee3 = UiMelee3;
					playerMovement.UiMagic3 = UiMagic3;
					playerMovement.UiTech3 = UiTech3;
					
					UiMelee1.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 6);
					UiMagic1.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 7);
					UiTech1.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 8);
					UiMelee2.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 6);
					UiMagic2.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 7);
					UiTech2.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 8);
					UiMelee3.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 6);
					UiMagic3.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 7);
					UiTech3.GetComponent<Image>().enabled = (transform.parent.gameObject.layer == 8);
				}
			}
			return;
		}
		{
			
			if(transition == 0.0f && !playerMovement.isPaused)
			{
				float mouseX = Input.GetAxis("Mouse X") * mouseSens;
				float mouseY = Input.GetAxis("Mouse Y") * mouseSens;
				xRot -= mouseY;
				xRot = Mathf.Clamp(xRot, -90f, 90f);
				playerBody.transform.Rotate(Vector3.up * mouseX);
				transform.localRotation = Quaternion.Euler(xRot, 0f, 0f);
			}
			
			if((Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Tab)) && !playerMovement.techAiming && !playerMovement.isPaused)
			{
				aerialView = !aerialView;
				if(aerialView)
				{
					Cursor.lockState = CursorLockMode.Confined;
					blender.BlendToMatrix(ortho, swapTime*(1f-transition));
					if(transition == 0.0f)
						xRotCache = xRot;
				}
				else
				{
					Cursor.lockState = CursorLockMode.Locked;
					blender.BlendToMatrix(perspective, swapTime*transition);
				}
			}
			
			if(!aerialView && transition != 0.0f)
			{
				transition = Mathf.Max(0.0f, transition - Time.deltaTime/swapTime);
				CameraUpdate();
			}
			
			if(aerialView && transition != 1.0f)
			{
				transition = Mathf.Min(1.0f, transition + Time.deltaTime/swapTime);
				CameraUpdate();
			}
			
		  //Dev change sens
			if(Input.GetKeyDown(KeyCode.LeftBracket) && !playerMovement.techAiming)
				mouseSens = Mathf.Max(0f,mouseSens-0.5f);
			if(Input.GetKeyDown(KeyCode.RightBracket)&& !playerMovement.techAiming)
				mouseSens += 0.5f;
		}
	}
	
	void CameraUpdate()
	{
		transform.localPosition = Vector3.up * (Mathf.Pow(transition,2f)*(100f-0.85f)+0.85f);
		transform.localEulerAngles = Vector3.right * (Mathf.Sqrt(transition)*(90-xRotCache)+xRotCache);
	}
	
	public void Aim()
	{
		blender.BlendToMatrix(aiming, 0.2f);
		mouseSens = mouseSens/3f;
	}
	
	public void Unaim()
	{
		blender.BlendToMatrix(perspective, 0.2f);
		mouseSens = mouseSens*3f;
	}
}
