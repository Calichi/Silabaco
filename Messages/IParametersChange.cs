namespace Silabaco.Messages;

public interface IParametersChange
{
    string CorrectSyllable { get; }
    IEnumerator<string> Syllables { get; }
}
