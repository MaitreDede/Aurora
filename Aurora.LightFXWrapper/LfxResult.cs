using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aurora.LightFXWrapper
{
    public enum LfxResult : UInt16
    {
        LFX_SUCCESS = 0,        // Success
        LFX_FAILURE = 1,        // Generic failure
        LFX_ERROR_NOINIT = 2,   // System not initialized yet
        LFX_ERROR_NODEVS = 3,   // No devices available
        LFX_ERROR_NOLIGHTS = 4, // No lights available
        LFX_ERROR_BUFFSIZE = 5,	// Buffer size too small
    }
}
