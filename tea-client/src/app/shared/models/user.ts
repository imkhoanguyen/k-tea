import { PaginationRequest } from './base';

export interface User {
  userName: string;
  fullName: string;
  accessToken: string;
  refreshToken: string;
  expiredDateAccessToken: string;
}

export interface AppUser {
  userName: string;
  fullName: string;
  email: string;
  role: string;
  isLoocked: boolean;
  created: string;
  phoneNumber: string;
  address: string;
}

export interface Register {
  userName: string;
  email: string;
  password: string;
  fullName: string;
  phoneNumber: string;
  address: string;
}

export class UserParams extends PaginationRequest {
  constructor() {
    super();
  }
}
