﻿IF exists(SELECT '1' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Application' AND COLUMN_NAME = 'RedisInstance')
BEGIN
    ALTER TABLE Application
    DROP Column RedisInstance 
END

GO
IF exists(SELECT '1' FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Application' AND COLUMN_NAME = 'RepoIndex')
BEGIN
    ALTER TABLE Application
    DROP Column RepoIndex 
END