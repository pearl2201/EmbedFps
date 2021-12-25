using System;
using DarkRift;
using DarkRift.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject loginWindow;
    [SerializeField]
    private InputField nameInput;
    [SerializeField]
    private Button submitLoginButton;

    void Start()
    {
        ConnectionManager.Instance.OnConnected += StartLoginProcess;
        ConnectionManager.Instance.Client.MessageReceived += OnMessage;
        loginWindow.SetActive(false);
        submitLoginButton.onClick.AddListener(OnSubmitLogin);
    }

    public void StartLoginProcess()
    {
        loginWindow.SetActive(true);
    }

    private void OnMessage(object sender, MessageReceivedEventArgs e)
    {
        using (Message message = e.GetMessage())
        {
            switch ((Tags)message.Tag)
            {
                case Tags.LoginRequestDenied:
                    OnLoginDecline();
                    break;
                case Tags.LoginRequestAccepted:
                    OnLoginAccept(message.Deserialize<LoginInfoData>());
                    break;
            }
        }
    }

    private void OnLoginDecline()
    {
        loginWindow.SetActive(true);
    }

    public void OnLoginAccept(LoginInfoData data)
    {
        ConnectionManager.Instance.PlayerId = data.Id;
        ConnectionManager.Instance.LobbyInfoData = data.Data;
        SceneManager.LoadScene("Lobby");
    }

    void OnDestroy()
    {
        ConnectionManager.Instance.OnConnected -= StartLoginProcess;
        ConnectionManager.Instance.Client.MessageReceived -= OnMessage;
    }

    public void OnSubmitLogin()
    {
        if (!String.IsNullOrEmpty(nameInput.text))
        {
            loginWindow.SetActive(false);

            using (Message message = Message.Create((ushort)Tags.LoginRequest, new LoginRequestData(nameInput.text)))
            {
                ConnectionManager.Instance.Client.SendMessage(message, SendMode.Reliable);
            }
        }
    }
}