using Sandbox;
using Sandbox.UI;

namespace RRL.Hud;

[Library( "menucontainer" )]
public class MenuContainer : Panel
{
	public static MenuContainer The { get; internal set; }

	public MenuContainer() : base()
	{
		The = this;

		Event.Run( "rrl.menu.init" );
	}

	public void RenderMenu( RRLMenu menu )
	{
		AddChild<Label>().Text = menu.Title;

		foreach ( var c in Children )
		{
			c.Delete();
		}

		foreach ( var item in menu.Items )
		{
			Log.Info( "penis" );
			var p = Add.Panel( item.Enabled() ? "" : "disabled" );
			var l = p.AddChild<Label>();
			l.Text = item.Title;
		}
	}
}
