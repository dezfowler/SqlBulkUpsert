
IF (@@microsoftversion / 0x01000000) > 9 
  BEGIN
	exec ('
	MERGE INTO [{0}] AS target
	USING [{1}] AS source
	ON {2}
	WHEN MATCHED THEN
		UPDATE SET {3}
	WHEN NOT MATCHED BY TARGET THEN
		INSERT ({4}) 
		VALUES ({4})
	OUTPUT $action [action], INSERTED.$IDENTITY [ident], [source].[_Surrogate];')
  END
ELSE
  BEGIN
	exec('
	BEGIN TRAN
		INSERT INTO [{0}] ({4})
		SELECT {4}
		FROM [{1}] source
		WHERE NOT EXISTS(SELECT * FROM [{0}] target WHERE {2})
		
		UPDATE [{0}]
		SET {3}
		FROM [{0}] target
		JOIN [{1}] source
		WHERE {2}
	COMMIT TRAN
	')
  END