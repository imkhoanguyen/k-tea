CREATE   PROCEDURE [dbo].[sp_GetTopSellingItems]
    @TopCount INT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT TOP (@TopCount)
        i.Id,
        i.Name AS Name,
        SUM(oi.Quantity) AS TotalSold,
        SUM(oi.Price * oi.Quantity) AS TotalRevenue,
        i.ImgUrl AS ImgUrl
    FROM [OrderItem] oi
    INNER JOIN [Items] i ON oi.ItemId = i.Id
    INNER JOIN [Orders] o ON oi.OrderId = o.Id
    WHERE o.IsDeleted = 0
    GROUP BY i.Id, i.Name, i.ImgUrl
    ORDER BY TotalSold DESC;
END