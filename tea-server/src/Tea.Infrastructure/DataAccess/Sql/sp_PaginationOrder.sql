CREATE   PROCEDURE [dbo].[sp_OrderPagination]
    @PageIndex INT,
    @PageSize INT,
    @FromDate DATETIMEOFFSET = NULL,
    @ToDate DATETIMEOFFSET = NULL,
    @MinAmount DECIMAL(18,2) = NULL,
    @MaxAmount DECIMAL(18,2) = NULL,
    @UserName NVARCHAR(256) = NULL,
    @TotalCount INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Calculate total count with filters
    SELECT @TotalCount = COUNT(*)
    FROM [Orders] o
    LEFT JOIN [AspNetUsers] u ON o.UserId = u.Id
    WHERE o.IsDeleted = 0
    AND (@FromDate IS NULL OR o.Created >= @FromDate)
    AND (@ToDate IS NULL OR o.Created <= @ToDate)
    AND (@MinAmount IS NULL OR (o.SubTotal + ISNULL(o.ShippingFee, 0) - ISNULL(o.DiscountPrice, 0)) >= @MinAmount)
    AND (@MaxAmount IS NULL OR (o.SubTotal + ISNULL(o.ShippingFee, 0) - ISNULL(o.DiscountPrice, 0)) <= @MaxAmount)
    AND (@UserName IS NULL OR u.UserName LIKE '%' + @UserName + '%' OR u.FullName LIKE '%' + @UserName + '%');
    
    -- Get paginated results
    SELECT 
        o.Id,
        o.OrderStatus,
        o.OrderType,
        o.PaymentStatus,
        o.PaymentType,
        o.Created,
        o.SubTotal,
        o.DiscountPrice,
        o.ShippingFee,
        o.DiscountId,
        o.Description,
        o.UserId,
        o.CustomerAddress,
        o.CustomerName,
        o.CustomerPhone,
        o.CreatedById,
        (o.SubTotal + ISNULL(o.ShippingFee, 0) - ISNULL(o.DiscountPrice, 0)) AS Total
    FROM [Orders] o
    LEFT JOIN [AspNetUsers] u ON o.UserId = u.Id
    WHERE o.IsDeleted = 0
    AND (@FromDate IS NULL OR o.Created >= @FromDate)
    AND (@ToDate IS NULL OR o.Created <= @ToDate)
    AND (@MinAmount IS NULL OR (o.SubTotal + ISNULL(o.ShippingFee, 0) - ISNULL(o.DiscountPrice, 0)) >= @MinAmount)
    AND (@MaxAmount IS NULL OR (o.SubTotal + ISNULL(o.ShippingFee, 0) - ISNULL(o.DiscountPrice, 0)) <= @MaxAmount)
    AND (@UserName IS NULL OR u.UserName LIKE '%' + @UserName + '%' OR u.FullName LIKE '%' + @UserName + '%')
    ORDER BY o.Created DESC
    OFFSET (@PageIndex * @PageSize) ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END