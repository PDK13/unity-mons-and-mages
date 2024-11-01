using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioClip m_soundTurnHint;

    [Space]
    [SerializeField] private AudioClip m_soundButtonPress;

    [Space]
    [SerializeField] private AudioClip m_soundCardEffect;

    [SerializeField] private AudioClip m_soundCardOrigin;
    [SerializeField] private AudioClip m_soundCardClass;
    [SerializeField] private AudioClip m_soundCardHard;
    [SerializeField] private AudioClip m_soundCardLight;
    [SerializeField] private AudioClip m_soundCardChoice;

    private Dictionary<string, GameObject> m_queue = new Dictionary<string, GameObject>();

    [Space]
    [SerializeField] private List<AudioClip> m_ignore = new List<AudioClip>();

    private void Awake()
    {
        SoundManager.instance = this;
    }

    public float Play(SoundType Type)
    {
        AudioClip Sound = null;
        switch (Type)
        {
            case SoundType.TurnHint:
                Sound = m_soundTurnHint;
                break;
            //
            case SoundType.ButtonPress:
                Sound = m_soundButtonPress;
                break;
            //
            case SoundType.CardEffect:
                Sound = m_soundCardEffect;
                break;

            case SoundType.CardOrigin:
                Sound = m_soundCardOrigin;
                break;

            case SoundType.CardClass:
                Sound = m_soundCardClass;
                break;

            case SoundType.CardHard:
                Sound = m_soundCardHard;
                break;
            case SoundType.CardLight:
                Sound = m_soundCardLight;
                break;

            case SoundType.CardChoice:
                Sound = m_soundCardChoice;
                break;
        }

        if (m_queue.ContainsKey(Sound.name))
            if (m_queue[Sound.name] != null)
                return Sound.length;

        var Object = new GameObject(Sound.name);
        var Source = Object.AddComponent<AudioSource>();
        Source.clip = Sound;
        Source.loop = false;
        Source.Play();
        Destroy(Object, Sound.length);

        if (!m_ignore.Exists(t => t.Equals(Sound)))
        {
            if (!m_queue.ContainsKey(Sound.name))
                m_queue.Add(Sound.name, Object);
            else
                m_queue[Sound.name] = Object;
        }

        return Sound.length;
    }
}