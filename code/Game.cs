using RRL.GameStates;
using Sandbox;

namespace RRL;

public partial class RRLGame : GameManager
{
    public static RRLGame Instance { get; private set; }
    public static BaseState CurrentState
    {
        get
        {
            return _state;
        }
        private set
        {
            _state?.StateDestroy();
            _state = value;
            _state?.StateInit();
        }
    }

    private static BaseState _state;

    public RRLGame()
    {
        Instance = this;

        if (Game.Server.MapIdent != "")
        {
            //
        }
        else
        {
            CurrentState = new MenuState();
        }
    }

    public override void ClientJoined(IClient client)
    {
        base.ClientJoined(client);

        CurrentState?.ClientJoined(client);
    }

    public override void ClientDisconnect(IClient cl, NetworkDisconnectionReason reason)
    {
        base.ClientDisconnect(cl, reason);

        CurrentState?.ClientDisconnect(cl, reason);
    }
}
