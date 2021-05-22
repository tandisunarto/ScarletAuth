IF((SELECT COUNT(*) FROM [AspNetUserClaims] WHERE ClaimType IS NULL) > 0) RAISERROR('All AspNetUserClaims must have a ClaimType value', 16, 1) 
GO

BEGIN TRANSACTION TransactionOne
	ALTER TABLE [AspNetRoles] ADD [Description] NVARCHAR(MAX) NULL
	ALTER TABLE [AspNetRoles] ADD [Reserved]	  BIT			NOT NULL DEFAULT(0)
	
	ALTER TABLE [AspNetRoles] ALTER COLUMN Reserved BIT NOT NULL

	CREATE TABLE "AspNetClaimTypes" (
		[Id]                               TEXT NOT NULL,
		[ConcurrencyStamp]                 TEXT  NULL,
		[Description]                      TEXT  NULL,
		[Name]                             TEXT  NOT NULL,
		[NormalizedName]                   TEXT  NULL,
		[Required]                         BIT            NOT NULL,
		[Reserved]                         BIT            NOT NULL,
		[Rule]                             TEXT  NULL,
		[RuleValidationFailureDescription] TEXT  NULL,
		[UserEditable]                     BIT            DEFAULT ((0)) NOT NULL,
		[ValueType]                        INT            NOT NULL,
		CONSTRAINT [PK_AspNetClaimTypes] PRIMARY KEY ([Id] ASC)
	);

	CREATE UNIQUE INDEX "IX_AspNetClaimTypes_Name" ON "AspNetClaimTypes" (
		"Name"
	);

	CREATE UNIQUE NONCLUSTERED INDEX [ClaimTypeNameIndex] ON [AspNetClaimTypes]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);
	
	WITH CTE AS (SELECT DISTINCT ClaimType FROM AspNetUserClaims)
	INSERT INTO [AspNetClaimTypes] 
		(Id,	  ConcurrencyStamp, Name,		 NormalizedName,   Required, Reserved, ValueType)
	SELECT 
		 NEWID(), NEWID(),		    ClaimType,   UPPER(ClaimType), 0,		 0,		   0
	FROM CTE
			
	ALTER TABLE [AspNetUserClaims] ALTER COLUMN ClaimType		NVARCHAR(256) NOT NULL
		
	ALTER TABLE [AspNetUserLogins] ALTER COLUMN LoginProvider NVARCHAR(450) NOT NULL
	ALTER TABLE [AspnetUserLogins] ALTER COLUMN ProviderKey	NVARCHAR(450) NOT NULL
		
	ALTER TABLE [AspNetUserTokens] ALTER COLUMN LoginProvider NVARCHAR(450) NOT NULL
	ALTER TABLE [AspNetUserTokens] ALTER COLUMN [Name]		NVARCHAR(450) NOT NULL
	
	ALTER TABLE [AspNetUsers] ADD [FirstName]           NVARCHAR (MAX) NULL
	ALTER TABLE [AspNetUsers] ADD [LastName]            NVARCHAR (MAX) NULL
	ALTER TABLE [AspNetUsers] ADD [IsBlocked]			  BIT			 NOT NULL DEFAULT(0)
	ALTER TABLE [AspNetUsers] ADD [IsDeleted]			  BIT			 NOT NULL DEFAULT(0)
		
	ALTER TABLE [AspNetUserTokens] DROP CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId]
	GO

	ALTER TABLE [AspNetUserRoles] DROP CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]
	GO

	ALTER TABLE [AspNetUserLogins] DROP CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId]
	GO

	ALTER TABLE [AspNetUserClaims] DROP CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId]
	GO

	ALTER TABLE [AspNetUsers] DROP CONSTRAINT [PK_AspNetUsers]
		
	ALTER TABLE [AspNetUsers] ADD CONSTRAINT [PK_AspNetUsers] PRIMARY KEY NONCLUSTERED ([Id] ASC)
	
	ALTER TABLE [AspNetUserTokens] ADD CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY (UserId) REFERENCES [AspNetUsers] ([Id])
	ALTER TABLE [AspNetUserRoles]  ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId]  FOREIGN KEY (UserId) REFERENCES [AspNetUsers] ([Id])
	ALTER TABLE [AspNetUserRoles]  ADD CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY (UserId) REFERENCES [AspNetUsers] ([Id])
	ALTER TABLE [AspNetUserClaims] ADD CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY (UserId) REFERENCES [AspNetUsers] ([Id])
	
	IF (NOT EXISTS (SELECT * 
					 FROM INFORMATION_SCHEMA.TABLES 
					 WHERE TABLE_SCHEMA = 'dbo' 
					 AND  TABLE_NAME = '__EFMigrationsHistory'))
	BEGIN
		CREATE TABLE [__EFMigrationsHistory](
			[MigrationId] [nvarchar](150) NOT NULL,
			[ProductVersion] [nvarchar](32) NOT NULL,
		 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
		(
			[MigrationId] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]
	END

	INSERT INTO [__EFMigrationsHistory]
	VALUES ('20171026080706_InitialSqlServerIdentityDbMigration', '2.1.4-rtm-31024')
	

COMMIT TRANSACTION TransactionOne