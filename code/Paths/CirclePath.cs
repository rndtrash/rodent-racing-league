using Sandbox.Diagnostics;

namespace TTP.Paths;

[Category( "Paths" )]
public sealed class CirclePath : PathComponent
{
	[Property] public GameObject ControlPoint { get; set; }
	
	public override IList<PathSample> Sample( int resolution )
	{
		// TODO:
		Assert.True( resolution > 0 );
		Assert.IsValid( Next );
		Assert.IsValid( ControlPoint );

		var samples = new List<PathSample>( resolution );
		for ( var i = 0; i < resolution; i++ )
		{
			var fraction = (float)i / resolution;
			var first = global::Transform.Lerp( Transform.World, ControlPoint.Transform.World, fraction, false );
			var second = global::Transform.Lerp( ControlPoint.Transform.World, Next.Transform.World, fraction, false );
			var final = global::Transform.Lerp( first, second, fraction, false );
			samples.Add( new PathSample( final, MathX.Lerp( Width, Next.Width, fraction ) ) );
		}

		return samples;
	}
}
