using System;
using System.Linq;
using System.Xml;

namespace JobLocationFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var doc = new XmlDocument();
            doc.Load(@"C:\Users\SORLIK\Desktop\Untitled-2.xml");
            var locationsList = doc.GetElementsByTagName("job:location");
            var locationByParent = locationsList.Cast<XmlNode>().ToList().GroupBy(e => e.ParentNode).Distinct();

            foreach (var parent in locationByParent)
            {
                var locations = string.Join(',', parent.Select(x => x.InnerXml));

                var newNode = parent.First().Clone();
                newNode.InnerXml = locations;

                foreach (var location in parent)
                {
                    parent.Key.RemoveChild(location);
                }

                parent.Key.AppendChild(newNode);
            }

            doc.Save(@"C:\Users\SORLIK\Desktop\Untitled-2_fixed.xml");
        }
    }
}
