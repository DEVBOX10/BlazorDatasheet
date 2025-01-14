using BlazorDatasheet.Events;
using BlazorDatasheet.Interfaces;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace BlazorDatasheet.Services;

/// <summary>
/// Assigns keyboard & mouse events to the browser "window" and allows us to respond to the events
/// </summary>
public class WindowEventService : IWindowEventService
{
    private readonly IJSRuntime _js;
    private DotNetObjectReference<WindowEventService> _dotNetHelper;
    private List<Tuple<string, string>> _fnStore = new List<Tuple<string, string>>();
    public event Func<KeyboardEventArgs, bool?> OnKeyDown;
    public event Func<MouseEventArgs, bool>? OnMouseDown;
    public event Func<MouseEventArgs, bool>? OnMouseUp;
    public event Func<MouseEventArgs, bool>? OnMouseMove;
    public event Func<PasteEventArgs, Task>? OnPaste;

    public WindowEventService(IJSRuntime js)
    {
        _js = js;
    }

    public async Task Init()
    {
        _dotNetHelper = DotNetObjectReference.Create(this);
        _fnStore.Add(await addWindowEvent("keydown", nameof(HandleWindowKeyDown)));
        _fnStore.Add(await addWindowEvent("mousedown", nameof(HandleWindowMouseDown)));
        _fnStore.Add(await addWindowEvent("mouseup", nameof(HandleWindowMouseUp)));
        _fnStore.Add(await addWindowEvent("mousemove", nameof(HandleWindowMouseMove)));
        _fnStore.Add(await addWindowEvent("paste", nameof(HandleWindowPaste)));
    }

    private async Task<Tuple<string, string>> addWindowEvent(string evType, string jsInvokableName)
    {
        var fnId = await _js.InvokeAsync<string>("setupBlazorWindowEvent", _dotNetHelper, evType, jsInvokableName);
        return new Tuple<string, string>(evType, fnId);
    }

    [JSInvokable]
    public bool? HandleWindowKeyDown(KeyboardEventArgs e)
    {
        var result = OnKeyDown?.Invoke(e);
        if (!result.HasValue)
            return false;
        return result.Value;
    }

    [JSInvokable]
    public bool HandleWindowMouseDown(MouseEventArgs e)
    {
        var result = OnMouseDown?.Invoke(e);
        if (!result.HasValue)
            return false;
        return result.Value;
    }
    
    [JSInvokable]
    public bool HandleWindowMouseUp(MouseEventArgs e)
    {
        var result = OnMouseUp?.Invoke(e);
        if (!result.HasValue)
            return false;
        return result.Value;
    }


    [JSInvokable]
    public bool HandleWindowMouseMove(MouseEventArgs e)
    {
        var result = OnMouseMove?.Invoke(e);
        if (!result.HasValue)
            return false;
        return result.Value;
    }

    [JSInvokable]
    public async Task HandleWindowPaste(PasteEventArgs e)
    {
        if (OnPaste is not null)
        {
            await OnPaste.Invoke(e);
        }
    }

    public async Task DisposeAsync()
    {
        try
        {
            foreach (var fn in _fnStore)
            {
                await _js.InvokeAsync<Task>("removeBlazorWindowEvent", fn.Item1, fn.Item2);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        _dotNetHelper?.Dispose();
    }

    public async void Dispose()
    {
        await DisposeAsync();
    }
}