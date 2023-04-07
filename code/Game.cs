
using Sandbox;

namespace RRL;

/// <summary>
/// This is your game class. This is an entity that is created serverside when
/// the game starts, and is replicated to the client. 
/// 
/// You can use this to create things like HUDs and declare which player class
/// to use for spawned players.
/// </summary>
public partial class Game : Sandbox.Game
{
	public bool InGame { get; internal set; }

	public Game()
	{
		InGame = Global.MapName != Config.MenuMapName;

		if ( IsServer )
		{
			_ = new Hud.HudEntity();
		}
	}

	/// <summary>
	/// A client has joined the server. Make them a pawn to play with
	/// </summary>
	public override void ClientJoined( Client client )
	{
		base.ClientJoined( client );

		if ( !InGame )
			return;

		var player = new Player();
		client.Pawn = player;

		player.Respawn();
	}

	public override bool CanHearPlayerVoice( Client source, Client dest ) => false; // no
}
