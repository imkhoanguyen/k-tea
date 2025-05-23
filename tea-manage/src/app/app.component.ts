import { Component, inject, OnInit } from '@angular/core';
import { NavigationEnd, Router, RouterOutlet } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzLayoutModule } from 'ng-zorro-antd/layout';
import { SidebarComponent } from './shared/layout/sidebar/sidebar.component';
import { HeaderComponent } from './shared/layout/header/header.component';
import { UserService } from './core/services/user.service';
import { LoginComponent } from './features/login/login.component';
import { NgxSpinnerComponent } from 'ngx-spinner';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,
    NzButtonModule,
    NzLayoutModule,
    SidebarComponent,
    HeaderComponent,
    LoginComponent,
    NgxSpinnerComponent,
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent implements OnInit {
  title = 'tea-manage';
  userService = inject(UserService);
  isSaleComponentActive = false;
  isReportComponentActive = false;
  isUpdateUserComponentActive = false;
  ngOnInit(): void {
    this.setCurrentUser();
  }

  constructor(private router: Router) {
    this.router.events.subscribe((event) => {
      if (event instanceof NavigationEnd) {
        this.isSaleComponentActive = event.url.includes('/ban-hang');
        this.isReportComponentActive = event.url.includes('/thong-ke');
        this.isUpdateUserComponentActive = event.url.includes(
          '/cap-nhat-nguoi-dung'
        );
      }
    });
  }

  setCurrentUser() {
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user = JSON.parse(userString);
    this.userService.setCurrentUser(user);
  }
}
