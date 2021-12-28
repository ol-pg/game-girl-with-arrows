using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField] private InputField nameField;

    private void Start()
    {
        if (PlayerPrefs.HasKey("Player_Name"))
            nameField.text = PlayerPrefs.GetString("Player_Name");
    }
    public void OnEndEditName()
    {
        PlayerPrefs.SetString("Player_Name", nameField.text);
    }

   public void OnClickPlay()
   {
        SceneManager.LoadScene(1);
   }
    public void OnClickExit()
    {
        Application.Quit();
    }
}
