using System;
using System.IO;
using System.Linq;
using System.Text;

namespace Malin.IO
{
    public class TextWriterProxy : TextWriter
    {
        private readonly Encoding _encoding = Encoding.UTF8;
        private readonly TextWriter[] _textWriters;

        public TextWriterProxy(params TextWriter[] textWriters)
            : this(Encoding.UTF8, textWriters)
        {
        }

        public TextWriterProxy(Encoding encoding, params TextWriter[] textWriters)
        {
            if (encoding == null) throw new ArgumentNullException("encoding");
            if (textWriters == null) throw new ArgumentNullException("textWriters");
            if (textWriters.Any(t => t == null)) throw new ArgumentException("No text writer may be null");
            _encoding = encoding;
            _textWriters = textWriters.ToArray();
        }

        public override Encoding Encoding
        {
            get { return _encoding; }
        }

        public override void Write(char value)
        {
            foreach (var textWriter in _textWriters)
                textWriter.Write(value);
        }
    }
}
