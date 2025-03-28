import { PaginationRequest } from './base';

export interface Role {
  id: string;
  name: string;
  description: string;
}

export class RoleParams extends PaginationRequest {
  constructor() {
    super();
  }
}

export interface RoleAdd {
  name: string;
  description: string;
}

export interface RoleUpdate {
  id: string;
  name: string;
  description: string;
}

export interface Permission {
  claimValue: string;
  name: string;
}

export interface PermissionGroup {
  groupName: string;
  permissions: Permission[];
}
