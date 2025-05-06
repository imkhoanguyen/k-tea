import { computed, inject, Injectable, signal } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import {
  AppUser,
  User,
  UserAdd,
  UserParams,
  UserUpdate,
} from '../../shared/models/user';
import { map } from 'rxjs';
import { Pagination } from '../../shared/models/base';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = environment.apiUrl;
  private http = inject(HttpClient);
  currentUser = signal<User | null>(null);

  role = computed(() => {
    const user = this.currentUser();
    if (user && user.accessToken) {
      const role = JSON.parse(atob(user.accessToken.split('.')[1])).role;
      return role;
    }
    return null;
  });

  login(userName: string, password: string) {
    const login = {
      userName: userName,
      password: password,
    };
    return this.http.post<User>(this.apiUrl + 'auths/login', login).pipe(
      map((user) => {
        if (user) {
          this.setCurrentUser(user);
        }
      })
    );
  }

  logout() {
    localStorage.clear();
    this.currentUser.set(null);
  }

  setCurrentUser(user: User) {
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
  }

  getPagination(prm: UserParams) {
    let params = new HttpParams();

    if (prm.search.length > 0) {
      params = params.append('search', prm.search);
    }

    if (prm.orderBy.length > 0) {
      params = params.append('orderBy', prm.orderBy);
    }

    params = params.append('pageIndex', prm.pageIndex);
    params = params.append('pageSize', prm.pageSize);

    return this.http.get<Pagination<AppUser>>(this.apiUrl + 'users', {
      params,
    });
  }

  add(userAdd: UserAdd) {
    return this.http.post<AppUser>(this.apiUrl + 'users', userAdd);
  }

  update(u: UserUpdate) {
    return this.http.put<AppUser>(this.apiUrl + `users/${u.userName}`, u);
  }

  changeRole(userName: string, role: string) {
    return this.http.put(this.apiUrl + `users/${userName}/change-role`, {
      role,
    });
  }

  get(username: string) {
    return this.http.get<AppUser>(this.apiUrl + `users/${username}`);
  }

  delete(username: string) {
    return this.http.delete(this.apiUrl + `users/${username}`);
  }

  callRefreshToken(refreshToken: string) {
    return this.http.post<any>(this.apiUrl + 'auths/refresh-token', {
      refreshToken,
    });
  }

  private decodeToken(token: string): any {
    const payload = token.split('.')[1];
    return JSON.parse(atob(payload));
  }

  hasClaim(claim: string): boolean {
    const userString = localStorage.getItem('user');
    let token = null;

    if (userString) {
      const user = JSON.parse(userString);
      token = user.accessToken;
    }
    if (!token) return false;
    const decodedToken = this.decodeToken(token);
    // console.log(decodedToken);
    // console.log(decodedToken.Permission);
    return (
      decodedToken &&
      decodedToken.Permission &&
      decodedToken.Permission.includes(claim)
    );
  }
}
