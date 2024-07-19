using Editor;
using TTP.Paths;

namespace Sandbox;

/// <summary>
/// Modify paths
/// </summary>
[EditorTool]
[Title( "Paths" )]
[Icon( "route" )]
[Alias( "paths" )]
[Group( "9" )]
public sealed class PathEditorTool : EditorTool
{
	private PathEditorWidgetWindow PathEditor;
	private PathEditorContext Context = new();

	public override void OnEnabled()
	{
		if ( GetSelectedComponent<PathComponent>() is not { } node )
		{
			Selection.Clear();
		}
		else
		{
			Context.SelectedNode = node;
		}

		PathEditor = new PathEditorWidgetWindow( SceneOverlay, Context );
		AddOverlay( PathEditor, TextFlag.RightBottom );
	}

	public override void OnUpdate()
	{
		Context.SelectedNode = GetSelectedComponent<PathComponent>();
		PathEditor.Visible = Context.SelectedNode is not null;

		if ( Context.SelectedNode is null )
			return;
	}
}
