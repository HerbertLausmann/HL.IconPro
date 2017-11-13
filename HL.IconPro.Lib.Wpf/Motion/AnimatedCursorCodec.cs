using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HL.IconPro.Lib.Wpf.Motion
{
    /// <summary>
    /// Base class for Animated Cursors Codecs. Adds support for the Animated Cursor Metadata
    /// </summary>
    public class AnimatedCursorCodec : CursorCodec
    {
        protected string _Name;
        protected string _Author;
        protected ushort _FrameRate;

        /// <summary>
        /// The cursor's name, if available
        /// </summary>
        public string Name => _Name;

        /// <summary>
        /// The name of the author, if available
        /// </summary>
        public string Author => _Author;

        /// <summary>
        /// Default frame display rate (measured in 1/60th-of-a-second units)
        /// </summary>
        public ushort FrameRate => _FrameRate;
    }
}
