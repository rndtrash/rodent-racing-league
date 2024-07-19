using System;
using System.Collections.Generic;
using Editor;
using Sandbox.Diagnostics;
using TTP.Paths;

namespace Sandbox;

public sealed class PathEditorContext
{
	public PathComponent SelectedNode { get; set; }
}

public sealed class PathEditorWidgetWindow : WidgetWindow
{
	public delegate PathComponent AddPathComponent( GameObject nodeObject, GameObject nextObject );

	public record PathNodeType( string Name, string Icon, AddPathComponent Make );

	// TODO: make it static readonly later because the static variables are hotloaded poorly 
	private List<PathNodeType> PathNodeTypes = new()
	{
		new PathNodeType( "Straight", "timeline", ( nodeObject, nextObject ) =>
		{
			var path = nodeObject.Components.Create<StraightLinePath>();

			return path;
		} ),
		new PathNodeType( "Quadratic Bézier curve", "line_curve", ( nodeObject, nextObject ) =>
		{
			var path = nodeObject.Components.Create<QuadraticBezierPath>();
			var controlPointObject = SceneEditorSession.Active.Scene.CreateObject();
			controlPointObject.Name = "Control Point";
			controlPointObject.Transform.World =
				Transform.Lerp( nodeObject.Transform.World, nextObject.Transform.World, 0.5f, true );
			controlPointObject.SetParent( nodeObject );
			path.ControlPoint = controlPointObject;
			
			return path;
		} ),
		new PathNodeType( "Circle", "circle", ( nodeObject, nextObject ) =>
		{
			Log.Error( "TODO: Circle" );
			return null;
		} ),
		new PathNodeType( "Crossroad Side", "line_end_circle", ( nodeObject, nextObject ) =>
		{
			Log.Error( "TODO: Crossroad Side" );
			return null;
		} ),
		new PathNodeType( "Crossroad Center", "hub", ( nodeObject, nextObject ) =>
		{
			Log.Error( "TODO: Crossroad Center" );
			return null;
		} ),
	};

	private PathEditorContext Context;

	public PathEditorWidgetWindow( Widget parent, PathEditorContext context ) : base( parent, "Path Editor" )
	{
		Context = context;

		Layout = Layout.Column();
		Layout.Margin = 8;
		Layout.Spacing = 8;

		var pathTypeSelect = new SegmentedControl();
		foreach ( var pathNodeType in PathNodeTypes )
		{
			pathTypeSelect.AddOption( pathNodeType.Name, pathNodeType.Icon );
		}

		pathTypeSelect.OnSelectedChanged += s =>
		{
			Log.Info( s );
		};

		Layout.Add( pathTypeSelect );

		var actionButtonsRow = Layout.Row();
		actionButtonsRow.Spacing = 8;
		{
			var addButton = new Button.Primary( "Add", "add" );
			addButton.Clicked += () => AddNewNode( pathTypeSelect.SelectedIndex );
			actionButtonsRow.Add( addButton );

			actionButtonsRow.Add( new Button( "Replace", "refresh" ) );

			var deleteButton = new Button( "Delete", "delete" );
			deleteButton.Clicked += () => DeleteSelectedNode();
			actionButtonsRow.Add( deleteButton );
		}
		Layout.Add( actionButtonsRow );
	}

	private void AddNewNode( int index )
	{
		Assert.True( index >= 0 && index < PathNodeTypes.Count );

		GameObject lastGameObject;
		PathComponent previous = null;
		float width = 10;

		if ( Context.SelectedNode.IsValid() )
		{
			// TODO: detect infinite loops
			// Find the last node in a path
			var currentPathTerminator = Context.SelectedNode;
			while ( currentPathTerminator is not PathTerminator && currentPathTerminator.Next.IsValid() )
				currentPathTerminator = currentPathTerminator.Next;

			lastGameObject = currentPathTerminator.GameObject;
			previous = currentPathTerminator.Previous;
			width = currentPathTerminator.Width;
			currentPathTerminator.Destroy();
		}
		else
		{
			lastGameObject = SceneEditorSession.Active.Scene.CreateObject();
			lastGameObject.Transform.Position = Gizmo.CameraTransform.Position + Gizmo.CameraTransform.Forward * 100;
			lastGameObject.Transform.Rotation = Gizmo.CameraTransform.Rotation.Angles().WithPitch( 0 );
		}

		var terminatorGameObject = SceneEditorSession.Active.Scene.CreateObject();
		terminatorGameObject.Parent = lastGameObject.Parent;
		terminatorGameObject.Transform.Position =
			lastGameObject.Transform.Position + lastGameObject.Transform.Rotation.Forward * 100;
		terminatorGameObject.Transform.Rotation = lastGameObject.Transform.Rotation;

		var terminator = terminatorGameObject.Components.Create<PathTerminator>();
		terminator.Width = width;

		var newComponent = PathNodeTypes[index].Make( lastGameObject, terminatorGameObject );
		newComponent.Width = width;

		terminator.Previous = newComponent;
		newComponent.Previous = previous;
		newComponent.Next = terminator;
		if ( previous.IsValid() )
			previous.Next = newComponent;
	}

	private void DeleteSelectedNode()
	{
		if ( !Context.SelectedNode.IsValid() )
			return;

		var next = Context.SelectedNode.Next;
		var previous = Context.SelectedNode.Previous;
		if ( previous.IsValid() )
			previous.Next = next;
		if ( next.IsValid() )
			next.Previous = previous;

		Context.SelectedNode.Destroy();
	}
}
