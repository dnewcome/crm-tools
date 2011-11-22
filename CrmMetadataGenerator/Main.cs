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

#if CRM4
using Gen = CrmMetadataGenerator.Generator;
#else
using Gen = CrmMetadataGenerator.Generator2011;
using MSCrmConfigurationSection = Altai.MSCrm.MSCrm5ConfigurationSection;
using MetadataService = Microsoft.Xrm.Sdk.IOrganizationService;
using EntityMetadata = Microsoft.Xrm.Sdk.Metadata.EntityMetadata;
#endif

namespace CrmMetadataGenerator
{
	class MainClass
	{
		static void Main( string[] args ) {
#if CRM4
			Console.WriteLine( "CRM metadata generator - CRM4" );
#else
			Console.WriteLine( "CRM metadata generator - CRM2011" );
#endif
			if( args.Length < 2 ) {
				Console.WriteLine( "usage: mdgen bizorg file [configfile]" );
				System.Environment.Exit( 1 );
			}

			// string json = new StreamReader( Console.OpenStandardInput() ).ReadToEnd();
			string currentDir = System.Environment.CurrentDirectory;
			string json = new StreamReader( new FileStream( currentDir + "\\" + args[1], FileMode.Open, FileAccess.Read ) ).ReadToEnd();

			MetadataService service;
#if CRM4
			if( args.Length == 3 ) {
				MSCrmConfigurationSection config = GetConfig( args[ 2 ] );
				service = CRMServiceUtil.GetCrmMetadataService( args[ 0 ], config );
			}
			else {
				service = CRMServiceUtil.GetCrmMetadataService( args[ 0 ] );
			}
#else
			if( args.Length == 3 ) {
				Console.WriteLine( "Reading configuration from " + args[ 2 ] );
				MSCrmConfigurationSection config = GetConfig( args[ 2 ] );
				service = CRM5ServiceUtil.GetCrmService( config );
			}
			else {
				service = CRM5ServiceUtil.GetCrmService();
			}
#endif

			EntityMetadata spec = Gen.CreateEntityMetadata4( Serializer.DeSerialize( json ) );

			// create the actual entity
			Gen.ExecuteMetadataRequest( spec, service );

			// generate the non-primary attributes other than lookups
			Gen.ExecuteAttributeMetadataRequest( spec, service );
			
			// create relationships
			Gen.CreateLookups( Serializer.DeSerialize( json ), service );

			// If there are picklist items specified in the metadata, add them last
			// Gen.CreatePickListValues( Serializer.DeSerialize( json ), service );
		}

		static MSCrmConfigurationSection GetConfig( string in_filename ) {
			ConfigurationFileMap fileMap = new ConfigurationFileMap( in_filename );
			Configuration configuration = System.Configuration.ConfigurationManager.OpenMappedMachineConfiguration( fileMap );
			
			MSCrmConfigurationSection config = ( MSCrmConfigurationSection )configuration.GetSection( "Altai.MSCrm" );
			return config;			
		}
	}
}

