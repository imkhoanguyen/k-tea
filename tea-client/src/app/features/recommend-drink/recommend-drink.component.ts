import { CommonModule } from '@angular/common';
import { Component, inject, ViewChild } from '@angular/core';
import { GeminiService } from '../../core/services/gemini.service';
import { ToastrService } from 'ngx-toastr';
import { RecommendDrink } from '../../shared/models/gemini';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzInputModule } from 'ng-zorro-antd/input';
import { NzIconModule } from 'ng-zorro-antd/icon';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { ItemModalComponent } from '../item-modal/item-modal.component';

@Component({
  selector: 'app-recommend-drink',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    NzCardModule,
    NzInputModule,
    NzIconModule,
    NzButtonModule,
    ItemModalComponent,
  ],
  templateUrl: './recommend-drink.component.html',
  styleUrl: './recommend-drink.component.css',
})
export class RecommendDrinkComponent {
  private geminiService = inject(GeminiService);
  private toastrService = inject(ToastrService);
  requestText: string = '';
  recommends: RecommendDrink[] = [];
  @ViewChild(ItemModalComponent) itemComponent!: ItemModalComponent;

  getDrinkRecommend() {
    console.log(this.requestText);
    if (this.requestText.length <= 0) {
      this.toastrService.info(
        'Nhập yêu cầu của bạn để nhận gợi ý đồ uống phù hợp'
      );
    }
    this.geminiService.getRecommendDrinkList(this.requestText).subscribe({
      next: (res) => {
        this.recommends = res;
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  showModal(id: number) {
    this.itemComponent.showModal(id);
  }
}
