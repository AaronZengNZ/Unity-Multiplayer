using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class NameSelector : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_Text buttonText;
    [SerializeField] private Button connectButton;
    [SerializeField] private int minNameLength = 1;
    [SerializeField] private int maxNameLength = 16;
    public const string PlayerNameKey = "PlayerName";
    private void Start()
    {
        if(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null){
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            return;
        }
        nameField.text = PlayerPrefs.GetString(PlayerNameKey, string.Empty);
        HandleNameChanged();
    }

    public void HandleNameChanged(){
        connectButton.interactable = 
            nameField.text.Length >= minNameLength &&
            nameField.text.Length <= maxNameLength;
        if(nameField.text.Length < minNameLength){
            buttonText.text = "Enter a name";
        }
        else if(nameField.text.Length > maxNameLength){
            buttonText.text = "Name too long";
        }
        else{
            buttonText.text = "Connect";
        }
    }

    public void Connect(){
        PlayerPrefs.SetString(PlayerNameKey, nameField.text);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
