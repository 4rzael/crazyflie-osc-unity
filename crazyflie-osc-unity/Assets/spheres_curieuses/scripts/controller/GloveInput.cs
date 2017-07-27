public class GloveInput
{
    public int currentValue = 0;
    public int edgeValue = 0; // rising => 1, falling => -1
    public int xAxis = 0;
    public int yAxis = 0;

    public GloveInput(int value = 0) { setValue(value); }
    public void setValue(int value, bool triggerEdge = true)
    {
        if (currentValue == 0 && value > 0)
            edgeValue = 1;
        else if (currentValue > 0 && value == 0)
            edgeValue = -1;
        else
            edgeValue = 0;

        currentValue = value;
    }
    override public string ToString() { return string.Format("GloveInput <value:{0} edge:{1} axis:[{2},{3}]>", currentValue, edgeValue, xAxis, yAxis); }
}
