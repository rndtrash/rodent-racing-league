using Sandbox.Diagnostics;

namespace TTP.Paths;

[Category( "Paths" )]
public sealed class PathTerminator : PathComponent
{
	public override IList<PathSample> Sample( int resolution )
	{
		Assert.True( resolution > 0 );
		
		return new List<PathSample> { new(Transform.World, Width) };
	}
}
