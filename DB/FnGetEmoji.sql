
GO
CREATE FUNCTION [dbo].[FnGetEmoji] (@id INT) 
returns varchar(250) 
AS 
  BEGIN 
      DECLARE @Result varchar(250)

    
	IF @id=1
	BEGIN
		SET @Result='P6xRVlHdoBM='
	END
	ELSE	 
	IF @id=2
	BEGIN
		SET @Result='yU1lupKGVNg='
	END
	ELSE	 
	IF @id=3
	BEGIN
		SET @Result='tELdnT3STqg='
	END
	ELSE	 
	IF @id=4
	BEGIN
		SET @Result='NkXbFVAb5eU='
	END
	ELSE	 
	IF @id=5
	BEGIN
		SET @Result='/c5rHtj62wU='
	END
	ELSE	 
	IF @id=6
	BEGIN
		SET @Result='9aAJiOh5xjE='
	END
	ELSE	 
	IF @id=7
	BEGIN
		SET @Result='sjYguIkLTFs='
	END
	ELSE	 
	IF @id=8
	BEGIN
		SET @Result='QnL/DIUk6iw='
	END
	ELSE	 
	IF @id=9
	BEGIN
		SET @Result='kJPkbg69yE0='
	END
	ELSE	 
	IF @id=10
	BEGIN
		SET @Result='1pT1sNMekVE='
	END
      RETURN ( @Result ) 
  END 
