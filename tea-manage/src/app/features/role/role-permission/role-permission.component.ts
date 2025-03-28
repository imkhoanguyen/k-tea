import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { RoleService } from '../../../core/services/role.service';
import { ToastrService } from 'ngx-toastr';
import { PermissionGroup, Role } from '../../../shared/models/role';
import { NzButtonModule } from 'ng-zorro-antd/button';

@Component({
  selector: 'app-role-permission',
  standalone: true,
  imports: [NzButtonModule],
  templateUrl: './role-permission.component.html',
  styleUrl: './role-permission.component.css',
})
export class RolePermissionComponent {
  roleId: string = '';
  private activatedRoute = inject(ActivatedRoute);
  private roleService = inject(RoleService);
  private toastrService = inject(ToastrService);
  private router = inject(Router);
  permissionGroups: PermissionGroup[] = [];
  roleClaims: string[] = [];
  role: Role | undefined;

  ngOnInit(): void {
    this.roleId = this.activatedRoute.snapshot.paramMap.get('id')!;
    this.loadRole();
    this.loadPermissions();
    this.loadRoleClaims();
  }

  loadPermissions() {
    this.roleService.getAllPermission().subscribe({
      next: (res) => {
        this.permissionGroups = res;
      },
      error: (er) => console.log(er),
    });
  }

  loadRole() {
    this.roleService.getRole(this.roleId).subscribe({
      next: (res) => {
        this.role = res;
      },
    });
  }

  loadRoleClaims() {
    this.roleService.getRoleClaims(this.roleId).subscribe({
      next: (res) => {
        this.roleClaims = res;
      },
      error: (er) => console.log(er),
    });
  }

  onClaimToggle(claimValue: string, event: Event) {
    const isChecked = (event.target as HTMLInputElement).checked;

    if (isChecked) {
      if (!this.roleClaims.includes(claimValue)) {
        this.roleClaims.push(claimValue);
      }
    } else {
      this.roleClaims = this.roleClaims.filter((c) => c !== claimValue);
    }
  }

  saveRoleClaims() {
    this.roleService.updateRoleClaim(this.roleId, this.roleClaims).subscribe({
      next: () => {
        this.toastrService.success('Quyền đã được cập nhật thành công.');
      },
      error: (er) => {
        this.toastrService.error('Có lỗi xảy ra khi cập nhật quyền: ' + er);
      },
    });
  }

  backToRole() {
    this.router.navigate(['/quyen']);
  }
}
