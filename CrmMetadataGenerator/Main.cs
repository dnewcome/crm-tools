using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Altai.MSCrm;
using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.Sdk.Metadata;

namespace CrmMetadataGenerator
{
	class MainClass
	{
		static void Main( string[] args ) {
			if( args.Length < 2 ) {
				Console.WriteLine( "CRM metadata generator" );
				Console.WriteLine( "usage: mdgen bizorg file" );
				System.Environment.Exit( 1 );
			}

			// string json = new StreamReader( Console.OpenStandardInput() ).ReadToEnd();
			string currentDir = System.Environment.CurrentDirectory;
			string json = new StreamReader( new FileStream( currentDir + "\\" + args[1], FileMode.Open, FileAccess.Read ) ).ReadToEnd();

			EntityMetadata spec = Generator.CreateEntityMetadata4( Serializer.DeSerialize( json ) );

			MetadataService service = CRMServiceUtil.GetCrmMetadataService( args[0] );
			Generator.ExecuteMetadataRequest( spec, service );
		}
	}
}
