﻿CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;

CREATE TABLE "Clients" (
    "Email" character varying(100) NOT NULL,
    "FirstName" character varying(50) NOT NULL,
    "LastName" character varying(50) NOT NULL,
    "PhoneNumber" character varying(15) NOT NULL,
    "DriverLicense" character varying(20) NOT NULL,
    "BirthDate" date NOT NULL,
    CONSTRAINT "PK_Clients" PRIMARY KEY ("Email")
);

CREATE TABLE "Offices" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Address" character varying(100) NOT NULL,
    "Name" character varying(50) NOT NULL,
    "PhoneNumber" character varying(15) NOT NULL,
    CONSTRAINT "PK_Offices" PRIMARY KEY ("ID")
);

CREATE TABLE "Vehicles" (
    "LicensePlate" character varying(20) NOT NULL,
    "Model" character varying(50) NOT NULL,
    "Year" integer NOT NULL,
    "FuelType" character varying(20) NOT NULL,
    "Mileage" integer NOT NULL,
    "CostPerDay" numeric(18,2) NOT NULL,
    "OfficeId" integer NOT NULL,
    CONSTRAINT "PK_Vehicles" PRIMARY KEY ("LicensePlate"),
    CONSTRAINT "FK_Vehicles_Offices_OfficeId" FOREIGN KEY ("OfficeId") REFERENCES "Offices" ("ID") ON DELETE CASCADE
);

CREATE TABLE "Workers" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "FirstName" character varying(50) NOT NULL,
    "LastName" character varying(50) NOT NULL,
    "PhoneNumber" character varying(15) NOT NULL,
    "OccupationalPosition" character varying(50) NOT NULL,
    "RentId" integer,
    "OfficeId" integer NOT NULL,
    CONSTRAINT "PK_Workers" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Workers_Offices_OfficeId" FOREIGN KEY ("OfficeId") REFERENCES "Offices" ("ID") ON DELETE CASCADE
);

CREATE TABLE "InsurancePolicies" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "PolicyNumber" text NOT NULL,
    "Provider" text NOT NULL,
    "Cost" numeric(18,2) NOT NULL,
    "StartDate" date NOT NULL,
    "EndDate" date NOT NULL,
    "VehicleLicensePlate" character varying(20) NOT NULL,
    CONSTRAINT "PK_InsurancePolicies" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_InsurancePolicies_Vehicles_VehicleLicensePlate" FOREIGN KEY ("VehicleLicensePlate") REFERENCES "Vehicles" ("LicensePlate") ON DELETE RESTRICT
);

CREATE TABLE "ServiceRecords" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "ServiceDate" date NOT NULL,
    "Description" text NOT NULL,
    "ServiceCost" numeric(18,2) NOT NULL,
    "VehicleLicencePlate" character varying(20) NOT NULL,
    CONSTRAINT "PK_ServiceRecords" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_ServiceRecords_Vehicles_VehicleLicencePlate" FOREIGN KEY ("VehicleLicencePlate") REFERENCES "Vehicles" ("LicensePlate") ON DELETE RESTRICT
);

CREATE TABLE "Rents" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "StartDate" date NOT NULL,
    "EndDate" date NOT NULL,
    "Cost" numeric(18,2) NOT NULL,
    "Status" character varying(20) NOT NULL,
    "VehicleLicensePlate" character varying(20) NOT NULL,
    "ClientEmail" character varying(100) NOT NULL,
    "WorkerId" integer NOT NULL,
    CONSTRAINT "PK_Rents" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Rents_Clients_ClientEmail" FOREIGN KEY ("ClientEmail") REFERENCES "Clients" ("Email") ON DELETE CASCADE,
    CONSTRAINT "FK_Rents_Vehicles_VehicleLicensePlate" FOREIGN KEY ("VehicleLicensePlate") REFERENCES "Vehicles" ("LicensePlate") ON DELETE RESTRICT,
    CONSTRAINT "FK_Rents_Workers_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Workers" ("ID") ON DELETE CASCADE
);

CREATE TABLE "DamageReports" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Description" text NOT NULL,
    "RepairCost" numeric(18,2) NOT NULL,
    "ReportDate" date NOT NULL,
    "RentId" integer NOT NULL,
    CONSTRAINT "PK_DamageReports" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_DamageReports_Rents_RentId" FOREIGN KEY ("RentId") REFERENCES "Rents" ("ID") ON DELETE RESTRICT
);

CREATE TABLE "Payments" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "PayTerm" date NOT NULL,
    "TotalCost" numeric(18,2) NOT NULL,
    "PaymentType" character varying(30) NOT NULL,
    "RentId" integer NOT NULL,
    CONSTRAINT "PK_Payments" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Payments_Rents_RentId" FOREIGN KEY ("RentId") REFERENCES "Rents" ("ID") ON DELETE CASCADE
);

CREATE TABLE "Reviews" (
    "ID" integer GENERATED BY DEFAULT AS IDENTITY,
    "Rating" integer NOT NULL,
    "Comment" character varying(500) NOT NULL,
    "ReviewDate" date NOT NULL,
    "ClientEmail" character varying(100) NOT NULL,
    "RentId" integer NOT NULL,
    CONSTRAINT "PK_Reviews" PRIMARY KEY ("ID"),
    CONSTRAINT "FK_Reviews_Clients_ClientEmail" FOREIGN KEY ("ClientEmail") REFERENCES "Clients" ("Email") ON DELETE RESTRICT,
    CONSTRAINT "FK_Reviews_Rents_RentId" FOREIGN KEY ("RentId") REFERENCES "Rents" ("ID") ON DELETE CASCADE
);

CREATE INDEX "IX_DamageReports_RentId" ON "DamageReports" ("RentId");

CREATE INDEX "IX_InsurancePolicies_VehicleLicensePlate" ON "InsurancePolicies" ("VehicleLicensePlate");

CREATE UNIQUE INDEX "IX_Payments_RentId" ON "Payments" ("RentId");

CREATE INDEX "IX_Rents_ClientEmail" ON "Rents" ("ClientEmail");

CREATE UNIQUE INDEX "IX_Rents_VehicleLicensePlate" ON "Rents" ("VehicleLicensePlate");

CREATE INDEX "IX_Rents_WorkerId" ON "Rents" ("WorkerId");

CREATE INDEX "IX_Reviews_ClientEmail" ON "Reviews" ("ClientEmail");

CREATE INDEX "IX_Reviews_RentId" ON "Reviews" ("RentId");

CREATE INDEX "IX_ServiceRecords_VehicleLicencePlate" ON "ServiceRecords" ("VehicleLicencePlate");

CREATE INDEX "IX_Vehicles_OfficeId" ON "Vehicles" ("OfficeId");

CREATE INDEX "IX_Workers_OfficeId" ON "Workers" ("OfficeId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20241011210752_Initial', '9.0.0-rc.1.24451.1');

COMMIT;

