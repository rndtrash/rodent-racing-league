namespace TTP.Paths;

[Category( "Paths" )]
public abstract class PathComponent : Component
{
	[Property] public PathComponent Previous { get; set; }
	[Property] public PathComponent Next { get; set; }
	[Property] public float Width { get; set; }
	[Property] [Range( 1, 100 )] public int Resolution { get; set; } = 10;

	protected override void OnDestroy()
	{
		if ( Previous.IsValid() && Next.IsValid() )
		{
			Previous.Next = Next;
			Next.Previous = Previous;
		}
	}

	protected override void OnUpdate()
	{
		if ( !Next.IsValid() && this is not PathTerminator )
		{
			var terminator = Components.Create<PathTerminator>();
			terminator.Previous = Previous;
			terminator.Width = Width;
			Destroy();
			return;
		}
	}

	protected override void DrawGizmos()
	{
		if ( Scene.IsEditor && !Game.IsPlaying )
		{
			var color = Gizmo.IsSelected ? Color.Yellow : Color.Green;

			if ( !Previous.IsValid() || !Next.IsValid() )
			{
				DebugOverlay.Sphere( new Sphere( Transform.Position, 10 ), color: color );
			}

			if ( Width > 0 )
			{
				var left = Transform.Rotation.Left * Width * 0.5f;
				DebugOverlay.Line( Transform.Position + left, Transform.Position - left, color );
			}

			if ( Next.IsValid() )
			{
				DebugOverlay.Line( Transform.Position, Next.Transform.Position );

				var samples = Sample();
				if ( samples.Count == 0 )
					return;

				if ( Width > 0 )
				{
					for ( var i = 1; i < samples.Count; i++ )
					{
						var prevSample = samples[i - 1];
						var sample = samples[i];
						var prevLeft = prevSample.Transform.Left * prevSample.Width * 0.5f;
						var left = sample.Transform.Left * sample.Width * 0.5f;
						DebugOverlay.Line( prevSample.Transform.Position + prevLeft, sample.Transform.Position + left,
							color );
						DebugOverlay.Line( prevSample.Transform.Position - prevLeft, sample.Transform.Position - left,
							color );
						DebugOverlay.Line( sample.Transform.Position + left,
							sample.Transform.Position - left, color );
					}

					var lastSample = samples[^1];
					var lastLeft = lastSample.Transform.Left * lastSample.Width * 0.5f;
					var nextLeft = Next.Transform.World.Left * Next.Width * 0.5f;
					DebugOverlay.Line( lastSample.Transform.Position + lastLeft,
						Next.Transform.World.Position + nextLeft,
						color );
					DebugOverlay.Line( lastSample.Transform.Position - lastLeft,
						Next.Transform.World.Position - nextLeft,
						color );
				}
				else
				{
					foreach ( var sample in samples )
					{
						DebugOverlay.Sphere( new Sphere( sample.Transform.Position, 2.5f ), color: color );
						DebugOverlay.Line( sample.Transform.Position,
							sample.Transform.Position + sample.Transform.Forward * 10, Color.White );
					}
				}
			}
		}
	}

	public record PathSample( Transform Transform, float Width );

	/// <summary>
	/// Split the path node into samples.
	/// </summary>
	/// <param name="resolution">Amount of sample points, > 0</param>
	/// <returns>A list of path samples with `resolution` items</returns>
	public abstract IList<PathSample> Sample( int resolution );

	public IList<PathSample> Sample() => Sample( Resolution );
}
