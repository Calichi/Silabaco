namespace Silabaco.Messages;

public class SyllableChange
{
    public SyllableChange(string syllable) {
        Syllable = syllable;
    }

    public string Syllable { get; set; }
}
