//http://xmltocsharp.azurewebsites.net/ used to generate this template from .osm file

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace OSM
{
    [XmlRoot(ElementName = "bounds")]
    public class BoundsXml
    {
        [XmlAttribute(AttributeName = "minlat")]
        public string Minlat { get; set; }
        [XmlAttribute(AttributeName = "minlon")]
        public string Minlon { get; set; }
        [XmlAttribute(AttributeName = "maxlat")]
        public string Maxlat { get; set; }
        [XmlAttribute(AttributeName = "maxlon")]
        public string Maxlon { get; set; }
    }

    [XmlRoot(ElementName = "node")]
    public class NodeXml
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "changeset")]
        public string Changeset { get; set; }
        [XmlAttribute(AttributeName = "timestamp")]
        public string Timestamp { get; set; }
        [XmlAttribute(AttributeName = "user")]
        public string User { get; set; }
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
        [XmlAttribute(AttributeName = "lat")]
        public string Lat { get; set; }
        [XmlAttribute(AttributeName = "lon")]
        public string Lon { get; set; }
        [XmlElement(ElementName = "tag")]
        public List<TagXml> Tag { get; set; }
    }

    [XmlRoot(ElementName = "tag")]
    public class TagXml
    {
        [XmlAttribute(AttributeName = "k")]
        public string K { get; set; }
        [XmlAttribute(AttributeName = "v")]
        public string V { get; set; }
    }

    [XmlRoot(ElementName = "nd")]
    public class NdXml
    {
        [XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
    }

    [XmlRoot(ElementName = "way")]
    public class WayXml
    {
        [XmlElement(ElementName = "nd")]
        public List<NdXml> Nd { get; set; }
        [XmlElement(ElementName = "tag")]
        public List<TagXml> Tag { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "changeset")]
        public string Changeset { get; set; }
        [XmlAttribute(AttributeName = "timestamp")]
        public string Timestamp { get; set; }
        [XmlAttribute(AttributeName = "user")]
        public string User { get; set; }
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
    }

    [XmlRoot(ElementName = "member")]
    public class MemberXml
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }
        [XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
        [XmlAttribute(AttributeName = "role")]
        public string Role { get; set; }
    }

    [XmlRoot(ElementName = "relation")]
    public class RelationXml
    {
        [XmlElement(ElementName = "member")]
        public List<MemberXml> Member { get; set; }
        [XmlElement(ElementName = "tag")]
        public List<TagXml> Tag { get; set; }
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }
        [XmlAttribute(AttributeName = "visible")]
        public string Visible { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "changeset")]
        public string Changeset { get; set; }
        [XmlAttribute(AttributeName = "timestamp")]
        public string Timestamp { get; set; }
        [XmlAttribute(AttributeName = "user")]
        public string User { get; set; }
        [XmlAttribute(AttributeName = "uid")]
        public string Uid { get; set; }
    }

    [XmlRoot(ElementName = "osm")]
    public class OsmXml
    {
        [XmlElement(ElementName = "bounds")]
        public BoundsXml Bounds { get; set; }
        [XmlElement(ElementName = "node")]
        public List<NodeXml> Node { get; set; }
        [XmlElement(ElementName = "way")]
        public List<WayXml> Way { get; set; }
        [XmlElement(ElementName = "relation")]
        public List<RelationXml> Relation { get; set; }
        [XmlAttribute(AttributeName = "version")]
        public string Version { get; set; }
        [XmlAttribute(AttributeName = "generator")]
        public string Generator { get; set; }
        [XmlAttribute(AttributeName = "copyright")]
        public string Copyright { get; set; }
        [XmlAttribute(AttributeName = "attribution")]
        public string Attribution { get; set; }
        [XmlAttribute(AttributeName = "license")]
        public string License { get; set; }
    }

}
