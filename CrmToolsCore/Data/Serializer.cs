using System;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmDataGenerator
{
	/**
	 * TODO: use the generic CrmJsonSerializer instead of this specific serializer
	 */
	public class Serializer
	{
		public static Entity[] DeSerialize( string json ) { 
			DataContractJsonSerializer ser = new DataContractJsonSerializer( typeof( Entity[] ) );
			Entity[] ent = ( Entity[] )ser.ReadObject(
				new MemoryStream( System.Text.Encoding.UTF8.GetBytes( json ) )
			);
			return ent;
		}

		public static string Serialize( Entity[] entity ) {
			DataContractJsonSerializer ser = new DataContractJsonSerializer( typeof( Entity ) );
			MemoryStream ms = new MemoryStream();
			ser.WriteObject( ms, entity );
			ms.Seek( 0, SeekOrigin.Begin );
			return new StreamReader( ms ).ReadToEnd();
		}
	}
}
