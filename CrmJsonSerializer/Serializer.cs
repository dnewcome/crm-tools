using System;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CrmDataGenerator
{
	class Serializer
	{
		public static T DeSerialize<T>( string json ) { 
			DataContractJsonSerializer ser = new DataContractJsonSerializer( typeof( T ) );
			T ent = ( T )ser.ReadObject(
				new MemoryStream( System.Text.Encoding.UTF8.GetBytes( json ) )
			);
			return ent;
		}

		public static string Serialize<T>( T entity ) {
			DataContractJsonSerializer ser = new DataContractJsonSerializer( typeof( T ) );
			MemoryStream ms = new MemoryStream();
			ser.WriteObject( ms, entity );
			ms.Seek( 0, SeekOrigin.Begin );
			return new StreamReader( ms ).ReadToEnd();
		}
	}
}
