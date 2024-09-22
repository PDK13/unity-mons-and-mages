using TMPro;
using UnityEngine;

public class PlayerViewButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_tmpIndex;
    [SerializeField] private TextMeshProUGUI m_tmpHealth;
    [SerializeField] private TextMeshProUGUI m_tmpStun;

    public bool Base;

    public int Health { set => m_tmpHealth.text = value.ToString(); }

    public int Stun { set => m_tmpStun.text = value.ToString(); }
}