using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;

namespace Com.MyCompany.LiamsGame
{
    /// <summary>
    /// Player Manager handles fire input and beams
    /// </summary>
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        #region IPunObservable Implementation
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting)
            {
                stream.SendNext(IsFiring);
                stream.SendNext(Health);
            }
            else
            {
                this.IsFiring = (bool)stream.ReceiveNext();
                this.Health = (float)stream.ReceiveNext();
            }
        }
        #endregion

        #region Private Fields

        [Tooltip("the players ui gameobject prefab")]
        [SerializeField]
        private GameObject playerUIPrefab;

        [Tooltip("The beams gameobject being controlled")]
        [SerializeField]
        private GameObject beams;
        bool IsFiring;

        #endregion

        #region Public Fields
        [Tooltip("current health of player")]
        public float Health = 1f;
        public static GameObject LocalPlayerInstance;
        #endregion

        #region MonoBehaviour Callbacks
        /// <summary>
        /// MB called on GO by unity
        /// </summary>
        private void Awake()
        {
            if(beams == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> Beams Reference.", this);
            }
            else
            {
                beams.SetActive(false);
            }

            if(photonView.IsMine)
            {
                LocalPlayerInstance = gameObject;
            }
            DontDestroyOnLoad(gameObject);
        }
        private void Start()
        {
            CameraWork _cameraWork = gameObject.GetComponent<CameraWork>();

            if(_cameraWork != null)
            {
                if(photonView.IsMine)
                {
                    _cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> CameraWork Component on playerPrefab.", this);

            }

            if(this.playerUIPrefab!= null)
            {
                //GameObject_uiGo = Instantiate(this.playerUIPrefab);
               // _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            }
            else
            {
                Debug.LogWarning("<Color=Red><b>Missing</b></Color> PlayerUiPrefab reference on player Prefab", this);
            }
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }

        public override void OnDisable()
        {
            base.OnDisable();
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        #region private methods
        #if UNITY_5_4_OR_NEWER
        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingmode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }
        #endif
        #endregion
        /// <summary>
        /// called every frame by unity
        /// </summary>
        // Update is called once per frame
        void Update()
        {
            ProcessInputs();

            if (beams != null && IsFiring != beams.activeInHierarchy)
            {
                beams.SetActive(IsFiring);

                
            }
            if (Health <= 0f)
            {
                GameManager.Instance.LeaveRoom();
            }
        }

        void OnTriggerEnter(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if(!other.name.Contains("Beam"))
            {
                return;
            }
            Health -= 0.1f;
        }

        private void OnTriggerStay(Collider other)
        {
            if (!photonView.IsMine)
            {
                return;
            }

            if(!other.name.Contains("Beam"))
            {
                return;
            }

            Health -= 0.1f * Time.deltaTime;
        }

        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }
        void CalledOnLevelWasLoaded(int level)
        {
            GameObject _uiGo = Instantiate(this.playerUIPrefab);
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

        #endregion

        #region Custom Fire Inputs
        void ProcessInputs()
        {
            if(Input.GetButtonDown("Fire1"))
            {
                if(!IsFiring)
                {
                    IsFiring = true;
                }
            }
            if(Input.GetButtonUp("Fire1"))
            {
                if(IsFiring)
                {
                    IsFiring = false;
                }
            }
        }
        #endregion
    }
}



