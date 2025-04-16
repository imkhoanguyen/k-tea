import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzResultModule } from 'ng-zorro-antd/result';
@Component({
  selector: 'app-payment-failed',
  standalone: true,
  imports: [NzResultModule, NzButtonModule, RouterLink],
  templateUrl: './payment-failed.component.html',
  styleUrl: './payment-failed.component.css',
})
export class PaymentFailedComponent {}
