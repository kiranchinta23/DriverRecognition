using UnityEngine;
using System.Collections;
//using Windows.Kinect;


public class SkeletonOverlayer : MonoBehaviour 
{
//	[Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
//	public GUITexture backgroundImage;

	[Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
	public Camera foregroundCamera;
	
	[Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
	public int playerIndex = 0;
	
	[Tooltip("Game object used to overlay the joints.")]
	public GameObject jointPrefab;

	//public float smoothFactor = 10f;

	//public UnityEngine.UI.Text debugText;
	
	private GameObject[] joints = null;

	private Quaternion initialRotation = Quaternion.identity;


	void Start()
	{
		KinectManager manager = KinectManager.Instance;

		if(manager && manager.IsInitialized())
		{
			int jointsCount = manager.GetJointCount();

			if(jointPrefab)
			{
				// array holding the skeleton joints
				joints = new GameObject[jointsCount];
				
				for(int i = 0; i < joints.Length; i++)
				{

					joints[i] = Instantiate(jointPrefab) as GameObject;
					joints[i].transform.parent = transform;
					joints[i].name = ((KinectInterop.JointType)i).ToString();
					joints[i].SetActive(false);
				}
			}
		}

		// always mirrored
		initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

		if (!foregroundCamera) 
		{
			// by default - the main camera
			foregroundCamera = Camera.main;
		}
	}
	
	void Update () 
	{
		KinectManager manager = KinectManager.Instance;
		
		if(manager && manager.IsInitialized() && foregroundCamera)
		{
//			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
//			if(backgroundImage && (backgroundImage.texture == null))
//			{
//				backgroundImage.texture = manager.GetUsersClrTex();
//			}

			// get the background rectangle (use the portrait background, if available)
			Rect backgroundRect = foregroundCamera.pixelRect;
			PortraitBackground portraitBack = PortraitBackground.Instance;
			
			if(portraitBack && portraitBack.enabled)
			{
				backgroundRect = portraitBack.GetBackgroundRect();
			}

			// overlay all joints in the skeleton
			if(manager.IsUserDetected(playerIndex))
			{
				long userId = manager.GetUserIdByIndex(playerIndex);
				int jointsCount = manager.GetJointCount();

				for(int i = 0; i < jointsCount; i++)
				{
					int joint = i;

					if(manager.IsJointTracked(userId, joint))
					{
						Vector3 posJoint = manager.GetJointPosColorOverlay(userId, joint, foregroundCamera, backgroundRect);
						//Vector3 posJoint = manager.GetJointPosition(userId, joint);

						if(joints != null)
						{
							// overlay the joint
							if(posJoint != Vector3.zero)
							{
								joints[i].SetActive(true);
								joints[i].transform.position = posJoint;

								Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
								rotJoint = initialRotation * rotJoint;
								joints[i].transform.rotation = rotJoint;
							}
							else
							{
								joints[i].SetActive(false);
							}
						}
					}
					else
					{
						if(joints != null)
						{
							joints[i].SetActive(false);
						}
					}
				}

			}
		}
	}

}
