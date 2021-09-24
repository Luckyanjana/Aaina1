
GO
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
/*

EXEC [dbo].[Chat_GetChatMessage] 77,60,1

EXEC [dbo].[Chat_GetChatMessage] 77,1,2

*/


alter PROCEDURE [dbo].[Chat_GetChatMessage] 
	@senderId INT,
	@receiverId INT,
	@receiveType TINYINT
AS
BEGIN
	
select SenderId,u.FName+' '+u.Lname  AS SenderName,ReceiverId,
(CASE WHEN AvatarUrl IS NOT NULL THEN '/DYF/'+CAST(CompanyId AS NVARCHAR(20))+'/EmployeeImages/'+AvatarUrl ELSE '/img/Default_avatar.jpg' END)AS ProfileImage,
Message,SendDate,IsRead from Chat_Message m
INNER JOIN UserLogin u ON m.SenderId=u.Id
WHERE 
(
	(
		@receiveType =1 AND 
		( 
			(SenderId=@senderId AND ReceiverId=@receiverId) OR 
			(SenderId=@receiverId AND ReceiverId=@senderId) 
		) 
	) OR 
	(
	@receiveType=2 AND
	ReceiverId=@receiverId
	)
) AND 
ReceiverType=@receiveType


ORDER BY SendDate DESC

END
GO


