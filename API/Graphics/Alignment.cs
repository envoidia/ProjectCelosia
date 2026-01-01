namespace API.Graphics;

// todo more alignments (center left/right, top/bottom center)
public enum Alignment
{
    TopLeft,
    TopRight,
    BottomLeft,
    BottomRight,
    Center,

    /// <summary>
    /// Origin will never automatically update.
    /// Intended for <c>IActor</c>s that are part of an <c>ILayoutWidget</c> that controls their origin
    /// </summary>
    Controlled
}