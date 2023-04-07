using System;

namespace RRL.Hud;

public class RRLMenuAction : RRLMenuItem
{
	private Action action;

	public RRLMenuAction( string title, Action action ) : base( title )
	{
		this.action = action;
	}

	public void Activate() => action();
}
