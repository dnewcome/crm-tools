using System;
using System.Runtime.Serialization;

using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmMetadataGenerator
{
	[DataContract]
	public class EntityMetadataSpec
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string PrimaryAttribute { get; set; }
		// naming prefix comes into play when creating metadata for different 
		// publishers/bizorgs - will get prepended as prefix_name
		[DataMember]
		public string NamingPrefix{ get; set; }
		[DataMember]
		public List<AttributeMetadataSpec> Attributes { get; set; }
	}
	[DataContract]
	public class AttributeMetadataSpec	
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string Type { get; set; }

		[DataMember]
		// used only for lookup attributes.. todo: should make this cleaner
		public string ReferencedEntity { get; set; }

		[DataMember]
		// used only for lookup attributes.. todo: should make this cleaner
		public List<PicklistMetadata> Values { get; set; }

	}

	[DataContract]
	public class PicklistMetadata
	{
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public int Value { get; set; }
	}

}
