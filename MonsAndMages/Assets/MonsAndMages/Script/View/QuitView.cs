using UnityEngine;
using UnityEngine.UI;

public class QuitView : MonoBehaviour
{
    [SerializeField] private Button m_btnYse;
    [SerializeField] private Button m_btnNo;

    private void OnEnable()
    {
        GameEvent.onEndView += OnEndView;
    }

    private void OnDisable()
    {
        GameEvent.onEndView -= OnEndView;
    }

    private void Start()
    {
        m_btnYse.onClick.AddListener(BtnYes);
        m_btnNo.onClick.AddListener(BtnNo);

        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);
    }

    //

    public void BtnYes()
    {
        GameManager.instance.GameEnd();
        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);
    }

    public void BtnNo()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(false);
    }

    //

    private void OnEndView()
    {
        for (int i = 0; i < this.transform.childCount; i++)
            this.transform.GetChild(i).gameObject.SetActive(true);
    }
}