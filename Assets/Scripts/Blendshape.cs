/// <summary>
/// Wrapper Class for Positive & Negative Values of Blendshapes
/// </summary>
public class Blendshape
{    
    public int positiveIndex { get; set; }
    public int negativeIndex { get; set; }

    public Blendshape (int positiveIndex, int negativeIndex)
    {
        this.positiveIndex = positiveIndex;
        this.negativeIndex = negativeIndex;
    }

}
