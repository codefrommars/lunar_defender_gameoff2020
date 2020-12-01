
public class FinalRoom : StageScript
{
    protected override ScriptAction DoBuildFor(Stage stage)
    {
        ScriptActionSequence script = new ScriptActionSequence(false,
            new ShowEndGameMessageAction()
        );
        return script;
    }
}
