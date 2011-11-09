using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using Altai.MSCrm;

namespace CrmDataGenerator
{

	class Generator
	{
		/*
		public static string ms_bizorg = "NLUS";

		public static void Generate( string json ) {
			Entity[] entity = Serializer.DeSerialize( json );
			
			IEntityWrap de = EntityWrapFactory.CreateWrap( entity.Name );
			foreach( Property prop in entity.Properties ) {
				switch( prop.Type ) { 
					case "String":
						de.AddString( prop.Name, ( string )prop.Value );
						break;
					case "DateTime":
						de.AddDateTime( prop.Name, DateTime.Parse( ( string )prop.Value ) );
						break;
					default:
						throw new NotSupportedException( prop.Type + " not supported" );
				}
			}
			DynamicEntityHelperWrap.CreateDynamicEntity( ms_bizorg, de );			
		}
		*/

	} // class
} // namespace 

