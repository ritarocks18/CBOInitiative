CREATE PROCEDURE [dbo].[spOnboardApplication]
	@UserId				UNIQUEIDENTIFIER,
	@assignToUserId		UNIQUEIDENTIFIER,
	@ApplicationName	nvarchar(max)
AS
BEGIN

DECLARE @appId int
DECLARE @Repoindex int

DECLARE @applicationLocaleId INT
DECLARE @newApplicationLocaleIds TABLE (ApplicationLocaleId INT)
DECLARE @IsAdmin INT

EXEC @IsAdmin = [dbo].[spCheckAdminPermissions] @userId = @userId
IF NOT (@IsAdmin =0)
	BEGIN
		EXEC [throwPermissionException]
	END

IF EXISTS(SELECT Id FROM [Application] WITH (NOLOCK) WHERE [Name] = @ApplicationName)
	BEGIN
	EXEC [throwApplicationAlreadyOnboardedException]
	END
ELSE
--------- OnBoarded App 
	BEGIN
	INSERT INTO [dbo].[Application] (Name, RedisInstance, RepoIndex)
	VALUES(@ApplicationName, Null, Null)
	select @appId=id from Application where Name = @ApplicationName
	END

---------Create EN-US locale for onboarded App 
	BEGIN
	PRINT 'Create en-US locale for application'
	INSERT INTO [dbo].[ApplicationLocale]
	([ApplicationId]
	,[LocaleId]
	,[Active]
	,[CreatedDate]
	,[CreatedBy])
	OUTPUT INSERTED.Id INTO @newApplicationLocaleIds
	VALUES
	(@appId
	,91
	,1
	,GETUTCDATE()
	,@userId) 
	END

-- en-US assignment, if missing for any user
	BEGIN
	INSERT INTO [UserApplicationLocale]
	SELECT @assignToUserId, ApplicationLocaleId
	FROM @newApplicationLocaleIds
	END

END

