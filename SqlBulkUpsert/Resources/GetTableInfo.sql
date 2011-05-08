
--DECLARE @tableName varchar(255) = 'TestUpsert'

-- Check table exists
SELECT * FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME = @tableName

-- Get column schema information for table (need this to create our temp table)
SELECT * FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = @tableName

-- Identifies the columns making up the primary key (do we use this for our match?)
SELECT kcu.COLUMN_NAME 
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
	INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
		ON kcu.CONSTRAINT_NAME = tc.CONSTRAINT_NAME
		AND CONSTRAINT_TYPE = 'PRIMARY KEY'
WHERE kcu.TABLE_NAME = @tableName
