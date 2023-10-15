using CommunityToolkit.Mvvm.Messaging;
using Silabaco.Messages;

using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Silabaco;

public partial class MainPage : ContentPage, IRecipient<Messages.Shoot>,
                                IRecipient<Messages.SyllableChange>,
                                IRecipient<Messages.ScoreIncrease>,
                                IRecipient<Messages.Win>,
                                IRecipient<Messages.Start>,
                                IRecipient<Messages.ShowButton>
{
    public MainPage()
    {
        InitializeComponent();
        WeakReferenceMessenger.Default.RegisterAll(this);
    }

    public async void Receive(Shoot message) {
        var from = backgroundButton.BackgroundColor;
        var to = message.Success ? Colors.SpringGreen : Colors.Red;

        await backgroundButton.BackgroundColorTo(to);
        await Task.Delay(500);
        await backgroundButton.BackgroundColorTo(from);
    }

    public async void Receive(SyllableChange message) {
        await syllableView.FadeTo(0);
        await MainThread.InvokeOnMainThreadAsync(() => {
            syllableView.Text = message.Syllable;
        });
        await syllableView.FadeTo(1);
    }

    public async void Receive(ScoreIncrease message) {
        var view = (Image)scoreView.Children[message.Score];
        await view.FadeTo(1);
    }

    public async void Receive(Win message) {
        await syllableView.FadeTo(0);

        await Task.WhenAll(awardView.FadeTo(1,500),
                           awardView.ScaleTo(1,500));
    }

    public async void Receive(Start message) {
        await Task.WhenAll(
            awardView.FadeTo(0),
            awardView.ScaleTo(0.5),
            HideEgg(0),
            HideEgg(1),
            HideEgg(2),
            HideEgg(3),
            HideEgg(4)
        );
    }

    public async void Receive(ShowButton message) {
        var frontButton = playButton;
        var backButton = pauseButton;

        if(message.RequiredFunction == ShowButton.Function.Pause) {
            frontButton = pauseButton;
            backButton = playButton;
        }

        await MainThread.InvokeOnMainThreadAsync(() => {
            backButton.ZIndex = 0;
            frontButton.ZIndex = 1;
        });

        await Task.WhenAll(
                backButton.FadeTo(0),
                backButton.ScaleTo(0.5)
              );

        await Task.WhenAll(
                frontButton.FadeTo(1),
                frontButton.ScaleTo(1)
              );
    }

    Task HideEgg(int index) {
        var egg = (Image)scoreView.Children[index];
        return egg.FadeTo(0);
    }
}