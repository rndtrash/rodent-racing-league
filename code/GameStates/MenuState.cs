using RRL.UI;
using Sandbox;
using Sandbox.UI;
using System;

namespace RRL.GameStates;

public class MenuState : BaseState
{
    private MenuRootPanel _menuRootPanel = null;

    public override void StateInit()
    {
        base.StateInit();

        if (Game.IsClient)
            _menuRootPanel = new();
    }

    public override void StateDestroy()
    {
        base.StateDestroy();

        if (Game.IsClient)
        {
            _menuRootPanel.Delete(false);
            _menuRootPanel = null;
        }
    }

    public override void ClientDisconnect(IClient cl, NetworkDisconnectionReason reason)
    {
        Log.Error("TODO: Left the lobby");
    }

    public override void ClientJoined(IClient client)
    {
        Log.Error("TODO: Joined the lobby");
    }
}
