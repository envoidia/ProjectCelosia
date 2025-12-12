namespace API.Graphics;

/// <summary>
/// Drawing is split into 3 batches. Within each batch, all sprites will render above all shapes
/// </summary>
public enum RenderPriority {
    B1Low,
    B1Med,
    B1High,

    B2Low,
    B2Med,
    B2High,

    B3Low,
    B3Med,
    B3High,

    Highest
}