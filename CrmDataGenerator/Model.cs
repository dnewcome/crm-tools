using System;
using System.IO;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using Altai.MSCrm;

namespace CrmDataGenerator
{
	[DataContract]
	class Property {
		[DataMember]
		public string Type { 
			get {
				return _type.ToString();
			} 
			set{
				_type = ( AttributeTypeCode )Enum.Parse( typeof( AttributeTypeCode ), value );
			} 
		}
		private AttributeTypeCode _type;

		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public object Value { get; set; }
	}
	[DataContract]
	class Entity {
		[DataMember]
		public string Name { get; set; }
		[DataMember]
		public string NamingPrefix { get; set; }
		[DataMember]
		public Property[] Properties { get; set; }
	}

	
} // namespace 

