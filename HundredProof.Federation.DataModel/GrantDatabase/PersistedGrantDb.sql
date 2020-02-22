CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

CREATE TABLE "DeviceCodes" (
    "UserCode" NVARCHAR(200) NOT NULL CONSTRAINT "PK_DeviceCodes" PRIMARY KEY,
    "DeviceCode" NVARCHAR(200) NOT NULL,
    "SubjectId" NVARCHAR(200) NULL,
    "ClientId" NVARCHAR(200) NOT NULL,
    "CreationTime" DATETIME NOT NULL,
    "Expiration" DATETIME NOT NULL,
    "Data" TEXT NOT NULL
);

CREATE TABLE "PersistedGrants" (
    "Key" NVARCHAR(200) NOT NULL CONSTRAINT "PK_PersistedGrants" PRIMARY KEY,
    "Type" NVARCHAR(50) NOT NULL,
    "SubjectId" NVARCHAR(200) NULL,
    "ClientId" NVARCHAR(200) NOT NULL,
    "CreationTime" DATETIME NOT NULL,
    "Expiration" DATETIME NULL,
    "Data" TEXT NOT NULL
);

CREATE UNIQUE INDEX "IX_DeviceCodes_DeviceCode" ON "DeviceCodes" ("DeviceCode");

CREATE INDEX "IX_DeviceCodes_Expiration" ON "DeviceCodes" ("Expiration");

CREATE INDEX "IX_PersistedGrants_Expiration" ON "PersistedGrants" ("Expiration");

CREATE INDEX "IX_PersistedGrants_SubjectId_ClientId_Type" ON "PersistedGrants" ("SubjectId", "ClientId", "Type");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20190927145451_Grants', '3.0.0');

