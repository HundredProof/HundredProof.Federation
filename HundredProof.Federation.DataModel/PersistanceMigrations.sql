IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ApiResources' and xtype='U')
	BEGIN
	CREATE TABLE "ApiResources" (
		"Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResources" PRIMARY KEY IDENTITY(1,1),
		"Enabled" BIT NOT NULL,
		"Name" NVARCHAR(200) NOT NULL,
		"DisplayName" NVARCHAR(200) NULL,
		"Description" NVARCHAR(1000) NULL,
		"Created" DATETIME NOT NULL,
		"Updated" DATETIME NULL,
		"LastAccessed" DATETIME NULL,
		"NonEditable" BIT NOT NULL
	)
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='Clients' and xtype='U')
	BEGIN
		CREATE TABLE "Clients" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Clients" PRIMARY KEY IDENTITY(1,1),
    "Enabled" BIT NOT NULL,
    "ClientId" NVARCHAR(200) NOT NULL,
    "ProtocolType" NVARCHAR(200) NOT NULL,
    "RequireClientSecret" BIT NOT NULL,
    "ClientName" NVARCHAR(200) NULL,
    "Description" NVARCHAR(1000) NULL,
    "ClientUri" NVARCHAR(2000) NULL,
    "LogoUri" NVARCHAR(2000) NULL,
    "RequireConsent" BIT NOT NULL,
    "AllowRememberConsent" BIT NOT NULL,
    "AlwaysIncludeUserClaimsInIdToken" BIT NOT NULL,
    "RequirePkce" BIT NOT NULL,
    "AllowPlainTextPkce" BIT NOT NULL,
    "AllowAccessTokensViaBrowser" BIT NOT NULL,
    "FrontChannelLogoutUri" NVARCHAR(2000) NULL,
    "FrontChannelLogoutSessionRequired" BIT NOT NULL,
    "BackChannelLogoutUri" NVARCHAR(2000) NULL,
    "BackChannelLogoutSessionRequired" BIT NOT NULL,
    "AllowOfflineAccess" BIT NOT NULL,
    "IdentityTokenLifetime" INTEGER NOT NULL,
    "AccessTokenLifetime" INTEGER NOT NULL,
    "AuthorizationCodeLifetime" INTEGER NOT NULL,
    "ConsentLifetime" INTEGER NULL,
    "AbsoluteRefreshTokenLifetime" INTEGER NOT NULL,
    "SlidingRefreshTokenLifetime" INTEGER NOT NULL,
    "RefreshTokenUsage" INTEGER NOT NULL,
    "UpdateAccessTokenClaimsOnRefresh" INTEGER NOT NULL,
    "RefreshTokenExpiration" INTEGER NOT NULL,
    "AccessTokenType" INTEGER NOT NULL,
    "EnableLocalLogin" BIT NOT NULL,
    "IncludeJwtId" BIT NOT NULL,
    "AlwaysSendClientClaims" BIT NOT NULL,
    "ClientClaimsPrefix" NVARCHAR(200) NULL,
    "PairWiseSubjectSalt" NVARCHAR(200) NULL,
    "Created" DATETIME NOT NULL,
    "Updated" DATETIME NULL,
    "LastAccessed" DATETIME NULL,
    "UserSsoLifetime" INTEGER NULL,
    "UserCodeType" NVARCHAR(100) NULL,
    "DeviceCodeLifetime" INTEGER NOT NULL,
    "NonEditable" BIT NOT NULL
)
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='IdentityResources' and xtype='U')
	BEGIN
		CREATE TABLE "IdentityResources" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityResources" PRIMARY KEY IDENTITY(1,1),
    "Enabled" BIT NOT NULL,
    "Name" NVARCHAR(200) NOT NULL,
    "DisplayName" NVARCHAR(200) NULL,
    "Description" NVARCHAR(1000) NULL,
    "Required" BIT NOT NULL,
    "Emphasize" BIT NOT NULL,
    "ShowInDiscoveryDocument" BIT NOT NULL,
    "Created" DATETIME NOT NULL,
    "Updated" DATETIME NULL,
    "NonEditable" BIT NOT NULL
)
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ApiClaims' and xtype='U')
	BEGIN
		CREATE TABLE "ApiClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiClaims" PRIMARY KEY IDENTITY(1,1),
    "Type" NVARCHAR(200) NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiClaims_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ApiProperties' and xtype='U')
	BEGIN
		CREATE TABLE "ApiProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiProperties" PRIMARY KEY IDENTITY(1,1),
    "Key" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(2000) NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiProperties_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ApiScopes' and xtype='U')
	BEGIN
		CREATE TABLE "ApiScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopes" PRIMARY KEY IDENTITY(1,1),
    "Name" NVARCHAR(200) NOT NULL,
    "DisplayName" NVARCHAR(200) NULL,
    "Description" NVARCHAR(1000) NULL,
    "Required" BIT NOT NULL,
    "Emphasize" BIT NOT NULL,
    "ShowInDiscoveryDocument" INTEGER NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiScopes_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ApiSecrets' and xtype='U')
	BEGIN
		CREATE TABLE "ApiSecrets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiSecrets" PRIMARY KEY IDENTITY(1,1),
    "Description" NVARCHAR(1000) NULL,
    "Value" NVARCHAR(4000) NOT NULL,
    "Expiration" NVARCHAR(250) NULL,
    "Type" NVARCHAR(4000) NOT NULL,
    "Created" DATETIME NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiSecrets_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientClaims' and xtype='U')
	BEGIN
		CREATE TABLE "ClientClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientClaims" PRIMARY KEY IDENTITY(1,1),
    "Type" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(250) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientClaims_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientCorsOrigins' and xtype='U')
	BEGIN
		CREATE TABLE "ClientCorsOrigins" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientCorsOrigins" PRIMARY KEY IDENTITY(1,1),
    "Origin" NVARCHAR(150) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientCorsOrigins_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientGrantTypes' and xtype='U')
	BEGIN
		CREATE TABLE "ClientGrantTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientGrantTypes" PRIMARY KEY IDENTITY(1,1),
    "GrantType" NVARCHAR(250) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientGrantTypes_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientIdPRestrictions' and xtype='U')
	BEGIN
		CREATE TABLE "ClientIdPRestrictions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientIdPRestrictions" PRIMARY KEY IDENTITY(1,1),
    "Provider" NVARCHAR(200) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientIdPRestrictions_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientPostLogoutRedirectUris' and xtype='U')
	BEGIN
		CREATE TABLE "ClientPostLogoutRedirectUris" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientPostLogoutRedirectUris" PRIMARY KEY IDENTITY(1,1),
    "PostLogoutRedirectUri" NVARCHAR(2000) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientPostLogoutRedirectUris_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientProperties' and xtype='U')
	BEGIN
		CREATE TABLE "ClientProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientProperties" PRIMARY KEY IDENTITY(1,1),
    "Key" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(2000) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientProperties_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientRedirectUris' and xtype='U')
	BEGIN
		CREATE TABLE "ClientRedirectUris" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientRedirectUris" PRIMARY KEY IDENTITY(1,1),
    "RedirectUri" NVARCHAR(2000) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientRedirectUris_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientScopes' and xtype='U')
	BEGIN
		CREATE TABLE "ClientScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientScopes" PRIMARY KEY IDENTITY(1,1),
    "Scope" NVARCHAR(200) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientScopes_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ClientSecrets' and xtype='U')
	BEGIN
		CREATE TABLE "ClientSecrets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientSecrets" PRIMARY KEY IDENTITY(1,1),
    "Description" NVARCHAR(2000) NULL,
    "Value" NVARCHAR(4000) NOT NULL,
    "Expiration" DATETIME NULL,
    "Type" NVARCHAR(250) NOT NULL,
    "Created" DATETIME NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientSecrets_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='IdentityClaims' and xtype='U')
	BEGIN
		CREATE TABLE "IdentityClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityClaims" PRIMARY KEY IDENTITY(1,1),
    "Type" NVARCHAR(200) NOT NULL,
    "IdentityResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_IdentityClaims_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "IdentityResources" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='IdentityProperties' and xtype='U')
	BEGIN
		CREATE TABLE "IdentityProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityProperties" PRIMARY KEY IDENTITY(1,1),
    "Key" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(2000) NOT NULL,
    "IdentityResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_IdentityProperties_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "IdentityResources" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='ApiScopeClaims' and xtype='U')
	BEGIN
		CREATE TABLE "ApiScopeClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopeClaims" PRIMARY KEY IDENTITY(1,1),
    "Type" NVARCHAR(200) NOT NULL,
    "ApiScopeId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiScopeClaims_ApiScopes_ApiScopeId" FOREIGN KEY ("ApiScopeId") REFERENCES "ApiScopes" ("Id") ON DELETE CASCADE
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='DeviceCodes' and xtype='U')
	BEGIN
		CREATE TABLE "DeviceCodes" (
    "UserCode" NVARCHAR(200) NOT NULL CONSTRAINT "PK_DeviceCodes" PRIMARY KEY,
    "DeviceCode" NVARCHAR(200) NOT NULL,
    "SubjectId" NVARCHAR(200) NULL,
    "ClientId" NVARCHAR(200) NOT NULL,
    "CreationTime" DATETIME NOT NULL,
    "Expiration" DATETIME NOT NULL,
    "Data" TEXT NOT NULL
);
	END
IF NOT EXISTS (SELECT name FROM sysobjects WHERE name='PersistedGrants' and xtype='U')
	BEGIN
		CREATE TABLE "PersistedGrants" (
    "Key" NVARCHAR(200) NOT NULL CONSTRAINT "PK_PersistedGrants" PRIMARY KEY,
    "Type" NVARCHAR(50) NOT NULL,
    "SubjectId" NVARCHAR(200) NULL,
    "ClientId" NVARCHAR(200) NOT NULL,
    "CreationTime" DATETIME NOT NULL,
    "Expiration" DATETIME NULL,
    "Data" TEXT NOT NULL
)
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiClaims_ApiResourceId')
	BEGIN
		CREATE INDEX "IX_ApiClaims_ApiResourceId" ON "ApiClaims" ("ApiResourceId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiProperties_ApiResourceId')
	BEGIN
		CREATE INDEX "IX_ApiProperties_ApiResourceId" ON "ApiProperties" ("ApiResourceId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiResources_Name')
	BEGIN
		CREATE UNIQUE INDEX "IX_ApiResources_Name" ON "ApiResources" ("Name");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiScopeClaims_ApiScopeId')
	BEGIN
		CREATE INDEX "IX_ApiScopeClaims_ApiScopeId" ON "ApiScopeClaims" ("ApiScopeId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiScopes_ApiResourceId')
	BEGIN
		CREATE INDEX "IX_ApiScopes_ApiResourceId" ON "ApiScopes" ("ApiResourceId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiScopes_Name')
	BEGIN
		CREATE UNIQUE INDEX "IX_ApiScopes_Name" ON "ApiScopes" ("Name");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ApiSecrets_ApiResourceId')
	BEGIN
		CREATE INDEX "IX_ApiSecrets_ApiResourceId" ON "ApiSecrets" ("ApiResourceId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientClaims_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientClaims_ClientId" ON "ClientClaims" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientCorsOrigins_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientCorsOrigins_ClientId" ON "ClientCorsOrigins" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientGrantTypes_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientGrantTypes_ClientId" ON "ClientGrantTypes" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientIdPRestrictions_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientIdPRestrictions_ClientId" ON "ClientIdPRestrictions" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientPostLogoutRedirectUris_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientPostLogoutRedirectUris_ClientId" ON "ClientPostLogoutRedirectUris" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientProperties_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientProperties_ClientId" ON "ClientProperties" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientRedirectUris_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientRedirectUris_ClientId" ON "ClientRedirectUris" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_Clients_ClientId')
	BEGIN
		CREATE UNIQUE INDEX "IX_Clients_ClientId" ON "Clients" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientScopes_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientScopes_ClientId" ON "ClientScopes" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_ClientSecrets_ClientId')
	BEGIN
		CREATE INDEX "IX_ClientSecrets_ClientId" ON "ClientSecrets" ("ClientId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_IdentityClaims_IdentityResourceId')
	BEGIN
		CREATE INDEX "IX_IdentityClaims_IdentityResourceId" ON "IdentityClaims" ("IdentityResourceId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_IdentityProperties_IdentityResourceId')
	BEGIN
		CREATE INDEX "IX_IdentityProperties_IdentityResourceId" ON "IdentityProperties" ("IdentityResourceId");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_IdentityResources_Name')
	BEGIN
		CREATE UNIQUE INDEX "IX_IdentityResources_Name" ON "IdentityResources" ("Name");	
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DeviceCodes_DeviceCode')
	BEGIN
		CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON "DeviceCodes" ("DeviceCode");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_DeviceCodes_Expiration')
	BEGIN
		CREATE INDEX "IX_DeviceCodes_Expiration" ON "DeviceCodes" ("Expiration");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_PersistedGrants_Expiration')
	BEGIN
		CREATE INDEX "IX_PersistedGrants_Expiration" ON "PersistedGrants" ("Expiration");
	END
IF NOT EXISTS (SELECT name FROM sysindexes WHERE name = 'IX_PersistedGrants_SubjectId_ClientId_Type')
	BEGIN
		CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");
	END