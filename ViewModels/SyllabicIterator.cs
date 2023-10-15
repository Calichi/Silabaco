using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Silabaco.Messages;

namespace Silabaco.ViewModels;

public partial class SyllabicIterator : ObservableRecipient, IRecipient<IParametersChange>
{
    public SyllabicIterator() : base(WeakReferenceMessenger.Default) {
        timer = new Timer(TimerElapsed, null, Timeout.Infinite, 500);
        Messenger.RegisterAll(this);
        TextToSpeech.Default.SpeakAsync("¡Bienvenido!");
    }

    //Funcionalidades publicas

    [ObservableProperty]
    string syllable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ShootCommand))]
    [NotifyCanExecuteChangedFor(nameof(StartGameCommand))]
    [NotifyCanExecuteChangedFor(nameof(PauseGameCommand))]
    [NotifyCanExecuteChangedFor(nameof(OpenSettingsCommand))]
    bool isPlaying = false;

    [RelayCommand(CanExecute = nameof(CanStartGame))]
    async Task StartGame() {
        if (resetRequired) await Reset();
        IsPlaying = true;
        StartTimer();
        Messenger.Send(new Messages.ShowButton(ShowButton.Function.Pause));
    }

    [RelayCommand(CanExecute = nameof(IsPlaying))]
    void PauseGame() {
        IsPlaying = false;
        StopTimer();
        Messenger.Send(new Messages.ShowButton(ShowButton.Function.Play));
    }

    bool CanStartGame() {
        return syllables != null &&
               !string.IsNullOrWhiteSpace(correctSyllable) &&
               !IsPlaying;
    }

    [RelayCommand(CanExecute = nameof(IsPlaying))]
    async Task Shoot() {
        StopTimer();
        await NotifyShotAction();
        if (IsPlaying) StartTimer();
    }

    [RelayCommand(CanExecute = nameof(CanOpenSettings))]
    async Task OpenSettings() {
        await Shell.Current.GoToAsync(nameof(Pages.Setting));
    }

    bool CanOpenSettings() => !IsPlaying;

    //Funcionalidades internas

    string[] syllables;
    string correctSyllable = "";
    string currentSyllable = "";
    int score = 0;
    Timer timer;
    bool resetRequired = false;

    void UpdateCurrentSyllable() {
        var index = Random.Shared.Next(5);
        currentSyllable = syllables[index];
    }

    void StartTimer() => timer.Change(0, 1000);

    async void TimerElapsed(object state) {
        if (IsPlaying) {
            UpdateCurrentSyllable();
            await NotifySyllableChange();
        }
    }

    void StopTimer() => timer.Change(Timeout.Infinite, Timeout.Infinite);

    bool IsSuccessfulShot() => currentSyllable == correctSyllable;

    async Task NotifyShotAction() {
        var shot = Messenger.Send(new Messages.Shoot(IsSuccessfulShot()));
        if (shot.Success) NotifyScoreIncrease();
        await Task.Delay(1000);
    }

    async Task NotifySyllableChange() {
        Messenger.Send(new Messages.SyllableChange(currentSyllable));
        await Task.Delay(500);
    }

    async void NotifyScoreIncrease() {
        await TextToSpeech.Default.SpeakAsync(correctSyllable);
        var state = Messenger.Send(new Messages.ScoreIncrease(score++));
        if (state.Score == 4) NotifyWin();
    }

    void NotifyWin() {
        PauseGameCommand.Execute(null);
        Messenger.Send(new Messages.Win());
        resetRequired = true;
    }

    async Task RequestGUIReset() {
        Messenger.Send(new Messages.Start());
        await Task.Delay(300);
    }

    public void Receive(IParametersChange message) {
        resetRequired = true;
        correctSyllable = message.CorrectSyllable;
        syllables = message.Syllables;
        StartGameCommand.NotifyCanExecuteChanged();
    }

    async Task Reset() {
        score = 0;
        await RequestGUIReset();
        resetRequired = false;
    }
}
