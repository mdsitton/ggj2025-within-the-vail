using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Comfort;

public class PlayerManager : MonoBehaviour, ITarget
{
    #region Properties
    public static PlayerManager instance;

    public PlayerHealth health;
    public PlayerVoice voice;
    #endregion

    #region Initialization
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Multiple PlayerManagers detected. Destroying the newest one.");
            Destroy(gameObject);
            return;
        }
        else if (health == null || voice == null)
        {
            Debug.LogWarning("PlayerManager is missing health or voice settings. Destroying the object.");
            Destroy(gameObject);
            return;
        }

        voice.Initialize(this);
        health.Initialize(this);
        health.onKilled.AddListener(OnKilled);

        instance = this;
    }
    #endregion

    #region Methods
    private void OnKilled ()
    {
        onKill.Invoke();
    }
    #endregion

    #region ITarget Implementation
    Vector3 ITarget.GetPosition() => transform.position;
    
    TargetType ITarget.TargetType => TargetType.Player;
    
    bool ITarget.IsAlive => health.IsAlive;

    public void Hit(int damage, ITarget attacker = null)
    {
        health.TakeDamage(damage);
    }

    public UnityEvent onHit;
    public UnityEvent onKill;
    public UnityEvent onRevive;

    UnityEvent ITarget.HitEvent => onHit;
    UnityEvent ITarget.KillEvent => onKill;
    UnityEvent ITarget.ReviveEvent => onRevive;
    #endregion
}

[System.Serializable]
public class PlayerHealth
{
    public TunnelingVignetteController controller;
    public VignetteParameters parameters = new VignetteParameters();

    [Min(1)]
    public int maxHealth = 100;
    public int currHealth = 100;
    public bool IsAlive => currHealth > 0;

    [Min(0)]
    [Tooltip("Seconds after taking damage before healing starts")]
    public float healDelay = 3f;
    [Min(1)]
    [Tooltip("Amount of health restored per second")]
    public float healRate = 10f;

    public UnityEvent onKilled;

    private Coroutine healCoroutine;
    private PlayerManager playerManager;

    public void Initialize (PlayerManager playerManager)
    {
        this.playerManager = playerManager;
        currHealth = maxHealth;

        if (controller != null)
            controller.BeginTunnelingVignette(new BloodiedSettings(parameters));
    }

    public void TakeDamage(int damage)
    {
        currHealth -= damage;
        if (currHealth <= 0)
        {
            currHealth = 0;
            onKilled.Invoke();

            if (healCoroutine != null)
                playerManager.StopCoroutine(healCoroutine);

            return;
        }

        playerManager.voice.PlayHit(damage);

        if (healCoroutine != null)
            playerManager.StopCoroutine(healCoroutine);

        healCoroutine = playerManager.StartCoroutine(HealOverTime());
    }

    private IEnumerator HealOverTime()
    {
        yield return new WaitForSeconds(healDelay);

        while (currHealth < maxHealth)
        {
            currHealth += (int)(healRate * Time.deltaTime);
            currHealth = Mathf.Min(currHealth, maxHealth);
            yield return null;
        }
    }

    [System.Serializable]
    public class BloodiedSettings : ITunnelingVignetteProvider
    {
        public VignetteParameters vignetteParameters { get => m_parameters; set => m_parameters = value; }

        [SerializeField]
        VignetteParameters m_parameters = new();

        public BloodiedSettings(VignetteParameters parameters)
        {
            m_parameters = parameters;
        }
    }
}

[System.Serializable]
public class PlayerVoice
{
    #region Properties
    public AudioSource voiceSource;
    public AudioSource breathingSource;

    public AudioClip[] smallHitClips;
    public AudioClip[] bigHitclips;

    [Min(0)]
    public int damageForSmallHurt = 5;
    [Min(10)]
    public int damageForBigHurt = 35;

    public UnityEvent onDialogueStart;
    public UnityEvent onDialogueEnd;

    private PlayerManager player;
    private Coroutine voiceClipEndCoroutine;

    private bool isPlayingDialogue = false;
    [Min(0)]
    private float currentVoicePriority = 0;
    #endregion

    #region Initialization
    public void Initialize(PlayerManager player)
    {
        this.player = player;
        if (damageForSmallHurt >= damageForBigHurt)
        {
            Debug.LogWarning("PlayerVoice: Little hurt damage must be less than big hurt damage.\n Readjusting values");
            damageForSmallHurt = 5;
        }
    }
    #endregion

    #region Public Methods
    public void PlayHit(float damage = 20)
    {
        if (damage < damageForSmallHurt)
            return; // Ignore damage sounds for small damage hits.
        else if (damage < damageForBigHurt)
            PlayVoiceClip(smallHitClips, priority: 10, dialogue: false);
        else
            PlayVoiceClip(bigHitclips, priority: 20, dialogue: false);
    }

    public void PlayDialogue(AudioClip clip)
    {
        PlayVoiceClip(clip, priority: 30, dialogue: true);
    }
    #endregion

    #region Private Methods
    protected void PlayVoiceClip(AudioClip[] clips, int priority = 1, bool dialogue = false)
    {
        PlayVoiceClip(clips[Random.Range(0, clips.Length)], priority: priority, dialogue: dialogue);
    }

    protected void PlayVoiceClip(AudioClip clip, int priority = 1, bool dialogue = false)
    {
        if (voiceSource.isPlaying && currentVoicePriority >= priority)
            return;

        if (voiceClipEndCoroutine != null)
        {
            player.StopCoroutine(voiceClipEndCoroutine);
            if (isPlayingDialogue && !dialogue)
            {
                isPlayingDialogue = false;
                onDialogueEnd?.Invoke();
            }
        }

        voiceClipEndCoroutine = player.StartCoroutine(VoiceClipEnd(clip.length));

        isPlayingDialogue = dialogue;
        voiceSource.clip = clip;
        currentVoicePriority = priority;
        voiceSource.Play();
        
        onDialogueStart?.Invoke();
    }

    protected void PlayBreathingClip(AudioClip clip)
    {
        if (breathingSource.isPlaying)
            return;

        breathingSource.clip = clip;
        breathingSource.Play();
    }

    protected IEnumerator VoiceClipEnd(float duration)
    {
        yield return new WaitForSeconds(duration);

        currentVoicePriority = 0;
        if (isPlayingDialogue)
        {
            isPlayingDialogue = false;
            onDialogueEnd?.Invoke();
        }
    }
    #endregion
}

#if UNITY_EDITOR
/// <summary>
/// Custom editor to add buttons in the inspector, for testing purposes.
/// </summary>
[CustomEditor(typeof(PlayerManager))]
public class PlayerEditor : Editor
{
    private int damageAmount = 20;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PlayerManager playerManager = (PlayerManager)target;

        damageAmount = EditorGUILayout.IntField("Damage Amount", damageAmount);

        if (GUILayout.Button("Damage"))
            playerManager.Hit(damageAmount);
    }
}
#endif