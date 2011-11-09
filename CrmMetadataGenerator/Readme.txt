# example json spec.

{ "Name": "gen_contact", "NamingPrefix": "dan", "PrimaryAttribute": "firstname", "Attributes": [
	{ "Name": "honorific", "Type": "Picklist" },
	{ "Name": "firstname", "Type": "String" },
	{ "Name": "birthdate", "Type": "DateTime" }
] }

somen notes on the format:

The name of the entity must be prefixed. The naming prefix does not apply to the entity name. We do 
this in order to enable us to extend existing entities. Eg, we can specify 'contact' as the name of 
the entity in order to extend it by adding new fields. THis means that in order to create a new entity
we must give it a name in the form of prefix_entityname. CRM4 requires that custom entities be prefixed.

The NamingPrefix field prefixes all attribute names so that we can avoid having to change the prefix
manually for all field names. We trade off a bit of flexibility here.

The PrimaryAttribute field is required by CRM4. One of the given fields must be the primary attribute
when creating a new entity.

