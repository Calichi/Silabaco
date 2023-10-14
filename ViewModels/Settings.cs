using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Silabaco.Messages;

namespace Silabaco.ViewModels;

public partial class Settings : ObservableRecipient, IParametersChange
{
    public Settings() : base(WeakReferenceMessenger.Default){
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    string consonant;

    [ObservableProperty]
    string correctSyllable;

    [RelayCommand(CanExecute = nameof(CanStartGame))]
    async Task StartGame() {
        GenerateSyllables();
        Messenger.Send(Message);
        await Shell.Current.GoToAsync("..");
    }

    bool CanStartGame() {
        return !string.IsNullOrWhiteSpace(Consonant);
    }

    public IEnumerator<string> Syllables { get; private set; }

    public IParametersChange Message => this;

    void GenerateSyllables() {
        var list = new List<string>();
        foreach(var _vowel in "aeiou") {
            list.Add($"{Consonant}{_vowel}");
        }
        Syllables = list.GetEnumerator();
    }
}
