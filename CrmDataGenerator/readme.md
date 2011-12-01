# CrmDataGenerator

# About

The CRM data generator is a tool designed to allow easy data import into Microsoft CRM installations regardless of the server version. Data is supplied using a file containing a JSON array of entity values which are parsed and inserted into CRM using the CRM Web services API.

# Usage

    c:\> CrmDataGenerator <orgname> <data.json> <xmlconfig>

Orgname is the name of the CRM organization, data.json is a json formatted data file that is used for the data import, and xmlconfig is a standard .NET configuration section for Altai.commons. If this configuration is not given on the commandline, the configuration present in the tool installation directory will be used instead.

# Sample usage

    c:\> CrmDataGenerator testorg data.json crm.config

bizorg is not used in the case of CRM 2011 but there must be a placeholder argument. The commandline tool still expects there to be three arguments.

### JSON file format

data.json is of the form defined below:


    [ 
        { 
            "Name": "contact", 
            "NamingPrefix": "org",
            "Properties": [
                { 
                    "Name": "firstname", 
                    "Type": "String", 
                    "Value": "Dan" 
                },
            ] 
        },
    ]

The data must be contained within an array. File must be valid JSON meaning all identifiers must be double-quoted. Comments are not supported within the JSON file. This is a limitation of JSON.

Property types are string representations of types found in EntityWrap.AttributeTypeCode. The actual enum type is not used however.

Naming convention uses NamingPrefix applied to the properties but not the entity name. For example given NamingPrefix of `org` and the property type `firstname`, the resulting property name will be `org_firstname`.

### Config file format

crm.config looks like the following in the case of CRM4:

	<?xml version="1.0"?>
	<configuration>
	  <configSections>
	    <section name="Altai.MSCrm" type="Altai.MSCrm.MSCrmConfigurationSection,Altai.Commons.Crm4"/>
	  </configSections>
	
	  <Altai.MSCrm
	    crmServiceUrl="http://localhost:5555/MSCRMServices/2007/CrmService.asmx"
	    credentialUsername="user"
	    credentialPassword="pass"
	    credentialDomain="domain"
	    />
	</configuration>

and in the case of CRM 2011:

	<?xml version="1.0"?>
	<configuration>
	  <configSections>
		<section name="Altai.MSCrm" type="Altai.MSCrm.MSCrm5ConfigurationSection,Altai.Commons.Crm2011"/>
	  </configSections>
	
	  <Altai.MSCrm
	    crmServiceUrl="http://localhost:5555/testorg/XRMServices/2011/Organization.svc"
	    credentialUsername="user"
	    credentialPassword="pass"
	    credentialDomain="domain"
	  />

	</configuration>

# Building

Building CRM4 version requires the CRM4 variable to be defined. This is not currently done in any build script, so it must be changed in the project file. By default, the CRM2011 configuration will be built.

# Further work

* Not all data types are supported.
* Handling of optional arguments is broken. Need to be able to handle all argument scenarios.