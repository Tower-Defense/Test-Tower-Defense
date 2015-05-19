using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    public Text infoText;

    public void OpenInfoPanel(string text_, Color color_)
    {
        infoText.color = color_;
        infoText.text = text_;
        gameObject.SetActive(true);
    }

    public void CloseInfoPanel()
    {
        if (PhotonNetwork.room != null)
            PhotonNetwork.LeaveRoom();
        gameObject.SetActive(false);
    }

}
