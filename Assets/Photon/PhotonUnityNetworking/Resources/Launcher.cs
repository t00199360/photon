using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;

namespace Com.MyCompany.LiamsGame
{
    public class Launcher : MonoBehaviourPunCallbacks
    {

        #region Private Serializable fields
        /// <summary>
        /// The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created.
        /// </summary>
        [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
        [SerializeField]
        private byte maxPlayersPerRoom = 10;
        #endregion

        #region Private Fields

        /// <summary>
        /// This client's version number.
        /// </summary>
        string gameVersion = "1";

        ///<summary>
        /// keep track of the current process. Connection is asynchronous and is based on several callbacks from Photon
        /// we need to keep track of this to properly adjust the behaviour when we receive call back by photon.
        /// typically this is used for the OnConnectedToMaster() callback
        ///</summary>
        bool isConnecting;
         

        #endregion

        #region MonoBehaviour CallBacks

        /// <summary>
        /// MonoBehavious method called on GameObject by Unity during runtime
        /// </summary>
        private void Awake()
        {
            //#Critical
            //this makes sure we can use PhotonNetwork.LoadLevel
            PhotonNetwork.AutomaticallySyncScene = true;
        }

        // Start is called before the first frame update
        void Start()
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
        }

        #endregion

        #region Public Fields
        [Tooltip("The Ui panel to let the user enter name, connect and play")]
        [SerializeField]
        private GameObject controlPanel;
        [Tooltip("The UI label to inform the user that the connection is in progress")]
        [SerializeField]
        private GameObject progressLabel;
        #endregion

        #region Public Methods

        /// <summary>
        /// Start the connection process
        /// - If already connected, we attempt joining a random room
        /// - If not yet connected, Connect this application instance to Photon Cloud Network
        /// </summary>

        public void Connect()
        {
            //we check if we are connected or not, we join if we are, else we instanciate the connection to the server.
            progressLabel.SetActive(true);
            controlPanel.SetActive(false);
            if (PhotonNetwork.IsConnected)
            {
                //#Critical we need at this point to attempt to joining a random room. If it fails, we'll get notified in OnJoinRandomFailed() and we'll create one.
                PhotonNetwork.JoinRandomRoom();
            }
            else
            {
                //#Critical, we must first and foremost connect to Photon Online Server.
                isConnecting = PhotonNetwork.ConnectUsingSettings();
                PhotonNetwork.GameVersion = gameVersion;
            }
        }
        #endregion

        #region MonoBehaviousPunCallbacks Callbacks
        public override void OnConnectedToMaster()
        {
            if(isConnecting)
            {
                //#Critical: we first need to try to join a potential existing room,If one exists, its fine. If one doesnt exist we call back OnJoinRandomFailed()
                PhotonNetwork.JoinRandomRoom();
                isConnecting = false;
            }
            
        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            progressLabel.SetActive(false);
            controlPanel.SetActive(true);
            isConnecting = false;
         
            Debug.Log("PUN basics tutorial/Launcher: OnDisconnected() was called by PUN with reason {0}");
        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log("PUN basics tutorial/launcher: OnJoinRandomFailed() was called by PUN. No random room available, so one will be created. \n Calling: PhotonNetwork.CreateRoom");

            //#Critical: We failed to join a random room, maybe one does not exist so we will attempt to create one
            PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
        }

        public override void OnJoinedRoom()
        {
            Debug.Log("PUN basics Tutorial/launcher: OnJoinedRoom() was called by PUN. The client is now in a room");

            if(PhotonNetwork.CurrentRoom.PlayerCount == 1)
            {
                Debug.Log("We load the 'Room for 1'");

                //#Critical
                //Load the room level
                PhotonNetwork.LoadLevel("Room for 1");
            }
        }
        #endregion
    }

}
