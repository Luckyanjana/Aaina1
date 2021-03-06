
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
ALTER PROCEDURE [dbo].[GetMenuWithPermission]
	-- Add the parameters for the stored procedure here
	@userId Int,
	@gameId int
AS
BEGIN

	
SELECT m.Id,rmp.MenuId,m.ParentId,rmp.RoleId,m.Name,m.Controller,m.Action,m.IsMain,rmp.IsList,rmp.IsView,rmp.IsAdd,rmp.IsEdit,rmp.IsDelete,m.[Order] FROM GamePlayer gp 
INNER JOIN RoleMenuPermission rmp ON gp.RoleId=rmp.RoleId
INNER JOIN Menu m ON  rmp.MenuId=m.Id
WHERE gp.UserId=@userId and gp.GameId=@gameId AND m.IsActive=1

END

