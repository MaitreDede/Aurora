using Aurora.LightFXWrapper.InteropServices;
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aurora.LightFXWrapper
{
    public static class DllMain
    {
        private static bool isInitialized = false;


        [DllExport("LFX_GetVersion", CallingConvention.Cdecl)]
        public static LfxResult LFX_GetVersion(StringBuilder version, uint versionSize)
        {
            if (!isInitialized)
                return LfxResult.LFX_FAILURE;
            version.Append("2.2.0.0");
            return LfxResult.LFX_SUCCESS;
        }
    }
}
