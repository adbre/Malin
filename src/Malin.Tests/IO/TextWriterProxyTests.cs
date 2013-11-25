using System.IO;
using Malin.IO;
using NUnit.Framework;

namespace Malin.Tests.IO
{
    public class TextWriterProxyTests
    {
        [Test]
        public void ShouldWriteToInnerTextWriters()
        {
            const string expected = "0E62D3EF-75A5-4EB2-9915-F8C169AF77B2\r\n";

            using (var writer1 = new StringWriter())
            using (var writer2 = new StringWriter())
            {
                var sut = new TextWriterProxy(writer1, writer2);

                sut.Write(expected);

                var actual1 = writer1.ToString();
                var actual2 = writer2.ToString();
                Assert.That(actual1, Is.EqualTo(actual2), "#1");
                Assert.That(actual1, Is.EqualTo(expected), "#2");
            }
        }
    }
}
