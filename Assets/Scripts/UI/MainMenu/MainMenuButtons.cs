using DG.Tweening;
using UnityEngine.UI;
using UnityEngine;
public class MainMenuButtons : MonoBehaviour
{
    public Image selectImg;
    public float imgShowTime = 0.3f;
    public float imgHideTime = 0.3f;

    public void OnEnterButton(int id)
    {
        selectImg.transform.position = transform.GetChild(id).position;
        selectImg.DOFade(1, imgShowTime);
    }

    public void OnExitButton()
    {
        selectImg.DOFade(0, imgHideTime);
    }
}
