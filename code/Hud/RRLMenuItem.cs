namespace RRL.Hud;

public abstract class RRLMenuItem
{
	public string Title { get; protected set; }

	protected RRLMenuItem(string title)
	{
		Title = title;
	}

	public virtual bool Enabled() => true;
}
