using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Com.MyCompany.LiamsGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region public fields

        [Tooltip("Pixel offset from the player target")]
        [SerializeField]
        private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        #endregion


        #region Private Fields

        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasgroup;
        Vector3 targetPosition;

        [Tooltip("UI Text to display Player's Name")]
        [SerializeField]
        private Text playerNameText;

        [Tooltip("UI Slider to display Player's Health")]
        [SerializeField]
        private Slider playerHealthSlider;

        private PlayerManager target;

        #endregion

        #region MonoBehaviour Callbacks
        void Start()
        {

        }

        private void Awake()
        {
            _canvasgroup = this.GetComponent<CanvasGroup>();
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
        }

        private void LateUpdate()
        {
            if(targetRenderer != null)
            {
                this._canvasgroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            if(targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;

            }
        }

        // Update is called once per frame
        void Update()
        {
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.Health;
            }
            if (target == null)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        #endregion

        #region Public Methods


        #endregion

        public void SetTarger(PlayerManager _target)
        {
            if(_target == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> PlayMakerManager target for PlayerUI.SetTarget.", this);
                return;
            }

            target = _target;
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            if(characterController != null)
            {
                characterControllerHeight = characterController.height;
            }
            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }
        // Start is called before the first frame update
        
    }
}
