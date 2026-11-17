using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class WristUI : MonoBehaviour
{
    public InputActionAsset inputActions;

    private Canvas _wristUICanvas;
    private InputAction _menu;

    private void Start()
    {
        _wristUICanvas = GetComponent<Canvas>();
        _menu = inputActions.FindAction("Menu");
        _menu.Enable();
        _menu.performed += ToggleMenu;
    }

    private void OnDestroy()
    {
        _menu.performed -= ToggleMenu;
    }

    public void ToggleMenu(InputAction.CallbackContext context)
    {
        _wristUICanvas.enabled = !_wristUICanvas.enabled;
    }

    public void Resume()
    {
        _wristUICanvas.enabled = !_wristUICanvas.enabled;
    }

    // Reinicia a cena atual
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Vai para a cena "MainMenu" (precisa estar adicionada no Build Settings)
    public void GotoMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // Fecha o jogo (no editor do Unity não vai funcionar, só no build)
    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        // Para também no editor (somente durante testes)
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
