namespace Silabaco.Messages;

public class ShowButton
{
    public ShowButton(Function function) {
        RequiredFunction = function;
    }

    public Function RequiredFunction { get; }

    public enum Function {
        Play, Pause
    }
}
