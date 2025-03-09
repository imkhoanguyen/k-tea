import { Component, EventEmitter, inject, Output } from '@angular/core';
import { Category, CategoryUpdate } from '../../../shared/models/category';
import { CategoryService } from '../../../core/services/category.service';
import { ToastrService } from 'ngx-toastr';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { NzButtonModule } from 'ng-zorro-antd/button';
import { NzModalModule } from 'ng-zorro-antd/modal';
import { NzFormModule } from 'ng-zorro-antd/form';
import { NzInputModule } from 'ng-zorro-antd/input';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-category-update',
  standalone: true,
  imports: [
    NzButtonModule,
    NzModalModule,
    NzFormModule,
    ReactiveFormsModule,
    NzInputModule,
    CommonModule,
  ],
  templateUrl: './category-update.component.html',
  styleUrl: './category-update.component.css',
})
export class CategoryUpdateComponent {
  id: number = 0;
  @Output() categoryUpdated = new EventEmitter<Category>();
  private categoryService = inject(CategoryService);
  private toastrService = inject(ToastrService);
  isVisible = false;
  private fb = inject(FormBuilder);
  frm: FormGroup = new FormGroup({});
  validationErrors?: string[];

  ngOnInit(): void {
    this.initForm();
  }

  initForm() {
    this.frm = this.fb.group({
      id: 0,
      name: ['', Validators.required],
      slug: ['', Validators.required],
      description: [''],
    });
  }

  patchForm(category: Category) {
    this.frm.patchValue({
      id: category.id,
      name: category.name,
      slug: category.slug,
      description: category.description,
    });
  }

  showModal(): void {
    if (this.id == 0) {
      this.toastrService.error('Không tìm thấy danh mục');
      return;
    }
    this.categoryService.get(this.id).subscribe({
      next: (res) => {
        this.patchForm(res as Category);
        this.isVisible = true;
      },
      error: (er) => {
        console.log(er);
      },
    });
  }

  submitForm(): void {
    const categoryUpdate: CategoryUpdate = {
      id: this.frm.value.id,
      name: this.frm.value.name,
      slug: this.frm.value.slug,
      description: this.frm.value.description,
    };
    this.categoryService.update(categoryUpdate).subscribe({
      next: (res) => {
        this.categoryUpdated.emit(res as Category);
        this.handleCancel();
        this.toastrService.success('Cập nhật danh mục thành công');
      },
      error: (er) => {
        console.log(er);
        this.validationErrors = er;
      },
    });
  }

  handleCancel(): void {
    this.frm.reset();
    this.isVisible = false;
    this.validationErrors = [];
  }
}
