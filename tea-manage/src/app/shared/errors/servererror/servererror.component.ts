import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzResultModule } from 'ng-zorro-antd/result';

@Component({
  selector: 'app-servererror',
  standalone: true,
  imports: [NzButtonModule, NzResultModule, RouterLink],
  templateUrl: './servererror.component.html',
  styleUrl: './servererror.component.css',
})
export class ServererrorComponent {}
