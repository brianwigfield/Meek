using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Meek.Storage
{
    public class FileSystemRepository : Repository
    {
        readonly string _baseDir;

        public FileSystemRepository(string baseDir)
        {
            _baseDir = baseDir;
        }

        public MeekContent Get(string route)
        {
            var element = GetRouteElement(route);
            if (element == null)
                return null;

            var fileName = element.Attribute("fileName").Value;
            if (!File.Exists(fileName))
                return null;

            return new MeekContent(element.Attribute("title").Value, File.ReadAllText(fileName),
                                   bool.Parse(element.Attribute("partial").Value));

        }

        public bool Exists(string route)
        {
            return GetRouteElement(route) != null;
        }

        public IEnumerable<string> AvailableRoutes(ContentTypes? type)
        {
            var query = DataFile.Element("Meek").Elements("content");

            if (type.HasValue)
            {
                if (type.Value == ContentTypes.Partial)
                    query = query.Where(x => x.Attribute("partial").Value == bool.TrueString);
                else
                    query = query.Where(x => x.Attribute("partial").Value == bool.FalseString);
            }

            return query.Select(x => x.Attribute("route").Value);

        }

        public void Save(string route, MeekContent content)
        {
            var parent = DataFile.Element("Meek");
            var element = parent.Elements("content").SingleOrDefault(ByRoute(route));
            string fileKey;
            if (element == null)
            {
                fileKey = Path.Combine(_baseDir, Guid.NewGuid().ToString());

                var newElement = new XElement("content");
                newElement.Add(new XAttribute("route", route));
                newElement.Add(new XAttribute("fileName", fileKey));
                newElement.Add(new XAttribute("title", content.Title));
                newElement.Add(new XAttribute("partial", content.Partial.ToString()));

                parent.Add(newElement);
            }
            else
            {
                fileKey = element.Attribute("fileName").Value;
                element.SetAttributeValue("title", content.Title);
                element.SetAttributeValue("partial", content.Partial.ToString());
            }

            var stream = File.CreateText(fileKey);
            stream.Write(content.Contents);
            stream.Close();

            DataFile = parent.Document;

        }

        public void Remove(string route)
        {
            var element = GetRouteElement(route);
            if (element == null)
                return;

            File.Delete(element.Attribute("fileName").Value);
            var modifiedDoc = element.Document;
            element.Remove();
            DataFile = modifiedDoc;
        }

        private XDocument DataFile
        {
            get
            {
                var dataFileName = Path.Combine(_baseDir, "Data.xml");
                if (!File.Exists(dataFileName))
                    CreateFileData(dataFileName);

                return XDocument.Load(dataFileName);
            }
            set
            {
                var writer = XmlWriter.Create(Path.Combine(_baseDir, "Data.xml"));
                value.WriteTo(writer);
                writer.Flush();
                writer.Close();
            }
        }

        private void CreateFileData(string fileName)
        {
            var fileStream = File.CreateText(fileName);
            fileStream.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            fileStream.WriteLine("<Meek>");
            fileStream.WriteLine("</Meek>");
            fileStream.Close();
        }

        private XElement GetRouteElement(string route)
        {
            var element = DataFile.Element("Meek").Elements("content").SingleOrDefault(ByRoute(route));
            if (element == null)
                return null;

            return element;
        }

        private Func<XElement, bool> ByRoute(string route)
        {
            return x => x.Attribute("route").Value.ToLower() == route.ToLower();
        }

    }
}
