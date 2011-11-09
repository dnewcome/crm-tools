using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altai.MSCrm;
using Microsoft.Crm.Sdk;

namespace CrmDataGenerator
{
	class MainClass
	{
		/**
		 * Create an instance of IEntityWrap from an entity spec
		 */
		static IEntityWrap WrapFromSpec( Entity spec ) {
			IEntityWrap wrap = EntityWrapFactory.CreateWrap( spec.Name );
			foreach( Property prop in spec.Properties ) {
				switch( prop.Type ) { 
					case "String":
						wrap.AddString( spec.NamingPrefix + "_" + prop.Name, ( string )prop.Value );
						break;
					case "DateTime":
						wrap.AddDateTime( spec.NamingPrefix + "_" + prop.Name, DateTime.Parse( ( string )prop.Value ) );
						break;
					case "Picklist":
						wrap.AddPickList( spec.NamingPrefix + "_" + prop.Name, ( int )prop.Value );
						break;
					case "Boolean":
						wrap.AddBoolean( spec.NamingPrefix + "_" + prop.Name, ( bool )prop.Value );
						break;
					case "Money":
						wrap.AddMoney( spec.NamingPrefix + "_" + prop.Name, ( decimal )prop.Value );
						break;

					default:
						throw new NotImplementedException( "WrapFromSpec(): " + prop.Type + " not supported yet." );
				}
			}
			return wrap;
		}
		static void Main( string[] args ) {
			if( args.Length < 2 ) {
				Console.WriteLine( "CRM data generator" );
				Console.WriteLine( "usage: crmgen bizorg file" );
				System.Environment.Exit( 1 );
			}

			string currentDir = System.Environment.CurrentDirectory;
			string json = new StreamReader( new FileStream( currentDir + "\\" + args[1], FileMode.Open, FileAccess.Read ) ).ReadToEnd();


			try {
				Entity[] ent = Serializer.DeSerialize( json );
				foreach( Entity item in ent ) {
					IEntityWrap wrap = WrapFromSpec( item );
					DynamicEntityHelperWrap.CreateDynamicEntity( "testorg", wrap );
				}
				
			}
			catch( Exception ex ) {
				Console.WriteLine( ex.Message );
			}
		}
	}
}
