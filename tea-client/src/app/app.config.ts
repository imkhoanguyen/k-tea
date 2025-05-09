import {
  ApplicationConfig,
  provideZoneChangeDetection,
  importProvidersFrom,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { vi_VN, provideNzI18n } from 'ng-zorro-antd/i18n';
import { registerLocaleData } from '@angular/common';
import vi from '@angular/common/locales/vi';
import { FormsModule } from '@angular/forms';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { provideNzIcons } from 'ng-zorro-antd/icon';
import {
  SendOutline,
  ShoppingCartOutline,
  UserOutline,
} from '@ant-design/icons-angular/icons';
import { provideToastr } from 'ngx-toastr';
import { NgxSpinnerModule } from 'ngx-spinner';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { loadingInterceptor } from './core/interceptors/loading.interceptor';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';

registerLocaleData(vi);
const icons = [UserOutline, ShoppingCartOutline, SendOutline];

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideNzI18n(vi_VN),
    importProvidersFrom(
      FormsModule,
      NgxSpinnerModule.forRoot({
        type: 'ball-scale-multiple',
      })
    ),
    provideAnimationsAsync(),
    provideHttpClient(
      withInterceptors([errorInterceptor, loadingInterceptor, jwtInterceptor])
    ),
    provideNzIcons(icons),
    provideToastr(),
  ],
};
