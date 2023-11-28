using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dcidr.Blazor.Extensions
{
    public static class JSRuntimeExtentions
    {
        public static ValueTask SaveAs(this IJSRuntime js, string filename, byte[] data)
            => js.InvokeVoidAsync(
                "dcidr.interop.saveAsFile",
                filename,
                Convert.ToBase64String(data));
    }
}
