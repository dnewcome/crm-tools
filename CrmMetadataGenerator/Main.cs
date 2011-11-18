using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altai.MSCrm;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.Sdk.Metadata;
using System.Configuration;

namespace CrmMetadataGenerator
{
	class MainClass
	{
		static void Main( string[] args ) {
			if( args.Length < 2 ) {
				Console.WriteLine( "CRM metadata generator" );
				Console.WriteLine( "usage: mdgen bizorg file [configfile]" );
				System.Environment.Exit( 1 );
			}

			// string json = new StreamReader( Console.OpenStandardInput() ).ReadToEnd();
			string currentDir = System.Environment.CurrentDirectory;
			string json = new StreamReader( new FileStream( currentDir + "\\" + args[1], FileMode.Open, FileAccess.Read ) ).ReadToEnd();

			MetadataService service;
			if( args.Length == 4 ) {
				MSCrmConfigurationSection config = GetConfig( args[ 3 ] );
				service = CRMServiceUtil.GetCrmMetadataService( args[ 0 ], config );
			}
			else {
				service = CRMServiceUtil.GetCrmMetadataService( args[ 0 ] );
			}
			

			EntityMetadata spec = Generator.CreateEntityMetadata4( Serializer.DeSerialize( json ) );

			// create the actual entity
			Generator.ExecuteMetadataRequest( spec, service );

			// generate the non-primary attributes other than lookups
			Generator.ExecuteAttributeMetadataRequest( spec, service );
			
			// create relationships
			Generator.CreateLookups( Serializer.DeSerialize( json ), service );

			// If there are picklist items specified in the metadata, add them last
			Generator.CreatePickListValues( Serializer.DeSerialize( json ), service );
		}

		static Altai.MSCrm.MSCrmConfigurationSection GetConfig( string in_filename ) {
			ConfigurationFileMap fileMap = new ConfigurationFileMap( in_filename );
			Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration( fileMap );
			Altai.MSCrm.MSCrmConfigurationSection config = ( Altai.MSCrm.MSCrmConfigurationSection )configuration.GetSection( "Altai.MSCrm" );
			return config;
		}
	}
}
