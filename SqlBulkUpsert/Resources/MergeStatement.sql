
MERGE INTO {0} AS target
USING {1} AS source
ON {2}
WHEN MATCHED THEN
	UPDATE SET {3}
WHEN NOT MATCHED BY TARGET THEN
	INSERT ({4}) 
	VALUES ({4})
OUTPUT $action [action], INSERTED.$IDENTITY [ident], [source].[_Surrogate];
