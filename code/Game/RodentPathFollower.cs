using System;
using TTP.Paths;

namespace Sandbox;

public sealed class RodentPathFollower : Component
{
	[Property] public RodentPathNode First { get; set; }
	[Property] public float Speed { get; set; } = 100;

	[Category("Debug")]
	[ReadOnly]
	[Property]
	private RodentPathNode Current { get; set; }
	
	[Category("Debug")]
	[ReadOnly]
	[Property]
	private int CurrentSampleIndex { get; set; }

	/// <summary>
	/// A fraction between the current sample and the next one
	/// </summary>
	[Category("Debug")]
	[ReadOnly]
	[Property]
	private float CurrentSampleFraction { get; set; }

	protected override void OnStart()
	{
		Current = First;
		CurrentSampleIndex = 0;
		CurrentSampleFraction = 0;
		Transform.World = Current.Samples[CurrentSampleIndex].Transform;
	}

	protected override void OnUpdate()
	{
		var sample = Current.Samples[CurrentSampleIndex];
		var left = sample.Transform.Left * sample.Width * 0.5f;
		DebugOverlay.Line( sample.Transform.Position + left, sample.Transform.Position - left, Color.Cyan );
	}

	protected override void OnFixedUpdate()
	{
		if ( !Game.IsPlaying )
			return;
		
		Advance( Time.Delta * Speed );
	}

	public void Advance( float distance )
	{
		if ( !Current.IsValid() || !Current.Next.IsValid() )
			return;

		var isForward = true;
		if ( distance < 0 )
		{
			isForward = false;
			distance = Math.Abs( distance );
		}

		float currentDistance = 0;

		// First step: travel through the current sample
		var current = Current.Samples[CurrentSampleIndex];
		var next = GetNextSample();
		var sampleDistance = current.Transform.Position.Distance( next.Transform.Position );
		var sampleBorderDistance = sampleDistance * (isForward ? 1 - CurrentSampleFraction : CurrentSampleFraction);
		if ( distance <= sampleBorderDistance )
		{
			if ( isForward )
				CurrentSampleFraction += distance / sampleDistance;
			else
				CurrentSampleFraction -= distance / sampleDistance;
			
			currentDistance = distance;
		}
		else
		{
			currentDistance += sampleBorderDistance;

			if ( isForward )
			{
				IncrementSampleIndex();
			}
			else
			{
				DecrementSampleIndex();
			}
		}

		// Second step: if it wasn't enough, travel through all the samples
		while ( currentDistance < distance )
		{
			current = Current.Samples[CurrentSampleIndex];
			next = GetNextSample();

			sampleDistance = current.Transform.Position.Distance( next.Transform.Position );
			var distanceLeft = distance - currentDistance;
			if ( sampleDistance >= distanceLeft )
			{
				CurrentSampleFraction = distanceLeft / sampleDistance;
				if ( !isForward )
				{
					CurrentSampleFraction = 1 - CurrentSampleFraction;
				}

				currentDistance = distance;
			}
			else
			{
				if ( isForward )
				{
					IncrementSampleIndex();
				}
				else
				{
					DecrementSampleIndex();
				}

				currentDistance += sampleDistance;
			}
		}

		// Third step: set the position 
		Transform.World = global::Transform.Lerp( current.Transform, next.Transform,
			CurrentSampleFraction, true );
		Transform.ClearInterpolation();
	}

	private PathComponent.PathSample GetNextSample() =>
		CurrentSampleIndex + 1 < Current.Samples.Length
			? Current.Samples[CurrentSampleIndex + 1]
			: Current.Next.Samples[0];

	private PathComponent.PathSample GetPreviousSample() =>
		CurrentSampleIndex - 1 >= 0
			? Current.Samples[CurrentSampleIndex - 1]
			: Current.Previous.Samples[^1];

	private void IncrementSampleIndex()
	{
		CurrentSampleIndex++;
		if ( CurrentSampleIndex >= Current.Samples.Length )
		{
			Current = Current.Next;
			CurrentSampleIndex = 0;
		}
	}

	private void DecrementSampleIndex()
	{
		CurrentSampleIndex--;
		if ( CurrentSampleIndex < 0 )
		{
			Current = Current.Previous;
			CurrentSampleIndex = Current.Samples.Length - 1;
		}
	}
}
