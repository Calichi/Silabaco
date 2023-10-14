using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Silabaco.Messages;

public class Shoot
{
    public Shoot(bool success) {
        Success = success;
    }

    public bool Success { get; set; }
}
