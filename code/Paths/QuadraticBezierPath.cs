using Sandbox.Diagnostics;

namespace TTP.Paths;

[Category( "Paths" )]
public sealed class QuadraticBezierPath : PathComponent
{
	[Property] public GameObject ControlPoint { get; set; }

	public override IList<PathSample> Sample( int resolution )
	{
		Assert.True( resolution > 0 );
		Assert.IsValid( Next );
		Assert.IsValid( ControlPoint );

		var samples = new List<PathSample>( resolution );
		for ( var i = 0; i < resolution; i++ )
		{
			var fraction = (float)i / resolution;
			// var first = global::Transform.Lerp( Transform.World, ControlPoint.Transform.World, fraction, false );
			// var second = global::Transform.Lerp( ControlPoint.Transform.World, Next.Transform.World, fraction, false );
			// var final = global::Transform.Lerp( first, second, fraction, false );
			// samples.Add( new PathSample( final, MathX.Lerp( Width, Next.Width, fraction ) ) );
			var first = Vector3.Lerp( Transform.World.Position, ControlPoint.Transform.World.Position, fraction,
				false );
			var second = Vector3.Lerp( ControlPoint.Transform.World.Position, Next.Transform.World.Position, fraction,
				false );
			var final = new Transform( Vector3.Lerp( first, second, fraction, false ),
				Rotation.Slerp( Transform.World.Rotation, Next.Transform.World.Rotation, fraction ) );
			samples.Add( new PathSample( final, MathX.Lerp( Width, Next.Width, fraction ) ) );
		}

		return samples;
	}
}
