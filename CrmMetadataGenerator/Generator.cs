using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Crm.Sdk;
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.Sdk.Metadata;

namespace CrmMetadataGenerator
{
	class Generator
	{

		private static string ApplyPrefix( string prefix, string name ) {
			if( !String.IsNullOrEmpty( prefix ) ) {
				return prefix + "_" + name;
			}
			else {
				return name;
			}
		}

		/**
		 * Create a picklist item in an existing picklist - code adapted from 4.0 SDK documentation
		 * mk:@MSITStore:C:\Users\Administrator\Desktop\altai-dev\crm4sdk\crmsdk4.chm::/htm/v4d0_sp5420r_insertoptionvaluemessage.htm
		 */
		public static void CreatePicklistItem( MetadataService svc, string entityname, string propertyname, string name, int val ) {
			// Create a new picklist label.
			CrmLabel newCrmLabel = new CrmLabel();
			CrmNumber langCode = new CrmNumber();
			LocLabel englishLabel = new LocLabel();

			// Use US English language code.
			langCode.Value = 1033;
			englishLabel.LanguageCode = langCode;
			englishLabel.Label = name;
			newCrmLabel.LocLabels = new LocLabel[] { englishLabel };

			// Create a request.
			InsertOptionValueRequest insertRequest = new InsertOptionValueRequest();

			// Insert new label text into an existing picklist.
			insertRequest.AttributeLogicalName = propertyname;
			insertRequest.EntityLogicalName = entityname;
			insertRequest.Label = newCrmLabel;
			insertRequest.Value = new CrmNumber();

			insertRequest.Value.Value = val;

			// Send the message.
			InsertOptionValueResponse insertResponse = ( InsertOptionValueResponse )svc.Execute( insertRequest );
		}

		/**
		 * Create metadata from spec
		 */
		public static EntityMetadata CreateEntityMetadata4( EntityMetadataSpec spec ) {

			
			EntityMetadata meta = new EntityMetadata();

			// TODO: here we allow unadorned entity name so that we can cusomize existing 
			// built-ins by specifying them. This means when creating a new altai_ entity
			// we'll need to specify the full name including the prefix. Not so bad since 
			// we only have to do it once at the top.
			meta.SchemaName = spec.Name;
			meta.LogicalName = spec.Name;
			meta.PrimaryField = ApplyPrefix( spec.NamingPrefix, spec.PrimaryAttribute );
			meta.DisplayName = new CrmLabel(
				new LocLabel[] { new LocLabel( spec.Name, new CrmNumber( 1033 ) ) }, new LocLabel( spec.Name, new CrmNumber( 1033 ) )
			);
			meta.DisplayCollectionName = new CrmLabel(
				new LocLabel[] { new LocLabel( spec.Name, new CrmNumber( 1033 ) ) }, new LocLabel( spec.Name, new CrmNumber( 1033 ) )
			);
			meta.OwnershipType = new CrmOwnershipTypes();
			meta.OwnershipType.Value = OwnershipTypes.UserOwned;

			/**
			 * Note that CRM doesn't seem to add any attributes when creating a new entity. I think we mmight need 'update'
			 */
			List<AttributeMetadata> attributes = new List<AttributeMetadata>();

			// todo: commented line is the more general case ... should be using this somehow
			// List<RelationshipMetadata> relationships = new List<RelationshipMetadata>();
			List<CreateOneToManyRequest> relationships = new List<CreateOneToManyRequest>();

			foreach( AttributeMetadataSpec aspec in spec.Attributes ) {
				switch( aspec.Type ) { 
					case "String":
						StringAttributeMetadata sameta = CreateAttributeMetadata<StringAttributeMetadata>( spec, aspec );
						attributes.Add( sameta );
						break;
					case "Picklist":
						PicklistAttributeMetadata plmeta = CreateAttributeMetadata<PicklistAttributeMetadata>( spec, aspec );
						attributes.Add( plmeta );
						break;
					case "DateTime":
						DateTimeAttributeMetadata dtmeta = CreateAttributeMetadata<DateTimeAttributeMetadata>( spec, aspec );
						attributes.Add( dtmeta );
						break;
					case "PartyList":
						// TODO: no real idea about how this should work
						LookupAttributeMetadata partymeta = CreateAttributeMetadata<LookupAttributeMetadata>( spec, aspec );
						attributes.Add( partymeta );
						break;

					case "Lookup":
						
					// doing lookups requires a different service call.. we need to redesign to support this
						/*
						LookupAttributeMetadata lameta = CreateAttributeMetadata<LookupAttributeMetadata>( spec, aspec );

						OneToManyMetadata lmeta = new OneToManyMetadata();
						
						
						// The account entity will be the 'one' in this one-to-many relationship
						lmeta.ReferencedEntity = spec.Name;

						// The campaign entity will be the 'many' in this one-to-many relationship
						lmeta.ReferencingEntity = aspec.ReferencedEntity;

						// Set the metadata properties
						lmeta.SchemaName = aspec.Name;

						CreateOneToManyRequest req = new CreateOneToManyRequest();
						req.OneToManyRelationship = lmeta;
						req.Lookup = lameta;
						relationships.Add( req );
						 */
						break;
					case "Boolean":
						BooleanAttributeMetadata bmeta = CreateAttributeMetadata<BooleanAttributeMetadata>( spec, aspec );

						// NOTE: setting attribute type does not seem to be required, but it is included in the sdk samples
						// bmeta.AttributeType.Value = AttributeType.Boolean;

						// Set extended properties.
						bmeta.FalseOption = new Option();
						bmeta.FalseOption.Label = CreateCrmLabel( "False" );
						bmeta.FalseOption.Value = new CrmNumber();
						bmeta.FalseOption.Value.Value = 0;
						bmeta.TrueOption = new Option();
						bmeta.TrueOption.Label = CreateCrmLabel( "True" );
						bmeta.TrueOption.Value = new CrmNumber();
						bmeta.TrueOption.Value.Value = 1;


						attributes.Add( bmeta );
						break;
					case "Money":
						MoneyAttributeMetadata mmeta = CreateAttributeMetadata<MoneyAttributeMetadata>( spec, aspec );
						attributes.Add( mmeta );
						break;
					default:
						throw new NotImplementedException( "Creating entity metadata of type " + aspec.Type + " not impemented" );
				}
			}
			meta.Attributes = attributes.ToArray();

			return meta;
		}

		/**
		 * Create a crm lable assuming en_US localization (1033)
		 */
		private static CrmLabel CreateCrmLabel( string name ) {
			CrmLabel label = new CrmLabel(
				new LocLabel[] { new LocLabel( name, new CrmNumber( 1033 ) ) }, new LocLabel( name, new CrmNumber( 1033 ) )
			);
			return label;
		}

		private static T CreateAttributeMetadata<T>( EntityMetadataSpec spec, AttributeMetadataSpec aspec ) where T : AttributeMetadata, new() {
			T ameta = new T();
			
			if( typeof( T ) == typeof( StringAttributeMetadata ) ) {
				StringAttributeMetadata sameta = ameta as StringAttributeMetadata;
				sameta.MaxLength = new CrmNumber( 100 );
			}
			
			ameta.DisplayName = new CrmLabel(
				new LocLabel[] { new LocLabel( aspec.Name, new CrmNumber( 1033 ) ) }, new LocLabel( aspec.Name, new CrmNumber( 1033 ) )
			);
			ameta.SchemaName = ApplyPrefix( spec.NamingPrefix, aspec.Name );
			ameta.LogicalName = ApplyPrefix( spec.NamingPrefix, aspec.Name );

			ameta.IsCustomField = new CrmBoolean( true );
			return ameta;
		}

		/**
		 * create a new entity from the given metadata in CRM 
		 */
		public static void ExecuteMetadataRequest( EntityMetadata meta, MetadataService in_service ) {
			CreateEntityRequest req = new CreateEntityRequest();

			// supposedly we need to set the primary field. ..  not sure if this is totally necessary
			foreach( AttributeMetadata sa in meta.Attributes ) {
				if( sa.SchemaName == meta.PrimaryField ) {
					req.PrimaryAttribute = ( StringAttributeMetadata )sa;
				}
			}

			req.Entity = meta;
			try {
				in_service.Execute( req );
			}
			catch( System.Web.Services.Protocols.SoapException e ) {
				Console.WriteLine( e.Detail.InnerText );
			}
			// note that attributes must be added one at a time, can't be added
			// along with creation of entity.
			foreach( AttributeMetadata amd in meta.Attributes ) {
				CreateAttributeRequest ureq = new CreateAttributeRequest ();
				ureq.Attribute = amd;
				ureq.EntityName = meta.SchemaName;
				try {
					in_service.Execute( ureq );
				}
				catch( System.Web.Services.Protocols.SoapException e ) {
					Console.WriteLine( e.Detail.InnerText );
				}
			}
		}

		/*
		public static void CreateEntity2011( IOrganizationService ms_crmservice, string in_name, string in_primaryfieldname ) {

			CreateEntityRequest req = new CreateEntityRequest();
			EntityMetadata meta = new EntityMetadata();


			meta.SchemaName = in_name;

			meta.DisplayName = new Label( in_name, 1033 );


			meta.DisplayCollectionName = new Label( in_name + "s", 1033 );

			meta.OwnershipType = OwnershipTypes.UserOwned;

			StringAttributeMetadata sameta = new StringAttributeMetadata();
			sameta.MaxLength = 100;
			sameta.SchemaName = in_primaryfieldname;
			req.PrimaryAttribute = sameta;
			req.Entity = meta;
			ms_crmservice.Execute( req );
		}
		*/
	}
}
