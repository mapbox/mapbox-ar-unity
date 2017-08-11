namespace Mapbox.IO.Compression
{
    using System.Diagnostics;

    internal enum CompressionTracingSwitchLevel {
        Off = 0,
        Informational = 1,
        Verbose = 2
    }

    // No tracing on Silverlight nor Windows Phone 7.
    internal class CompressionTracingSwitch
#if !NETFX_CORE
        : System.Diagnostics.Switch
#endif // !NETFX_CORE
	{
		internal readonly static CompressionTracingSwitch tracingSwitch =
            new CompressionTracingSwitch("CompressionSwitch", "Compression Library Tracing Switch");

        internal CompressionTracingSwitch(string displayName, string description)
#if !NETFX_CORE
            : base(displayName, description)
#endif // !NETFX_CORE
		{
		}

        public static bool Verbose {
            get {
#if NETFX_CORE
				return false;
#else
                return tracingSwitch.SwitchSetting >= (int)CompressionTracingSwitchLevel.Verbose;
#endif
            }
        }

        public static bool Informational {
            get {
#if NETFX_CORE
				return false;
#else
                return tracingSwitch.SwitchSetting >= (int)CompressionTracingSwitchLevel.Informational;
#endif
            }
        }

#if ENABLE_TRACING
        public void SetSwitchSetting(CompressionTracingSwitchLevel level) {
            if (level < CompressionTracingSwitchLevel.Off || level > CompressionTracingSwitchLevel.Verbose) {
                throw new ArgumentOutOfRangeException("level");
            }
            this.SwitchSetting = (int)level;
        }
#endif

    }    
}

