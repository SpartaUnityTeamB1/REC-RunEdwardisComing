using UnityEngine.SceneManagement;

public class GamesceneManager
{
    public BaseScene CurrentScene { get; private set; }

    public void SceneChange(BaseScene scene)
    {
        CurrentScene = scene;
    }

    public void GoToNextScene()
    {
        GameManager.Instance.Clear();
        SceneManager.LoadScene((int)CurrentScene.nextSceneIndex);

    }

    public void GoToPrevScene()
    {
        if (SceneIndex.SCENEEND != CurrentScene.prevSceneIndex)
        {
            GameManager.Instance.Clear();
            SceneManager.LoadScene((int)CurrentScene.prevSceneIndex);
        }
    }
}