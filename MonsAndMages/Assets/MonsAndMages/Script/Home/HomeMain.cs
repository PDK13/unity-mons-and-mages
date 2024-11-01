using UnityEngine;
using UnityEngine.UI;

public class HomeMain : MonoBehaviour
{
    [SerializeField] private Button m_btnTutorial;
    [SerializeField] private Button m_btnFreePlay;

    private void Awake()
    {
    }

    private void OnEnable()
    {
        GameEvent.onStart += OnStart;
        GameEvent.onEnd += OnEnd;
    }

    private void OnDisable()
    {
        GameEvent.onStart -= OnStart;
        GameEvent.onEnd -= OnEnd;
    }

    private void Start()
    {
        m_btnTutorial.onClick.AddListener(BtnTutorial);
        m_btnFreePlay.onClick.AddListener(BtnFreePlay);

        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(true);
    }

    //

    public void BtnTutorial()
    {
        GameManager.instance.GameTutorial();
    }

    public void BtnFreePlay()
    {
        GameManager.instance.GameStart();
    }

    //

    private void OnStart()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);
    }

    private void OnEnd()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(true);
    }
}