using System;
using System.Collections.Generic;

namespace RRL.Hud;

public class RRLMenu : RRLMenuItem
{
	public IList<RRLMenuItem> Items { get; protected set; }

	public int Level { get; protected set; } = 0;

	public RRLMenu( string title, RRLMenuItem[] children, bool root = false ) : base( title )
	{
		if ( children.Length == 0 )
			throw new ArgumentException( "This menu is empty" );

		Items = children;

		if ( root )
		{
			SetLevel( 0 );
		}
	}

	protected void SetLevel( int level )
	{
		Level = level;
		foreach ( var item in Items )
		{
			if ( item is RRLMenu m )
				m.SetLevel( Level + 1 );
		}
	}
}
