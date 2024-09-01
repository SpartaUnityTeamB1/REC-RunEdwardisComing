public class IntroScene : BaseScene
{
    protected override void Start()
    {
        base.Start();

        GameManager.Instance.CurrentScene = SceneIndex.Intro;

        GameManager.Instance.ShowUI<IntroSceneUI>(UI.Scene);
    }
}