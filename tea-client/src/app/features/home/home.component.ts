import { Component, inject, OnInit } from '@angular/core';
import { CarouselComponent } from '../../shared/layout/carousel/carousel.component';
import { Pagination } from '../../shared/models/base';
import { Item, ItemParams } from '../../shared/models/item';
import { ItemService } from '../../core/services/item.service';
import { ItemSliderComponent } from '../item-slider/item-slider.component';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CarouselComponent, ItemSliderComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.css',
})
export class HomeComponent implements OnInit {
  private itemService = inject(ItemService);
  items?: Pagination<Item>;
  prm = new ItemParams();

  ngOnInit(): void {
    this.getPagination();
  }

  getPagination() {
    this.prm.isFeatured = true;
    this.itemService.getPagination(this.prm).subscribe({
      next: (res) => {
        this.items = res;
      },
      error: (er) => console.log(er),
    });
  }
}
