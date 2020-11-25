using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

namespace Com.MyCompany.LiamsGame
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        #region Public Fields
        public static GameManager Instance;
        [Tooltip("The prefab for representing the player")]
        public GameObject playerPrefab;
        #endregion

        #region Photon Callbacks

        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName);

            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", otherPlayer.NickName);

            if(PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient{0}", PhotonNetwork.IsMasterClient);

                LoadArena();
            }
        }

        /// <summary>
        /// Called when the local player left the room. The launcher scene needs to be loaded. 
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene(0);
        }

        #endregion

        #region Public Methods
        [System.Obsolete]
        private void Start()
        {
            Instance = this;

            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GO 'GameManager'", this);
            }
            else
            {
                if (PlayerManager.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are instantiating LocalPlayer from {0}", Application.loadedLevelName);
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
                }
                else
                {
                    Debug.LogFormat("ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }
        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }
        #endregion

        #region Private Methods

        void LoadArena()
        {
            if(!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to load a level but we are not the master client");
            }
            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.CurrentRoom.PlayerCount);
        }
        #endregion

    }
}


