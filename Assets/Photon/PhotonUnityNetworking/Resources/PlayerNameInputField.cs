using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.LiamsGame
{   /// <summary>
    /// Player name input field. Let the user input their name, this will appear above them in the game
    /// </summary>
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constraints

        //Store the Playerpref key to avoid typos
        const string playerNamePrefKey = "PlayerName";

        #endregion

        #region MonoBehaviour Callbacks


        ///<summary>
        ///MonoBehaviour method called on GameObject by Unity during initialisation phase.
        ///</summary>
        private void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = this.GetComponent<InputField>();
            if (_inputField != null)
            {
                if(PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            PhotonNetwork.NickName = defaultName;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Sets the name of the plater and saves it in the PlayerPrefs for future sessions.
        /// </summary>
        /// <param name= "value">The name of the Player</param>
        public void SetPlayerName(string value)
        {
            //#Important
            if(string.IsNullOrEmpty(value))
            {
                Debug.LogError("Player name cannot be empty");
                return;
            }
            PhotonNetwork.NickName = value;

            PlayerPrefs.SetString(playerNamePrefKey, value);
        }
        #endregion
    }
}

