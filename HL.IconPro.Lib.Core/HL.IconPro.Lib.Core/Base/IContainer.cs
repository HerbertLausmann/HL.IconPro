using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace HL.IconPro.Lib.Core
{
    public interface IContainer
    {
        void Write(Stream Output);
        void Read(Stream Source);
    }
}
