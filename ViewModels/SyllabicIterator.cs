using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Silabaco.Messages;

namespace Silabaco.ViewModels;

public partial class SyllabicIterator : ObservableRecipient,IRecipient<IParametersChange>
{
    public SyllabicIterator() : base(WeakReferenceMessenger.Default) {
        timer = new Timer(TimerElapsed, null, Timeout.Infinite, 500);
        Messenger.RegisterAll(this);
    }

    //Funcionalidades publicas

    [ObservableProperty]
    string syllable;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(ShootCommand))]
    bool isPlaying = false;

    [RelayCommand]
    async Task Start() {
        score = 0;
        await NotifyStart();
        IsPlaying = true;
        StartTimer();
    }

    [RelayCommand(CanExecute = nameof(IsPlaying))]
    async Task Shoot() {
        StopTimer();
        await NotifyShotAction();
        if (IsPlaying) StartTimer();
    }

    [RelayCommand]
    async Task OpenSettings() {
        await Shell.Current.GoToAsync(nameof(Pages.Setting));
    }

    //Funcionalidades internas

    IEnumerator<string> syllables;
    string correctSyllable = "";
    int score = 0;

    Timer timer;


    string GetNextSyllable() {
        if (syllables.MoveNext()) return syllables.Current;

        syllables.Reset();
        return GetNextSyllable();
    }

    void StartTimer() => timer.Change(0, 1000);

    async void TimerElapsed(object state) {
        if(IsPlaying) await NotifySyllableChange();
    }

    void StopTimer() => timer.Change(Timeout.Infinite, Timeout.Infinite);
    
    bool IsSuccessfulShot() => syllables.Current == correctSyllable;

    async Task NotifyShotAction() {
        var shot = Messenger.Send(new Messages.Shoot(IsSuccessfulShot()));
        if (shot.Success) NotifyScoreIncrease();
        await Task.Delay(1000);
    }

    async Task NotifySyllableChange() {
        Messenger.Send(new Messages.SyllableChange(GetNextSyllable()));
        await Task.Delay(500);
    }

    void NotifyScoreIncrease() {
        var state = Messenger.Send(new Messages.ScoreIncrease(score++));
        if (state.Score == 4) NotifyWin();
    }

    void NotifyWin() {
        IsPlaying = false;
        Messenger.Send(new Messages.Win());
    }

    async Task NotifyStart() {
        Messenger.Send(new Messages.Start());
        await Task.Delay(300);
    }

    public void Receive(IParametersChange message) {
        correctSyllable = message.CorrectSyllable;
        syllables = message.Syllables;
    }
}
