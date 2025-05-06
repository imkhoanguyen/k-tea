import { Component, inject, Input, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NzCarouselModule } from 'ng-zorro-antd/carousel';
import { Item } from '../../shared/models/item';
import { Pagination } from '../../shared/models/base';
import { ItemModalComponent } from '../item-modal/item-modal.component';
import { UtilitiesService } from '../../core/services/utilities.service';
import { NzToolTipModule } from 'ng-zorro-antd/tooltip';
import { NzCardModule } from 'ng-zorro-antd/card';

@Component({
  selector: 'app-item-slider',
  standalone: true,
  imports: [
    CommonModule,
    NzCarouselModule,
    ItemModalComponent,
    NzToolTipModule,
    NzCardModule,
  ],
  templateUrl: './item-slider.component.html',
  styleUrl: './item-slider.component.css',
})
export class ItemSliderComponent {
  @Input() items?: Pagination<Item>;
  @ViewChild(ItemModalComponent) itemComponent!: ItemModalComponent;
  utilService = inject(UtilitiesService);

  chunkArray(arr: Item[] | undefined, chunkSize: number): Item[][] {
    if (!arr) return [];
    const chunks = [];
    for (let i = 0; i < arr.length; i += chunkSize) {
      chunks.push(arr.slice(i, i + chunkSize));
    }
    return chunks;
  }

  showModal(id: number): void {
    this.itemComponent.showModal(id);
  }

  calculateDiscountPercent(originalPrice: number, newPrice: number): number {
    return Math.round(((originalPrice - newPrice) / originalPrice) * 100);
  }
}
