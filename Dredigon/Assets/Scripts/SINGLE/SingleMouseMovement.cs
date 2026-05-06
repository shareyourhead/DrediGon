using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleMouseMovement : MonoBehaviour
{
	public GameObject playerBody;
	public SinglePlayerMovement playerMovement;
	
	public float mouseSens = 4.5f;
	public bool aerialView = false;
	public float transition = 0.0f;
	private float swapTime = 1.4f;
	
	private float xRot = 0f;
	public float xRotCache;
	
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
	}
	
	// Update is called once per frame
	void Update()
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
