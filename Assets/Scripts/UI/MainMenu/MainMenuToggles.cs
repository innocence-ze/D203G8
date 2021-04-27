using DG.Tweening;
using UnityEngine.UI;

public class MainMenuToggles : ToggleGroup
{
    public Image selectImg;
    public float imgShowTime = 0.3f;
    public float imgHideTime = 0.3f;

    public void OnEnterToggle(int id)
    {
        selectImg.transform.position = m_Toggles[id].transform.position;
        m_Toggles[id].isOn = true;
        selectImg.DOFade(1, imgShowTime);
    }

    public void OnExitToggle(int id)
    {
        m_Toggles[id].isOn = false;
        selectImg.DOFade(0, imgHideTime);
    }
}
