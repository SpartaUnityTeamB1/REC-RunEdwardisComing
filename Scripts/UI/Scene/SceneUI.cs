public class SceneUI : BaseUI
{
    public void OnClickNextScene()
    {
        GameManager.Instance.GoToNextScene();
    }
}