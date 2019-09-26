using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BMPToJPG
{
    class EncoderEngine : IEncoderEngine
    {

        private string _filePath;
        private Bitmap _bitMap;
        private ImageCodecInfo _jpgEncoder;
        private System.Drawing.Imaging.Encoder _myEncoder;
        private EncoderParameters _myEncoderParameters;
        private EncoderParameter _myEncoderParameter;
        public EncoderEngine(string filePath)
        {
            _filePath = filePath;
            try
            {
                _bitMap = new Bitmap(_filePath);
            }
            catch (Exception ex)
            {
                using (EventLog log = new EventLog())
                {
                    log.Source = "BMPtoJPGConverter Encoder";
                    log.WriteEntry("There was a problem with the filepath. Automatically trying again. Path: " + _filePath + " " + ex.Message);
                }
                Thread.Sleep(500);
                _bitMap = new Bitmap(_filePath);
            }
            _jpgEncoder = GetEncoder(ImageFormat.Jpeg);
            _myEncoder = System.Drawing.Imaging.Encoder.Quality;
            _myEncoderParameters = new EncoderParameters(1);
            _myEncoderParameter = new EncoderParameter(_myEncoder, 50L);
        }

        public static EncoderEngine CreateEncoderEngine(string filePath)
        {
            return new EncoderEngine(filePath);
        }

        public void EncodeToJPG()
        {
            _myEncoderParameters.Param[0] = _myEncoderParameter;
            try
            {
                _bitMap.Save(StrippedPath() + ".jpg", _jpgEncoder, _myEncoderParameters);
            }
            catch (Exception)
            { }
            _bitMap.Dispose();
        }
        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();

            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
            return null;
        }

        private string StrippedPath()
        {
            int bmpIndex = _filePath.IndexOf(".bmp");
            return _filePath.Remove(bmpIndex);
        }

        public void Dispose()
        {
            _bitMap.Dispose();
            _myEncoderParameters.Dispose();
            _filePath = null;
            _myEncoderParameter.Dispose();

        }
    }
}
