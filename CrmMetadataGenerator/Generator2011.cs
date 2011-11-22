using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;

// effectively exclude this file if we are building for CRM4
#if ! CRM4

// attempt at mapping things so we can use the same code for crm 4 and 2011
#if CRM4
using Microsoft.Crm.SdkTypeProxy.Metadata;
using Microsoft.Crm.Sdk.Metadata;
using MDService = Microsoft.Crm.SdkTypeProxy.Metadata.MetadataService;
using LocalizedLabel = Microsoft.Crm.Sdk.LocLabel;
using Label = Microsoft.Crm.Sdk.CrmLabel;
using AttributeTypeCode = Microsoft.Crm.Sdk.Metadata.CrmAttributeType;
using OneToManyMetadata = Microsoft.Crm.Sdk.Metadata.OneToManyMetadata;
using CrmAssociatedMenuBehavior = Microsoft.Crm.Sdk.Metadata.AssociatedMenuBehavior;
#else
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using Microsoft.Xrm.Sdk.Metadata;
using MDService = Microsoft.Xrm.Sdk.IOrganizationService;
using CrmNumber = System.Int32;
#endif



namespace CrmMetadataGenerator
{
	class Generator2011
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
		 * TODO: special note - we have to publish entity after we add these values before
		 * they can be used.
		 */
		public static void CreatePickListValues( EntityMetadataSpec spec, MDService service ) { 
			foreach( AttributeMetadataSpec attribute in spec.Attributes ) {
				if( attribute.Type == "Picklist" ) {
					foreach( PicklistMetadata picklist in attribute.Values ) {
						try {
							Console.WriteLine( "Creating Picklist Item " + attribute.Name + " " + picklist.Name );
							CreatePicklistItem( service, spec.Name, Generator2011.ApplyPrefix( spec.NamingPrefix, attribute.Name ), picklist.Name, picklist.Value );
						}
						catch( System.Web.Services.Protocols.SoapException e ) {
							Console.WriteLine( e.Detail.InnerText );
						}
						catch( System.ServiceModel.FaultException fe ) {
							Console.WriteLine( fe.Message );
						}
					}
				}
			}
		}
		/**
		 * Create a picklist item in an existing picklist - code adapted from 4.0 SDK documentation
		 * mk:@MSITStore:C:\Users\Administrator\Desktop\altai-dev\crm4sdk\crmsdk4.chm::/htm/v4d0_sp5420r_insertoptionvaluemessage.htm
		 */
		public static void CreatePicklistItem( MDService svc, string entityname, string propertyname, string name, int val ) {
			
			// Create a request.
			InsertOptionValueRequest insertRequest = new InsertOptionValueRequest();

			// Insert new label text into an existing picklist.
			insertRequest.AttributeLogicalName = propertyname;
			insertRequest.EntityLogicalName = entityname;
			insertRequest.Label = CreateCrmLabel( name );
			insertRequest.Value = val;

			// Send the message.
			InsertOptionValueResponse insertResponse = ( InsertOptionValueResponse )svc.Execute( insertRequest );
		}

		public static void CreateLookups( EntityMetadataSpec spec, MDService service ) { 
			foreach( AttributeMetadataSpec aspec in spec.Attributes ) { 
				if( aspec.Type == "PartyList" || aspec.Type == "Lookup" ) {
					
					LookupAttributeMetadata lameta = CreateAttributeMetadata<LookupAttributeMetadata>( spec, aspec );
						
					// lameta.SchemaName = "new_foo_bar";
						
					// display name is set by CreateAttributeMetadata
					lameta.DisplayName = CreateCrmLabel( "Lookup" );
						
					lameta.RequiredLevel = new AttributeRequiredLevelManagedProperty( AttributeRequiredLevel.None );
					lameta.RequiredLevel.Value = AttributeRequiredLevel.None;
					// has to have a description. otherwise "Generic SQL error
					lameta.Description = CreateCrmLabel( "default description" );
						



					OneToManyRelationshipMetadata lmeta = new OneToManyRelationshipMetadata();
						
						
					// The account entity will be the 'one' in this one-to-many relationship
					lmeta.ReferencedEntity = aspec.ReferencedEntity;

					// The campaign entity will be the 'many' in this one-to-many relationship
					lmeta.ReferencingEntity = spec.Name;

					// Set the metadata properties
					lmeta.SchemaName = ApplyPrefix( spec.NamingPrefix, aspec.ReferencedEntity + "_" + spec.Name );
						

					CreateOneToManyRequest req = new CreateOneToManyRequest();
					OneToManyRelationshipMetadata oneToManyRelationship = lmeta;
					req.OneToManyRelationship = lmeta;
					req.Lookup = lameta;

					ExecuteOneToManyRequest( req, service );
					// TODO: do we add this attribute as well?
				}			
					
			}
		
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
			meta.DisplayName = CreateCrmLabel( spec.Name );
			meta.DisplayCollectionName = CreateCrmLabel( spec.Name );
			meta.OwnershipType = OwnershipTypes.UserOwned;

			typeof( EntityMetadata )
				.GetProperty( "PrimaryNameAttribute" )
				.SetValue( meta, ApplyPrefix( spec.NamingPrefix, spec.PrimaryAttribute ), null );
			
			

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
						// option values in 2011 have to be specified at the time of creation, rather than afterward
						OptionSetMetadata optionset = new OptionSetMetadata() { IsGlobal = false, OptionSetType = OptionSetType.Picklist };
						foreach( PicklistMetadata optionsetmetadata in aspec.Values ) { 
							optionset.Options.Add(
								new OptionMetadata( CreateCrmLabel( optionsetmetadata.Name ), optionsetmetadata.Value )
							);
						}

						plmeta.OptionSet = optionset;
						attributes.Add( plmeta );
						break;
					case "DateTime":
						DateTimeAttributeMetadata dtmeta = CreateAttributeMetadata<DateTimeAttributeMetadata>( spec, aspec );
						attributes.Add( dtmeta );
						break;
					case "PartyList":
						// TODO: no real idea about how this should work
						break;

					case "Boolean":
						BooleanAttributeMetadata bmeta = CreateAttributeMetadata<BooleanAttributeMetadata>( spec, aspec );

						// NOTE: setting attribute type does not seem to be required, but it is included in the sdk samples
						// bmeta.AttributeType.Value = AttributeType.Boolean;

						// Set extended properties.
						bmeta.OptionSet = new BooleanOptionSetMetadata();
						bmeta.OptionSet.FalseOption = new OptionMetadata();
						bmeta.OptionSet.FalseOption.Label = CreateCrmLabel( "False" );
						bmeta.OptionSet.FalseOption.Value = 0;
						bmeta.OptionSet.TrueOption = new OptionMetadata();
						bmeta.OptionSet.TrueOption.Label = CreateCrmLabel( "True" );
						bmeta.OptionSet.TrueOption.Value = 1;


						attributes.Add( bmeta );
						break;
					case "Money":
						MoneyAttributeMetadata mmeta = CreateAttributeMetadata<MoneyAttributeMetadata>( spec, aspec );
						attributes.Add( mmeta );
						break;
					case "Lookup":
						// ignore lookups here, we create them in another step
						break;
					default:
						throw new NotImplementedException( "Creating entity metadata of type " + aspec.Type + " not impemented" );
				}
			}
			typeof( EntityMetadata )
				.GetProperty( "Attributes" )
				.SetValue( meta, attributes.ToArray(), null );
			//meta.Attributes = attributes.ToArray();

			return meta;
		}

		/**
		 * Create a crm lable assuming en_US localization (1033)
		 */
		private static Label CreateCrmLabel( string name ) {
			Label label = new Label(
				new LocalizedLabel( name, 1033 ), new LocalizedLabel[] { new LocalizedLabel( name, 1033 ) }
			);
			return label;
		}

		private static T CreateAttributeMetadata<T>( EntityMetadataSpec spec, AttributeMetadataSpec aspec ) where T : AttributeMetadata, new() {
			T ameta = new T();
			
			if( typeof( T ) == typeof( StringAttributeMetadata ) ) {
				StringAttributeMetadata sameta = ameta as StringAttributeMetadata;
				sameta.MaxLength = 100;
			}

			ameta.DisplayName = CreateCrmLabel( aspec.Name );
			ameta.SchemaName = ApplyPrefix( spec.NamingPrefix, aspec.Name );
			ameta.LogicalName = ApplyPrefix( spec.NamingPrefix, aspec.Name );

			// ameta.IsCustomAttribute = true;
			return ameta;
		}

		/**
		 * Call CRM to create entity attributes
		 */
		public static void ExecuteAttributeMetadataRequest( EntityMetadata meta, MDService in_service ) {
			// note that attributes must be added one at a time, can't be added
			// along with creation of entity.
			foreach( AttributeMetadata amd in meta.Attributes ) {
				if( amd.SchemaName == meta.PrimaryNameAttribute ) {
					continue;
				}
				CreateAttributeRequest ureq = new CreateAttributeRequest();
				ureq.Attribute = amd;
				ureq.EntityName = meta.SchemaName;
				try {
					Console.WriteLine( "Creating Attribute " + amd.LogicalName );
					in_service.Execute( ureq );
				}
				catch( System.Web.Services.Protocols.SoapException e ) {
					Console.WriteLine( e.Detail.InnerText );
				}
				catch( System.ServiceModel.FaultException fe ) {
					Console.WriteLine( fe.Message );
				}
			}
		}

		/**
		* Call CRM to create entity attributes
		*/
		public static void ExecuteOneToManyRequest( CreateOneToManyRequest req, MDService in_service ) {
			try {
				Console.WriteLine( "Creating Reference " + req.OneToManyRelationship.SchemaName );
				in_service.Execute( req );
			}
			catch( System.Web.Services.Protocols.SoapException e ) {
				Console.WriteLine( e.Detail.InnerText );
			}
			catch( System.ServiceModel.FaultException fe ) {
				Console.WriteLine( fe.Message );
			}
		}

		/**
		 * create a new entity from the given metadata in CRM 
		 */
		public static void ExecuteMetadataRequest( EntityMetadata meta, MDService in_service ) {
			CreateEntityRequest req = new CreateEntityRequest();

			// supposedly we need to set the primary field. ..  not sure if this is totally necessary
			foreach( AttributeMetadata sa in meta.Attributes ) {
				if( sa.SchemaName == meta.PrimaryNameAttribute ) {
					req.PrimaryAttribute = ( StringAttributeMetadata )sa;
				}
			}

			req.Entity = meta;
			try {
				Console.WriteLine( "Creating Entity " + meta.SchemaName );
				in_service.Execute( req );
			}
			catch( System.Web.Services.Protocols.SoapException e ) {
				Console.WriteLine( e.Detail.InnerText );
			}
			catch( System.ServiceModel.FaultException fe ) {
				Console.WriteLine( fe.Message );
			}
		}
	}
}

// the matching if directive excludes the file if we build for CRM4
#endif