using TTP.Paths;

namespace Sandbox;

public sealed class RodentPathNode : Component
{
	[RequireComponent] public PathComponent Node { get; set; }

	[Property] public float Height { get; set; } = 100;
	[Range( 1, 500 )] [Property] public int Resolution { get; set; } = 100;

	public RodentPathNode Next { get; private set; }
	public RodentPathNode Previous { get; private set; }
	public PathComponent.PathSample[] Samples { get; private set; }

	protected override void OnAwake()
	{
		Next = Node.Next.Components.Get<RodentPathNode>();
		Previous = Node.Previous.Components.Get<RodentPathNode>();
		Samples = Node.Sample( Resolution ).ToArray();
	}

	protected override void OnStart()
	{
		if ( Game.IsPlaying && Node.IsValid() )
		{
			Node.Destroy();
			Node = null;
		}
	}
}
