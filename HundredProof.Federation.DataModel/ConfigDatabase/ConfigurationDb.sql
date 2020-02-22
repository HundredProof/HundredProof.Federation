CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "ApiResources" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiResources" PRIMARY KEY AUTOINCREMENT,
    "Enabled" BIT NOT NULL,
    "Name" NVARCHAR(200) NOT NULL,
    "DisplayName" NVARCHAR(200) NULL,
    "Description" NVARCHAR(1000) NULL,
    "Created" DATETIME NOT NULL,
    "Updated" DATETIME NULL,
    "LastAccessed" DATETIME NULL,
    "NonEditable" BOOL NOT NULL
);

CREATE TABLE "Clients" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Clients" PRIMARY KEY AUTOINCREMENT,
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
);

CREATE TABLE "IdentityResources" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityResources" PRIMARY KEY AUTOINCREMENT,
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
);

CREATE TABLE "ApiClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" NVARCHAR(200) NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiClaims_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(2000) NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiProperties_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopes" PRIMARY KEY AUTOINCREMENT,
    "Name" NVARCHAR(200) NOT NULL,
    "DisplayName" NVARCHAR(200) NULL,
    "Description" NVARCHAR(1000) NULL,
    "Required" BIT NOT NULL,
    "Emphasize" BIT NOT NULL,
    "ShowInDiscoveryDocument" INTEGER NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiScopes_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiSecrets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiSecrets" PRIMARY KEY AUTOINCREMENT,
    "Description" NVARCHAR(1000) NULL,
    "Value" NVARCHAR(4000) NOT NULL,
    "Expiration" NVARCHAR(250) NULL,
    "Type" NVARCHAR(4000) NOT NULL,
    "Created" DATETIME NOT NULL,
    "ApiResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiSecrets_ApiResources_ApiResourceId" FOREIGN KEY ("ApiResourceId") REFERENCES "ApiResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(250) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientClaims_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientCorsOrigins" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientCorsOrigins" PRIMARY KEY AUTOINCREMENT,
    "Origin" NVARCHAR(150) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientCorsOrigins_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientGrantTypes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientGrantTypes" PRIMARY KEY AUTOINCREMENT,
    "GrantType" NVARCHAR(250) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientGrantTypes_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientIdPRestrictions" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientIdPRestrictions" PRIMARY KEY AUTOINCREMENT,
    "Provider" NVARCHAR(200) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientIdPRestrictions_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientPostLogoutRedirectUris" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientPostLogoutRedirectUris" PRIMARY KEY AUTOINCREMENT,
    "PostLogoutRedirectUri" NVARCHAR(2000) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientPostLogoutRedirectUris_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(2000) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientProperties_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientRedirectUris" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientRedirectUris" PRIMARY KEY AUTOINCREMENT,
    "RedirectUri" NVARCHAR(2000) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientRedirectUris_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientScopes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientScopes" PRIMARY KEY AUTOINCREMENT,
    "Scope" NVARCHAR(200) NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientScopes_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ClientSecrets" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ClientSecrets" PRIMARY KEY AUTOINCREMENT,
    "Description" NVARCHAR(2000) NULL,
    "Value" NVARCHAR(4000) NOT NULL,
    "Expiration" DATETIME NULL,
    "Type" NVARCHAR(250) NOT NULL,
    "Created" DATETIME NOT NULL,
    "ClientId" INTEGER NOT NULL,
    CONSTRAINT "FK_ClientSecrets_Clients_ClientId" FOREIGN KEY ("ClientId") REFERENCES "Clients" ("Id") ON DELETE CASCADE
);

CREATE TABLE "IdentityClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" NVARCHAR(200) NOT NULL,
    "IdentityResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_IdentityClaims_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "IdentityResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "IdentityProperties" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_IdentityProperties" PRIMARY KEY AUTOINCREMENT,
    "Key" NVARCHAR(250) NOT NULL,
    "Value" NVARCHAR(2000) NOT NULL,
    "IdentityResourceId" INTEGER NOT NULL,
    CONSTRAINT "FK_IdentityProperties_IdentityResources_IdentityResourceId" FOREIGN KEY ("IdentityResourceId") REFERENCES "IdentityResources" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ApiScopeClaims" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ApiScopeClaims" PRIMARY KEY AUTOINCREMENT,
    "Type" NVARCHAR(200) NOT NULL,
    "ApiScopeId" INTEGER NOT NULL,
    CONSTRAINT "FK_ApiScopeClaims_ApiScopes_ApiScopeId" FOREIGN KEY ("ApiScopeId") REFERENCES "ApiScopes" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_ApiClaims_ApiResourceId" ON "ApiClaims" ("ApiResourceId");

CREATE INDEX "IX_ApiProperties_ApiResourceId" ON "ApiProperties" ("ApiResourceId");

CREATE UNIQUE INDEX "IX_ApiResources_Name" ON "ApiResources" ("Name");

CREATE INDEX "IX_ApiScopeClaims_ApiScopeId" ON "ApiScopeClaims" ("ApiScopeId");

CREATE INDEX "IX_ApiScopes_ApiResourceId" ON "ApiScopes" ("ApiResourceId");

CREATE UNIQUE INDEX "IX_ApiScopes_Name" ON "ApiScopes" ("Name");

CREATE INDEX "IX_ApiSecrets_ApiResourceId" ON "ApiSecrets" ("ApiResourceId");

CREATE INDEX "IX_ClientClaims_ClientId" ON "ClientClaims" ("ClientId");

CREATE INDEX "IX_ClientCorsOrigins_ClientId" ON "ClientCorsOrigins" ("ClientId");

CREATE INDEX "IX_ClientGrantTypes_ClientId" ON "ClientGrantTypes" ("ClientId");

CREATE INDEX "IX_ClientIdPRestrictions_ClientId" ON "ClientIdPRestrictions" ("ClientId");

CREATE INDEX "IX_ClientPostLogoutRedirectUris_ClientId" ON "ClientPostLogoutRedirectUris" ("ClientId");

CREATE INDEX "IX_ClientProperties_ClientId" ON "ClientProperties" ("ClientId");

CREATE INDEX "IX_ClientRedirectUris_ClientId" ON "ClientRedirectUris" ("ClientId");

CREATE UNIQUE INDEX "IX_Clients_ClientId" ON "Clients" ("ClientId");

CREATE INDEX "IX_ClientScopes_ClientId" ON "ClientScopes" ("ClientId");

CREATE INDEX "IX_ClientSecrets_ClientId" ON "ClientSecrets" ("ClientId");

CREATE INDEX "IX_IdentityClaims_IdentityResourceId" ON "IdentityClaims" ("IdentityResourceId");

CREATE INDEX "IX_IdentityProperties_IdentityResourceId" ON "IdentityProperties" ("IdentityResourceId");

CREATE UNIQUE INDEX "IX_IdentityResources_Name" ON "IdentityResources" ("Name");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190927145500_Config', '3.0.0');

