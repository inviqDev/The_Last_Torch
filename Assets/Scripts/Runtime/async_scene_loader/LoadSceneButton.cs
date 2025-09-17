using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class LoadSceneButton : MonoBehaviour
    {
        [SerializeField] private TransitionAsset transition;

        private void Awake()
        {
            var button = GetComponent<Button>();
            button.onClick.AddListener(OnStartGameButtonClick);
        }

        private void OnStartGameButtonClick()
        {
            TransitionService.Instance.Load(transition);
        }
    }
}