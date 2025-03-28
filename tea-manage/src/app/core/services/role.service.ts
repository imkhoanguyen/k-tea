import { inject, Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  PermissionGroup,
  Role,
  RoleAdd,
  RoleParams,
  RoleUpdate,
} from '../../shared/models/role';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getPagination(prm: RoleParams) {
    let params = new HttpParams();

    if (prm.search.length > 0) {
      params = params.append('search', prm.search);
    }

    if (prm.orderBy.length > 0) {
      params = params.append('orderBy', prm.orderBy);
    }

    params = params.append('pageIndex', prm.pageIndex);
    params = params.append('pageSize', prm.pageSize);

    return this.http.get<Pagination<Role>>(this.apiUrl + 'roles', {
      params,
    });
  }

  add(r: RoleAdd) {
    return this.http.post(this.apiUrl + 'roles', r);
  }

  update(r: RoleUpdate) {
    return this.http.put(this.apiUrl + `roles/${r.id}`, r);
  }

  get(id: string) {
    return this.http.get(this.apiUrl + `roles/${id}`);
  }

  delete(id: string) {
    return this.http.delete(this.apiUrl + `roles/${id}`);
  }

  getAllPermission() {
    return this.http.get<PermissionGroup[]>(this.apiUrl + 'roles/permissions');
  }

  getRole(roleId: string) {
    return this.http.get<Role>(this.apiUrl + `roles/${roleId}`);
  }

  getRoleClaims(roleId: string) {
    return this.http.get<string[]>(this.apiUrl + `roles/${roleId}/claims`);
  }

  updateRoleClaim(roleId: string, listClaimValue: string[]) {
    return this.http.put(
      this.apiUrl + `roles/${roleId}/update-claims`,
      listClaimValue
    );
  }
}
