using System;
using System.Configuration;
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
				Console.WriteLine( "usage: crmgen bizorg file [configfile]" );
				System.Environment.Exit( 1 );
			}


			string currentDir = System.Environment.CurrentDirectory;
			string json = new StreamReader( new FileStream( currentDir + "\\" + args[1], FileMode.Open, FileAccess.Read ) ).ReadToEnd();

			MSCrmConfigurationSection config = null;
			if( args.Length == 3 ) {
				config = GetConfig( args[ 2 ] );
			}

			try {
				Entity[] ent = Serializer.DeSerialize( json );
				foreach( Entity item in ent ) {
					IEntityWrap wrap = WrapFromSpec( item );
					if( config != null ) {
						DynamicEntityHelperWrap.CreateDynamicEntity( args[ 0 ], config, wrap );
					}
					else {
						DynamicEntityHelperWrap.CreateDynamicEntity( args[ 0 ], wrap );
					}
					
				}
				
			}
			catch( Exception ex ) {
				Console.WriteLine( ex.Message );
			}
		}

		static Altai.MSCrm.MSCrmConfigurationSection GetConfig( string in_filename ) {
			ConfigurationFileMap fileMap = new ConfigurationFileMap( in_filename );
			Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration( fileMap );
			Altai.MSCrm.MSCrmConfigurationSection config = ( Altai.MSCrm.MSCrmConfigurationSection )configuration.GetSection( "Altai.MSCrm" );
			return config;
		}

	} // class
} // namespace
