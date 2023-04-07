using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;

namespace RRL.Hud;

/// <summary>
/// This is the HUD entity. It creates a RootPanel clientside, which can be accessed
/// via RootPanel on this entity, or Local.Hud.
/// </summary>
public partial class HudEntity : HudEntity<RootPanel>
{
	public readonly struct MenuStackItem
	{
		public readonly int Selection;
		public readonly RRLMenu Current;
	}

	public static readonly RRLMenu MenuRoot = new(
			"Rodent Racing League",
			new RRLMenuItem[] {
				new RRLMenuAction(
						"New Game",
						() => Log.Info("rodent input devide")
					),
				new RRLMenuAction(
						"Demo",
						() => Log.Info("hamster mouse")
					)
			},
			true
		);

	[Net, Change( nameof( NewMenuLevel ) )] public IList<int> Previous { get; protected set; } = new List<int>();
	[Net, Change( nameof( NewSelection ) )] public int Selection { get; protected set; } = 0;

	private RRLMenu currentMenu = MenuRoot;

	public HudEntity() : base()
	{
		if ( IsClient )
		{
			RootPanel.StyleSheet.Load( "/Hud/Hud.scss" );
			RootPanel.AddChild<MenuContainer>();
		}
	}

	[Event( "rrl.menu.init" )]
	public void OnMenuInit()
	{
		Log.Info( "menu init moment" );
		NewMenuLevel( Previous, Previous );
		NewSelection( 0, 0 );
	}

	protected void NewSelection( int oldSelection, int newSelection )
	{
		// TODO:
	}

	protected void NewMenuLevel( IList<int> oldS, IList<int> newS )
	{
		// TODO: play the fade in animation if oldS.Count <= newS.Count, fade out otherwise

		currentMenu = MenuRoot;
		try
		{
			while ( newS.Count > 0 )
			{
				if ( currentMenu.Items[newS[0]] is not RRLMenu menu )
					throw new ArgumentOutOfRangeException( "currentMenu.Items[newS.First.Value]" );

				currentMenu = menu;
				newS.RemoveAt( 0 );
			}
		}
		catch ( Exception e )
		{
			Log.Error( $"Invalid menu, kicking the player ({e.Message})" );
			Local.Client.Kick();
			return;
		}

		MenuContainer.The.RenderMenu( currentMenu );
	}
}
