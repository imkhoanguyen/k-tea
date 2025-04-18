CREATE   PROCEDURE [dbo].[sp_GetDailyRevenueInMonth]
    @MonthNumber INT,
    @Year INT
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Tạo bảng tạm chứa tất cả các ngày trong tháng
    DECLARE @FirstDayOfMonth DATE = DATEFROMPARTS(@Year, @MonthNumber, 1);
    DECLARE @DaysInMonth INT = DAY(EOMONTH(@FirstDayOfMonth));
    
    DECLARE @DateTable TABLE (
        DayOfMonth INT,
        DateValue DATE,
        DayName NVARCHAR(10)
    );
    
    -- Điền dữ liệu các ngày trong tháng
    DECLARE @CurrentDay INT = 1;
    WHILE @CurrentDay <= @DaysInMonth
    BEGIN
        DECLARE @CurrentDate DATE = DATEFROMPARTS(@Year, @MonthNumber, @CurrentDay);
        INSERT INTO @DateTable VALUES (
            @CurrentDay,
            @CurrentDate,
            DATENAME(WEEKDAY, @CurrentDate)
        );
        SET @CurrentDay = @CurrentDay + 1;
    END;
    
    -- Lấy doanh thu từng ngày
    SELECT 
        dt.DayOfMonth,
        dt.DateValue,
        dt.DayName,
        ISNULL(SUM(o.SubTotal + ISNULL(o.ShippingFee, 0) - ISNULL(o.DiscountPrice, 0)), 0) AS DailyRevenue,
        ISNULL(COUNT(o.Id), 0) AS OrderCount
    FROM @DateTable dt
    LEFT JOIN [Orders] o ON 
        o.Created >= CAST(dt.DateValue AS DATETIMEOFFSET) AND
        o.Created < DATEADD(DAY, 1, CAST(dt.DateValue AS DATETIMEOFFSET)) AND
        o.IsDeleted = 0
    GROUP BY dt.DayOfMonth, dt.DateValue, dt.DayName
    ORDER BY dt.DayOfMonth;
END