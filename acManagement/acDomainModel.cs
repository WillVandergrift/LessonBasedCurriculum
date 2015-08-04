﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Common;
using System.Collections.Generic;
using Telerik.OpenAccess;
using Telerik.OpenAccess.Metadata;
using acManagement;


namespace acManagement	
{	
	public partial class acDbContext : OpenAccessContext
	{
		private static string connectionStringName = @"AC_ManagementConnection";
			
		private static BackendConfiguration backend = GetBackendConfiguration();
		
			
		private static MetadataSource metadataSource = XmlMetadataSource.FromAssemblyResource("acDomainModel.rlinq");
	
		public acDbContext()
			:base(connectionStringName, backend, metadataSource)
		{ }
		
		public acDbContext(string connection)
			:base(connection, backend, metadataSource)
		{ }
	
		public acDbContext(BackendConfiguration backendConfiguration)
			:base(connectionStringName, backendConfiguration, metadataSource)
		{ }
			
		public acDbContext(string connection, MetadataSource metadataSource)
			:base(connection, backend, metadataSource)
		{ }
		
		public acDbContext(string connection, BackendConfiguration backendConfiguration, MetadataSource metadataSource)
			:base(connection, backendConfiguration, metadataSource)
		{ }
			
		public IQueryable<LbcExpectation> LbcExpectations 
		{
	    	get
	    	{
	        	return this.GetAll<LbcExpectation>();
	    	}
		}
		
		public IQueryable<LbcExpectationIndex> LbcExpectationIndices 
		{
	    	get
	    	{
	        	return this.GetAll<LbcExpectationIndex>();
	    	}
		}
		
		public IQueryable<LbcDistrict> LbcDistricts 
		{
	    	get
	    	{
	        	return this.GetAll<LbcDistrict>();
	    	}
		}
		
		public static BackendConfiguration GetBackendConfiguration()
		{
			BackendConfiguration backend = new BackendConfiguration();
			backend.Backend = "mssql";
			return backend;
		}
	}
}
#pragma warning restore 1591
