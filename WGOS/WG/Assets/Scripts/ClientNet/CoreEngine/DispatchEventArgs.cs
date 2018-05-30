using System;

public class DispatchEventArgs:EventArgs
{
    public readonly int type;
    public object[] data;

    public DispatchEventArgs(int type, params object[] data)
    {
        this.type = type;
        this.data = data;
    }
    public DispatchEventArgs()
    {

    }
}
