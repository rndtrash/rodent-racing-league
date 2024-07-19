using Sandbox.Diagnostics;

namespace TTP.Paths;

[Category( "Paths" )]
public sealed class StraightLinePath : PathComponent
{
	public override IList<PathSample> Sample( int resolution )
	{
		Assert.True( resolution > 0 );
		Assert.IsValid( Next );

		var samples = new List<PathSample>( resolution );
		for ( var i = 0; i < resolution; i++ )
		{
			var fraction = (float)i / resolution;
			samples.Add( new PathSample(
				global::Transform.Lerp( Transform.World, Next.Transform.World, fraction, false ),
				MathX.Lerp( Width, Next.Width, fraction ) ) );
		}

		return samples;
	}
}
