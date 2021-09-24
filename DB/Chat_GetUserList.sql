
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*
EXEC [dbo].[Chat_GetUserList]  8,77

*/



alter PROCEDURE [dbo].[Chat_GetUserList] 
	@companyId INT,
	@userId INT
AS
BEGIN
	

SELECT ul.Id,ul.Name,ul.IsAdmin,ul.Admin,ul.ProfileImage,ul.Type,ISNULL(lm.SenderName,'') AS SenderName,isnull(lm.Message,'') as LastMessage,lm.SendDate,ul.UserIds,ul.UserType FROM

(
SELECT g.Id,g.Name,g.IsAdmin,g.Admin,g.ProfileImage, 2 AS Type,g.SenderName,g.LastMessage,g.SendDate,u.UserIds,UserType from
(
	SELECT g.Id,g.Name,CAST(CASE WHEN g.CretedBy=@userId THEN 1 ELSE 0 END AS BIT) AS  IsAdmin,ua.FName+' '+ua.Lname AS Admin,'/img/Group.png' ProfileImage, 2 AS Type,0 AS UserType,
	'' AS SenderName,'' AS LastMessage,NULL AS SendDate FROM Chat_Group_User gu
INNER JOIN Chat_Group g ON gu.GroupId=g.Id
INNER JOIN UserLogin u ON gu.UserId=u.Id
INNER JOIN UserLogin ua ON g.CretedBy=ua.Id
WHERE u.CompanyId=@companyId and gu.UserId=@userId AND g.IsDeleted=0 AND gu.IsDeleted=0
) g

 CROSS APPLY
( SELECT STUFF((SELECT ','+CAST(UserId AS NVARCHAR(10)) FROM Chat_Group_User WHERE GroupId=g.Id FOR XML PATH('')),1,1,'') AS UserIds ) AS u
) ul


LEFT JOIN

(SELECT ul.Id,ul.Name,ul.IsAdmin,ul.Admin,ul.ProfileImage,ul.Type,lastMessage.SenderName,lastMessage.Message,lastMessage.SendDate,u.UserIds,UserType FROM		
(SELECT g.Id,g.Name,CAST(CASE WHEN g.CretedBy=@userId THEN 1 ELSE 0 END AS BIT) AS  IsAdmin,ua.FName+' '+ua.Lname AS Admin,'/img/Group.png' ProfileImage, 2 AS Type,'' AS SenderName,'' AS LastMessage,NULL AS SendDate, 0 AS UserType FROM Chat_Group_User gu
INNER JOIN Chat_Group g ON gu.GroupId=g.Id
INNER JOIN UserLogin u ON gu.UserId=u.Id
INNER JOIN UserLogin ua ON g.CretedBy=ua.Id
WHERE u.CompanyId=@companyId and gu.UserId=@userId AND g.IsDeleted=0 AND gu.IsDeleted=0
) ul

 CROSS APPLY
( SELECT STUFF((SELECT ','+CAST(UserId AS NVARCHAR(10)) FROM Chat_Group_User WHERE GroupId=ul.Id FOR XML PATH('')),1,1,'') AS UserIds ) AS u

CROSS APPLY

(select TOP 1 CASE WHEN SenderId=@userId THEN 'You' ELSE u.FName+' '+u.Lname END AS SenderName,Message,SendDate from Chat_Message m
INNER JOIN UserLogin u ON m.SenderId=u.Id
WHERE 
ReceiverId=ul.Id AND ReceiverType=2
ORDER BY SendDate DESC
) lastMessage


) lm ON ul.Id=lm.Id

UNION ALL

SELECT ul.Id,ul.Name,ul.IsAdmin, ul.Admin,ul.ProfileImage,ul.Type,ISNULL(lm.SenderName,'') AS SenderName,isnull(lm.Message,'') as LastMessage,lm.SendDate,'' AS UserIds,ul.UserType FROM
(SELECT Id,FName+' '+Lname AS Name,CAST(0 AS BIT) AS IsAdmin,'' AS Admin,(CASE WHEN AvatarUrl IS NOT NULL THEN '/DYF/'+CAST(CompanyId AS NVARCHAR(20))+'/EmployeeImages/'+AvatarUrl ELSE '/img/Default_avatar.jpg' END)AS ProfileImage, 1 AS Type,UserType,
NULL AS SenderName,'' AS Message,NULL AS SendDate FROM UserLogin
WHERE CompanyId=@companyId AND Id<>@userId AND IsActive=1
) ul

LEFT JOIN
(SELECT * FROM
(
SELECT Id,FName+' '+Lname AS Name,CAST(0 AS BIT) AS IsAdmin,'' AS Admin,(CASE WHEN AvatarUrl IS NOT NULL THEN '/DYF/'+CAST(CompanyId AS NVARCHAR(20))+'/EmployeeImages/'+AvatarUrl ELSE '/img/Default_avatar.jpg' END)AS ProfileImage, 1 AS Type, UserType  FROM UserLogin
WHERE CompanyId=@companyId AND Id<>@userId AND IsActive=1
) AS ul
 CROSS APPLY
(
select TOP 1 CASE WHEN SenderId=@userId THEN 'You' ELSE u.FName+' '+u.Lname END AS SenderName,Message,SendDate from Chat_Message m
INNER JOIN UserLogin u ON m.SenderId=u.Id
WHERE ReceiverType=1 AND
( 
			(SenderId=@userId AND ReceiverId=ul.Id) OR 
			(SenderId=ul.Id AND ReceiverId=@userId) 
)  
ORDER BY SendDate DESC 
) AS lastMess 
) lm ON ul.Id=lm.Id

END
GO
