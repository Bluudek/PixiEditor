using System.Runtime.InteropServices;
using PixiEditor.Extensions.CommonApi.Windowing;
using PixiEditor.Extensions.Wasm.Api.FlyUI;
using PixiEditor.Extensions.Wasm.Bridge;

namespace PixiEditor.Extensions.Wasm.Api.Window;

public class WindowProvider : IWindowProvider
{
    public PopupWindow CreatePopupWindow(string title, LayoutElement body)
    {
        CompiledControl compiledControl = body.BuildNative();
        byte[] bytes = compiledControl.Serialize().ToArray();
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        int handle = Native.create_popup_window(title, ptr, bytes.Length);
        Marshal.FreeHGlobal(ptr);
        
        SubscribeToEvents(compiledControl);
        return new PopupWindow(handle) { Title = title };
    }

    internal void LayoutStateChanged(int uniqueId, CompiledControl newLayout)
    {
        byte[] bytes = newLayout.Serialize().ToArray();
        IntPtr ptr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, ptr, bytes.Length);
        Native.state_changed(uniqueId, ptr, bytes.Length);
        Marshal.FreeHGlobal(ptr);

        SubscribeToEvents(newLayout);
    }

    private void SubscribeToEvents(CompiledControl body)
    {
        foreach (CompiledControl child in body.Children)
        {
            SubscribeToEvents(child);
        }

        foreach (var queuedEvent in body.QueuedEvents)
        {
            Native.subscribe_to_event(body.UniqueId, queuedEvent);
        }
    }

    public IPopupWindow CreatePopupWindow(string title, object body)
    {
        if(body is not LayoutElement element)
            throw new ArgumentException("Body must be of type LayoutElement");

        return CreatePopupWindow(title, element);
    }

    public IPopupWindow GetWindow(BuiltInWindowType type)
    {
        int handle = Native.get_built_in_window((int)type);
        return new PopupWindow(handle);
    }

    public IPopupWindow GetWindow(string windowId)
    {
        int handle = Native.get_window(windowId);
        return new PopupWindow(handle);
    }
}
