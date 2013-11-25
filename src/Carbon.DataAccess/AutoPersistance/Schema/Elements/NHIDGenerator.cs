using System.Xml.Serialization;
using NHibernate.Carbon.AutoPersistance.Core;

namespace NHibernate.Carbon.AutoPersistance.Schema.Elements
{
	[Optimization("For id value generation, identity generator not supported in favor of native generator")]
	[XmlRoot(ElementName = "generator")]
	public class NHIDGenerator
	{
		[XmlAttribute(AttributeName = "class")]
		public string Class { get; set; }

		[XmlAttribute(AttributeName = "unsaved-value")]
		public string UnSavedValue { get; set; }

		public void CreateGenerator(IdGenerationTypes idGenerationType)
		{
			// OPTIMIZATION: identity generator not supported in favor of native generator
			switch (idGenerationType)
			{
				case IdGenerationTypes.Assigned:
					this.Class = "assigned";
					break;
				case IdGenerationTypes.CombGuid:
					this.Class = "guid.comb";
					break;
				case IdGenerationTypes.Identity:
					this.Class = "identity";
					break;
				case IdGenerationTypes.Native:
					this.Class = "native";
					break;
			}
		}

		public string Serialize()
		{
			return ORMUtils.Serialize(this);
		}
	}
}