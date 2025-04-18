export interface Report {
  totalOrderPerDay: number;
  totalUser: number;
  totalRevenuePerDay: number;
}

export interface DailyRevenueInMonth {
  dayOfMonth: number;
  dateValue: string;
  dayName: string;
  dailyRevenue: number;
  orderCount: number;
}

export interface TopSellingItem {
  id: number;
  name: string;
  imgUrl: string;
  totalSold: number;
  totalRevenue: number;
}
