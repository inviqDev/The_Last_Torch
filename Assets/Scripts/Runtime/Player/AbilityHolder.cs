using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Runtime
{
    public class AbilityHolder : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI abilityName;
        [SerializeField] private Button abilityButton;
        [SerializeField] private Image abilityIcon;
        [SerializeField] private Slider abilityCooldown;

        // public void SetUpAbilityHolder(Ability ability, bool interactable)
        // {
        //     abilityName.text = ability.Name;
        //     abilityIcon.sprite = ability.Logo;
        //     abilityCooldown.value = ability.Cooldown;
        //
        //     SetInteractableStatus(interactable);
        // }

        public void SetInteractableStatus(bool interactable)
        {
            abilityButton.interactable = interactable;
        }

        public void MoveCooldownSlider(float value)
        {
            abilityCooldown.value = value;
        }

        // [SerializeField] private Ability[] abilities;

        // // Find the nearest enemy able to attack
        // public EnemyModel enemy;
        // public Animator animator;
        //
        // public void ActivateAbility(int index)
        // {
        //     abilities[index].Activate(enemy, animator);
        // }


        // [SerializeField] private Slider _cooldownSlider;
        // [SerializeField] private Image _image;
        //
        // private SkillsBar skillsBar;
        //
        // private Ability _currentConfig;
        // private readonly float _minValue = 0.0f;
        // private float _maxValue;
        // private bool _activeOnStart;
        //
        // private Timer skillTimer;
        //
        // private void Awake()
        // {
        //     skillsBar = GetComponentInParent<SkillsBar>();
        //     UnityEngine.Assertions.Assert.IsNotNull(skillsBar, "skillsBar is not found");
        // }
        //
        // private void Start()
        // {
        //     skillsBar.ResetToDefault();
        //     skillsBar.SetNewSkill(startSkill);
        // }
        //
        // public void SetDefaultSkillBarView()
        // {
        //     _cooldownSlider.gameObject.SetActive(false);
        //     _image.sprite = defaultConfig.abilityButton;
        // }
        //
        // public void SetUpSkill(Ability ability)
        // {
        //     _currentConfig = ability;
        //     
        //     _cooldownSlider.gameObject.SetActive(true);
        //     
        //     _maxValue = ability.cooldownTime;
        //     _cooldownSlider.minValue = _minValue;
        //     _cooldownSlider.maxValue = _maxValue;
        //     
        //     _cooldownSlider.value = _minValue;
        //
        //     skillTimer = new Timer(this);
        //     skillTimer.OnAnyValueChanged += ShowSkillCooldown;
        //     skillTimer.TimerIsOver += OnTimerIsOver;
        //     
        //     skillTimer.StartFromToTimer(_minValue, _maxValue, TimerType.Increasing);
        // }
        //
        // private void OnTimerIsOver()
        // {
        //     skillTimer.StopTimer();
        //     SetUpSkill(_currentConfig);
        // }
        //
        // private void ShowSkillCooldown(float currentTimerValue)
        // {
        //     _cooldownSlider.value = currentTimerValue;
        // }
        //
        // // on attack => set value to 0 => start timer => onCooldownIsOver => set "skill active"
    }
}