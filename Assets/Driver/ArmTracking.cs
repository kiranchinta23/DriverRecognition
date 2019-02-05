using UnityEngine;
using System.Collections;
//using Windows.Kinect;
using UnityEngine.UI;


public class ArmTracking : MonoBehaviour
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

        if (manager && manager.IsInitialized())
        {
            int jointsCount = manager.GetJointCount();

            if (jointPrefab)
            {
                // array holding the skeleton joints
                joints = new GameObject[jointsCount];

                for (int i = 0; i < joints.Length; i++)
                {

                    joints[i] = Instantiate(jointPrefab) as GameObject;
                    joints[i].transform.parent = transform;
                    joints[i].name = ((KinectInterop.JointType)i).ToString();
                    joints[i].GetComponent<MeshRenderer>().enabled = false;
                    joints[i].SetActive(false);

                    //Debug.Log(i.ToString() + " : " + joints[i].name);
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

    void Update()
    {
        KinectManager manager = KinectManager.Instance;

        if (manager && manager.IsInitialized() && foregroundCamera)
        {
            //			//backgroundImage.renderer.material.mainTexture = manager.GetUsersClrTex();
            //			if(backgroundImage && (backgroundImage.texture == null))
            //			{
            //				backgroundImage.texture = manager.GetUsersClrTex();
            //			}

            // get the background rectangle (use the portrait background, if available)
            Rect backgroundRect = foregroundCamera.pixelRect;
            PortraitBackground portraitBack = PortraitBackground.Instance;

            if (portraitBack && portraitBack.enabled)
            {
                backgroundRect = portraitBack.GetBackgroundRect();
            }

            // overlay all joints in the skeleton
            if (manager.IsUserDetected(playerIndex))
            {
                DriverManager.instance.OutObjList[0].GetComponent<Text>().text = "Driver : Recognized";
                DriverManager.instance.OutObjList[0].SetActive(true);

                long userId = manager.GetUserIdByIndex(playerIndex);
                int jointsCount = manager.GetJointCount();

                for (int i = 0; i < jointsCount; i++)
                {
                    int joint = i;

                    if (manager.IsJointTracked(userId, joint))
                    {

                        Vector3 posJoint = manager.GetJointPosColorOverlay(userId, joint, foregroundCamera, backgroundRect);
                        //Vector3 posJoint = manager.GetJointPosition(userId, joint);

                        if (joints != null)
                        {
                            // overlay the joint
                            if (posJoint != Vector3.zero)
                            {
                                joints[i].SetActive(true);
                                joints[i].transform.position = posJoint;

                                Quaternion rotJoint = manager.GetJointOrientation(userId, joint, false);
                                rotJoint = initialRotation * rotJoint;
                                joints[i].transform.rotation = rotJoint;

                                ////Main Routine
                                if(joints[6].activeSelf)
                                {
                                    ///LeftArm Gaze
                                    if (joints[6].transform.position.y > joints[1].transform.position.y)
                                    {
                                        if ((joints[6].transform.position.y - joints[1].transform.position.y) > 0.05f)
                                        {
                                            DriverManager.instance.OutObjList[3].GetComponent<Text>().text = "LeftArm : Up";
                                            DriverManager.instance.OutObjList[3].SetActive(true);
                                        }
                                        else
                                        {
                                            DriverManager.instance.OutObjList[3].SetActive(true);
                                        }
                                    }
                                    else if ((joints[6].transform.position.x - joints[4].transform.position.x) > 0.05f)
                                    {
                                        DriverManager.instance.OutObjList[3].GetComponent<Text>().text = "LeftArm : Down";
                                        DriverManager.instance.OutObjList[3].SetActive(true);
                                    }
                                    else
                                    {
                                        DriverManager.instance.OutObjList[3].SetActive(false);
                                    }
                                    ///LeftArm Position
                                    if ((joints[4].transform.position.x - joints[6].transform.position.x) > 0.15f)
                                    {
                                        DriverManager.instance.OutObjList[1].GetComponent<Text>().text = "LeftArm : Left";
                                        DriverManager.instance.OutObjList[1].SetActive(true);
                                    }
                                    else
                                    {
                                        DriverManager.instance.OutObjList[1].SetActive(false);
                                    }
                                }
                                if (joints[10].activeSelf)
                                {
                                    ///RightArm Gaze
                                    if (joints[10].transform.position.y > joints[1].transform.position.y)
                                    {
                                        if((joints[10].transform.position.y - joints[1].transform.position.y) > 0.05f)
                                        {
                                            DriverManager.instance.OutObjList[4].GetComponent<Text>().text = "RightArm : Up";
                                            DriverManager.instance.OutObjList[4].SetActive(true);
                                        }
                                        else
                                        {
                                            DriverManager.instance.OutObjList[4].SetActive(false);
                                        }
                                    }
                                    else if ((joints[8].transform.position.x - joints[10].transform.position.x) > 0.02f)
                                    {
                                        DriverManager.instance.OutObjList[4].GetComponent<Text>().text = "RightArm : Down";
                                        DriverManager.instance.OutObjList[4].SetActive(true);
                                    }
                                    else
                                    {
                                        DriverManager.instance.OutObjList[4].SetActive(false);
                                    }
                                    ///RightArm Position
                                    if ((joints[10].transform.position.x - joints[8].transform.position.x) > 0.1f)
                                    {
                                        DriverManager.instance.OutObjList[2].GetComponent<Text>().text = "RightArm : Right";
                                        DriverManager.instance.OutObjList[2].SetActive(true);
                                    }
                                    else
                                    {
                                        DriverManager.instance.OutObjList[2].SetActive(false);
                                    }
                                }
                                //Debug.Log("joint" + joints[6].transform.position.y.ToString());
                            }
                            else
                            {
                                joints[i].SetActive(false);
                            }
                        }
                    }
                    else
                    {
                        if (joints != null)
                        {
                            joints[i].SetActive(false);
                        }
                    }
                }

            }
            else
            {
                foreach (GameObject OutObj in DriverManager.instance.OutObjList)
                {
                    OutObj.SetActive(false);
                }
            }
        }
    }

}
