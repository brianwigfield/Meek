using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
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

            var filePath = Path.Combine(_baseDir, element.Attribute("fileName").Value);
            if (!File.Exists(filePath))
                return null;

            string title = null;
            if (element.Attribute("title") != null)
                title = element.Attribute("title").Value;

            return new MeekContent(title, File.ReadAllText(filePath),
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
                fileKey = Guid.NewGuid().ToString();

                var newElement = new XElement("content");
                newElement.Add(new XAttribute("route", route));
                newElement.Add(new XAttribute("fileName", fileKey));
                newElement.Add(new XAttribute("partial", content.Partial.ToString()));
                if (!string.IsNullOrEmpty(content.Title))
                    newElement.Add(new XAttribute("title", content.Title));

                parent.Add(newElement);
            }
            else
            {
                fileKey = element.Attribute("fileName").Value;
                element.SetAttributeValue("title", content.Title);
                element.SetAttributeValue("partial", content.Partial.ToString());
            }

            var stream = File.CreateText(Path.Combine(_baseDir, fileKey));
            var binaryWriter = new BinaryWriter(stream.BaseStream);
            var contentBytes = Encoding.UTF8.GetBytes(content.Contents);
            binaryWriter.Write(contentBytes, 0, contentBytes.Length);
            stream.Close();

            DataFile = parent.Document;

        }

        public void Remove(string route)
        {
            var element = GetRouteElement(route);
            if (element == null)
                return;

            File.Delete(Path.Combine(_baseDir,element.Attribute("fileName").Value));
            var modifiedDoc = element.Document;
            element.Remove();
            DataFile = modifiedDoc;
        }

        public string SaveFile(MeekFile file)
        {
            var parent = DataFile.Element("Meek");
            var fileKey = Guid.NewGuid().ToString();

            var newElement = new XElement("file");
            newElement.Add(new XAttribute("fileName", fileKey));
            newElement.Add(new XAttribute("originalFileName", file.FileName));
            newElement.Add(new XAttribute("contentType", file.ContentType));

            parent.Add(newElement);

            var stream = File.CreateText(Path.Combine(_baseDir,fileKey));
            var binaryWriter = new BinaryWriter(stream.BaseStream);
            binaryWriter.Write(file.Contents, 0, file.Contents.Length);
            stream.Close();

            DataFile = parent.Document;
            return fileKey;
        }

        public MeekFile GetFile(string fileId)
        {
            var element = GetFileElement(fileId);
            if (element == null)
                return null;

            var filePath = Path.Combine(_baseDir, element.Attribute("fileName").Value);
            if (!File.Exists(filePath))
                return null;

            return new MeekFile(element.Attribute("originalFileName").Value, element.Attribute("contentType").Value,
                                File.ReadAllBytes(filePath));
        }

        public IEnumerable<string> GetFiles()
        {
            return DataFile.Element("Meek").Elements("file").Select(x => x.Attribute("fileName").Value);
        }

        public void RemoveFile(string fileId)
        {
            var element = GetFileElement(fileId);
            if (element == null)
                return;

            File.Delete(Path.Combine(_baseDir, element.Attribute("fileName").Value));
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

        private XElement GetFileElement(string fileId)
        {
            var element = DataFile.Element("Meek").Elements("file").SingleOrDefault(x => x.Attribute("fileName").Value.ToLower() == fileId.ToLower());
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
