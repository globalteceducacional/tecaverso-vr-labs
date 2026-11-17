using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // Método chamado no botão para carregar a cena pelo nome
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Método para sair do aplicativo (funciona no build)
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Jogo fechado! (Funciona apenas no build)");
    }
}
