using System;
using System.IO;
using System.Linq;
using System.Xml;
using Serilog;

public class XMLProcessor {

  private readonly string _jobTag = "job:location";

  public void ProcessFile (string filepath) {
    Log.Information ($@"Starting {filepath} processing");

    ValidateAndThrow (filepath);
    var doc = LoadAsXml (filepath);

    Log.Information ("File is valid XML, can be processed");
    ProceedFile (doc);

    var newFileName = NewFileName (filepath);
    Log.Information ($@"Saving file as {newFileName}");
    doc.Save (newFileName);
  }

  private void ValidateAndThrow (string filepath) {
    if (string.IsNullOrEmpty (filepath))
      throw new FileNotFoundException ($@"Filepath is empty");

    if (!FileExists (filepath))
      throw new FileNotFoundException ($@"File does not exists: {filepath}");
  }
  private bool FileExists (string filepath) => File.Exists (filepath);
  private XmlDocument LoadAsXml (string filepath) {

    var doc = new XmlDocument ();
    doc.Load (filepath);

    return doc;
  }

  private void ProceedFile (XmlDocument doc) {
    var locationsList = doc.GetElementsByTagName (_jobTag)?.Cast<XmlNode> ();

    Log.Debug ($@"Found {locationsList?.Count()} {_jobTag} tags");
    if (!locationsList.Any ())
      throw new XmlException ($@"Tag {_jobTag} not found.");

    var locationByParent = locationsList.Cast<XmlNode> ().ToList ().GroupBy (e => e.ParentNode).Distinct ();
    Log.Debug ($@"Found {locationByParent?.Count()} parent groups. Starting iteration through them");

    foreach (var parent in locationByParent) {
      var locations = string.Join (',', parent.Select (x => x.InnerXml));

      var newNode = parent.First ().Clone ();
      newNode.InnerXml = locations;

      foreach (var location in parent) {
        parent.Key.RemoveChild (location);
      }

      parent.Key.AppendChild (newNode);
    }

    locationsList = doc.GetElementsByTagName (_jobTag)?.Cast<XmlNode> ();

    Log.Debug ($@"Found {locationsList?.Count()} {_jobTag} tags after reduction");
  }

  private string NewFileName (string filename) => filename.Replace (".xml", $@"_modified_{DateTime.Now.
  ToUniversalTime()
  .ToString()
  .Replace('.', '_')
  .Replace(':', '_')
  .Replace(' ', '_')
  }.xml");
}