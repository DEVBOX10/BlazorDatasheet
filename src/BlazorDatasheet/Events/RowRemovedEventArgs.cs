﻿namespace BlazorDatasheet.Events;

public class RowRemovedEventArgs
{
    public int Index { get; }
    public RowRemovedEventArgs(int index)
    {
        Index = index;
    }
}