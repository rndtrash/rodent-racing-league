using Sandbox;

namespace RRL.GameStates;

public abstract class BaseState : BaseNetworkable
{
    public virtual void StateInit()
    {
        Event.Register(this);
    }

    public virtual void StateDestroy()
    {
        Event.Unregister(this);
    }

    public abstract void ClientJoined(IClient client);
    public abstract void ClientDisconnect(IClient cl, NetworkDisconnectionReason reason);
}
