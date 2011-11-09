using System;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmMetadataGenerator
{
	/**
	* TODO: use the generic CrmJsonSerializer instead of this specific serializer
	*/
	class Serializer
	{
		public static EntityMetadataSpec DeSerialize( string json ) {
			DataContractJsonSerializer ser = new DataContractJsonSerializer( typeof( EntityMetadataSpec ) );
			EntityMetadataSpec ent = ( EntityMetadataSpec )ser.ReadObject(
				new MemoryStream( System.Text.Encoding.UTF8.GetBytes( json ) )
			);
			return ent;
		}

		public static string Serialize( EntityMetadataSpec entity ) {
			DataContractJsonSerializer ser = new DataContractJsonSerializer( typeof( EntityMetadataSpec ) );
			MemoryStream ms = new MemoryStream();
			ser.WriteObject( ms, entity );
			ms.Seek( 0, SeekOrigin.Begin );
			return new StreamReader( ms ).ReadToEnd();
		}
	}
}
