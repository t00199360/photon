using UnityEngine;
using System.Collections;


namespace Com.MyCompany.LiamsGame
{
    /// <summary>
    /// Camera work will follow the target(users character)
    /// </summary>
    public class CameraWork : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("The dostance in the local x-z plane to the target")]
        [SerializeField]
        private float distance = 7.0f;

        [Tooltip("The height we want the camera to be above the target")]
        [SerializeField]
        private float height = 3.0f;

        [Tooltip("Allow the camera to be offset vertically from the target")]
        [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manually call OnStartFollowing() when and if needed.")]
        [SerializeField]
        private bool followOnStart = false;

        [Tooltip("The smoothing for the camera to follow the target")]
        [SerializeField]
        private float smoothSpeed = 0.125f;

        //cached transform of the target
        Transform cameraTransform;

        //maintain a flag internally to reconnect if the target is lost or the camera is switched
        bool isFollowing;

        //Cache for camera offset
        Vector3 cameraOffset = Vector3.zero;

        #endregion

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            if (followOnStart)
            {
                OnStartFollowing();
            }
        }
        void LateUpdate()
        {
            // The transform target may not destroy on level load,
            // so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
            if (cameraTransform == null && isFollowing)
            {
                OnStartFollowing();
            }


            // only follow is explicitly declared
            if (isFollowing)
            {
                Follow();
            }
        }
        #endregion

        #region Public Methods

        public void OnStartFollowing()
        {
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            Cut();
        }
        #endregion

        #region Private Methods

        void Follow()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.cameraTransform.position + this.transform.TransformVector(cameraOffset), smoothSpeed * Time.deltaTime);

            cameraTransform.LookAt(this.transform.position + centerOffset);

        }

        void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

            cameraTransform.LookAt(this.transform.position + centerOffset);
        }

        #endregion
        // Update is called once per frame
        void Update()
        {

        }
    }
}
