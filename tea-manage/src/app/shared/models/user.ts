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
}

export interface UserAdd {
  userName: string;
  email: string;
  password: string;
  fullName: string;
  role: string;
  phoneNumber: string;
}

export class UserParams extends PaginationRequest {
  constructor() {
    super();
  }
}
