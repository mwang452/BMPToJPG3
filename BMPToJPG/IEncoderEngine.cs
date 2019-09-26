using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BMPToJPG
{
    interface IEncoderEngine : IDisposable
    {
        void EncodeToJPG();
    }
}
